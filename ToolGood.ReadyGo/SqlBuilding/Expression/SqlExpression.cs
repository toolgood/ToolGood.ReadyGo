using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.Poco;

namespace ToolGood.ReadyGo.SqlBuilding
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlExpression
    {
        protected const string sep = " ";

        #region Resolve
        public static SqlExpression Resolve(SqlType type)
        {
            switch (type) {
                case SqlType.SqlServer:
                case SqlType.MySql:
                case SqlType.SQLite:
                case SqlType.MsAccessDb:
                case SqlType.Oracle:
                case SqlType.PostgreSQL:
                case SqlType.FirebirdDb:
                case SqlType.MariaDb:
                case SqlType.SqlServerCE:
                default: break;
            }
            return Singleton<SqlExpression>.Instance;
        }


        #endregion

        #region 可重写的方法
        protected virtual string GetQuotedValue(string paramValue)
        {
            return "'" + paramValue.Replace("'", "''") + "'";
        }
        protected virtual string GetQuotedValue(object value, Type fieldType)
        {
            if (value == null) return "NULL";

            if (fieldType.IsEnum) {
                var isEnumFlags = fieldType.IsEnum;
                long enumValue;
                if (!isEnumFlags && Int64.TryParse(value.ToString(), out enumValue)) {
                    value = Enum.ToObject(fieldType, enumValue).ToString();
                }
                var enumString = value.ToString();

                return !isEnumFlags
                    ? GetQuotedValue(enumString.Trim('"'))
                    : enumString;
            }

            var typeCode = Type.GetTypeCode(fieldType);
            switch (typeCode) {
                case TypeCode.Boolean:
                    return (bool)value ? "1" : "0";

                case TypeCode.Single:
                    return ((float)value).ToString(CultureInfo.InvariantCulture);

                case TypeCode.Double:
                    return ((double)value).ToString(CultureInfo.InvariantCulture);

                case TypeCode.Decimal:
                    return ((decimal)value).ToString(CultureInfo.InvariantCulture);

                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    //if (IsNumericType(fieldType))
                    return Convert.ChangeType(value, fieldType).ToString();
                    //break;
            }

            if (fieldType == typeof(TimeSpan))
                return ((TimeSpan)value).Ticks.ToString(CultureInfo.InvariantCulture);
            // TO： add 用于sqlite

            return GetQuotedValue(value.ToString());
        }
        protected virtual object GetQuotedTrueValue()
        {
            return new PartialSqlString("1");
        }
        protected virtual object GetQuotedFalseValue()
        {
            return new PartialSqlString("0");
        }

        protected virtual object VisitMethodCall(MethodCallExpression m, Dictionary<ParameterExpression, string> paramDicts)
        {
            if (m.Method.DeclaringType == typeof(ObjectExtend))
                return VisitObjectExtendMethodCall(m, paramDicts);

            if (m.Method.DeclaringType == typeof(SQL))
                return VisitSqlMethodCall(paramDicts, m);

            if (IsStaticArrayMethod(m, paramDicts))
                return VisitStaticArrayMethodCall(m, paramDicts);

            if (IsEnumerableMethod(m, paramDicts))
                return VisitEnumerableMethodCall(m, paramDicts);

            if (IsColumnAccess(m, paramDicts))
                return VisitColumnAccessMethod(m, paramDicts);

            return Expression.Lambda(m).Compile().DynamicInvoke();
        }
        protected virtual string VisitSqlMethodCall(Dictionary<ParameterExpression, string> paramDicts, MethodCallExpression call)
        {
            if (call.Method.DeclaringType != typeof(SQL)) throw new Exception("无效列！！！");
            var callName = call.Method.Name;
            if (callName == "CountAll") return "COUNT(*)";

            List<Object> args = new List<object>();
            var original = call.Arguments;
            string quotedColName = null;

            for (int i = 0, n = original.Count; i < n; i++) {
                var o = original[i];
                if (o.NodeType == ExpressionType.MemberAccess) {
                    quotedColName = getColumnName(paramDicts, o as MemberExpression);
                } else {
                    args.Add((o as ConstantExpression).Value);
                }
            }

            if (callName == "CountOfDistinct") {
                return string.Format("COUNT(DISTINCT {0})", quotedColName);
            }
            return string.Format("{0}({1}{2})",
                callName.ToUpper(), quotedColName,
                args.Count == 1 ? string.Format(",'{0}'", args[0]) : ""
                );
        }

        // String 类方法调用
        protected virtual object VisitColumnAccessMethod(MethodCallExpression m, Dictionary<ParameterExpression, string> paramDicts)
        {
            List<Object> _args = this.VisitExpressionList(m.Arguments, paramDicts);
            var quotedColName = Visit(m.Object, paramDicts);
            var statement = "";

            var wildcardArg = _args.Count > 0 ? _args[0] != null ? EscapeWildcards(_args[0].ToString()) : "" : "";
            var escapeSuffix = wildcardArg.IndexOf('^') >= 0 ? " escape '^'" : "";
            switch (m.Method.Name) {
                case "Trim": statement = string.Format("ltrim(rtrim({0}))", quotedColName); break;
                case "LTrim": statement = string.Format("ltrim({0})", quotedColName); break;
                case "RTrim": statement = string.Format("rtrim({0})", quotedColName); break;
                case "ToUpper": statement = string.Format("upper({0})", quotedColName); break;
                case "ToLower": statement = string.Format("lower({0})", quotedColName); break;
                case "StartsWith": statement = string.Format("{0} like {1}{2}", quotedColName, GetQuotedValue(wildcardArg + "%"), escapeSuffix); break;
                case "EndsWith": statement = string.Format("{0} like {1}{2}", quotedColName, GetQuotedValue("%" + wildcardArg), escapeSuffix); break;
                case "Contains": statement = string.Format("{0} like {1}{2}", quotedColName, GetQuotedValue("%" + wildcardArg + "%"), escapeSuffix); break;
                case "Substring":
                    var startIndex = Int32.Parse(_args[0].ToString()) + 1;
                    var length = (_args.Count > 1) ? Int32.Parse(_args[1].ToString()) : -1;
                    statement = SubstringStatement(quotedColName, startIndex, length);
                    break;

                case "Equals":
                    statement = string.Format("({0} = {1})", quotedColName, GetQuotedValue(wildcardArg));
                    break;

                case "ToString":
                    statement = quotedColName.ToString();
                    break;

                default:
                    throw new NotSupportedException();
            }
            return new PartialSqlString(statement);
        }
        protected virtual string SubstringStatement(object columnName, int startIndex, int length)
        {
            if (length >= 0)
                return string.Format("substring({0},{1},{2})", columnName, startIndex, length);
            else
                return string.Format("substring({0},{1},8000)", columnName, startIndex);
        }

        #endregion


        #region Analysis getColumnName


        public void Analysis(LambdaExpression exp, out string sql)
        {
            Dictionary<ParameterExpression, string> paramDicts = new Dictionary<ParameterExpression, string>();
            for (int i = 0; i < exp.Parameters.Count; i++) {
                var item = exp.Parameters[i];
                paramDicts[item] = "t" + (i + 1).ToString();
            }
            sql = Visit(exp, paramDicts).ToString();

        }

        public string GetColumnName(LambdaExpression exp)
        {
            Dictionary<ParameterExpression, string> paramDicts = new Dictionary<ParameterExpression, string>();
            for (int i = 0; i < exp.Parameters.Count; i++) {
                var item = exp.Parameters[i];
                paramDicts[item] = "t" + (i + 1).ToString();
            }

            if (exp.Body.NodeType == ExpressionType.MemberAccess) {
                return getColumnName(paramDicts, exp.Body as MemberExpression);
            }
            if (exp.Body.NodeType != ExpressionType.Call) {
                return getColumnName(paramDicts, (exp.Body as UnaryExpression).Operand.Reduce() as MemberExpression);
            }

            var call = exp.Body as MethodCallExpression;
            return VisitSqlMethodCall(paramDicts, call);
        }

        protected virtual string getColumnName(Dictionary<ParameterExpression, string> paramDicts, MemberExpression m)
        {
            var colName = m.Member.Name;
            var p = m.Expression as ParameterExpression;
            var tableDef = PocoData.ForType(p.Type);
            var col = tableDef.Columns.FirstOrDefault(q => q.PropertyInfo.Name == colName);
            if (col != null) {
                colName = col.ColumnName;
            }
            return string.Format("{0}.{1}", paramDicts[p], colName);
        }


        #endregion getColumnName

        #region GetSelectSql

        protected object GetTrueExpression()
        {
            return new PartialSqlString(string.Format("({0}={1})", GetQuotedTrueValue().ToString(), GetQuotedTrueValue().ToString()));
        }

        protected object GetFalseExpression()
        {
            return new PartialSqlString(string.Format("({0}={1})", GetQuotedTrueValue().ToString(), GetQuotedFalseValue().ToString()));
        }



        #endregion GetSelectSql

        #region Expression Visit

        protected internal object Visit(Expression exp, Dictionary<ParameterExpression, string> paramDicts)
        {
            if (exp == null) return string.Empty;
            switch (exp.NodeType) {
                case ExpressionType.Lambda: return VisitLambda(exp as LambdaExpression, paramDicts);
                case ExpressionType.MemberAccess: return VisitMemberAccess(exp as MemberExpression, paramDicts);
                case ExpressionType.Constant: return VisitConstant(exp as ConstantExpression, paramDicts);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr: return VisitBinary(exp as BinaryExpression, paramDicts);
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs: return VisitUnary(exp as UnaryExpression, paramDicts);
                case ExpressionType.Parameter: return VisitParameter(exp as ParameterExpression, paramDicts);
                case ExpressionType.Call: return VisitMethodCall(exp as MethodCallExpression, paramDicts);
                case ExpressionType.New: return VisitNew(exp as NewExpression, paramDicts);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds: return VisitNewArray(exp as NewArrayExpression, paramDicts);
                case ExpressionType.MemberInit: return VisitMemberInit(exp as MemberInitExpression, paramDicts);
                case ExpressionType.Conditional:
                    return VisitConditional(exp as ConditionalExpression, paramDicts);
                default: return exp.ToString();
            }
        }
        protected virtual object VisitConditional(ConditionalExpression conditional, Dictionary<ParameterExpression, string> paramDicts)
        {
            var test = Visit(conditional.Test, paramDicts);
            var trueSql = Visit(conditional.IfTrue, paramDicts);
            var falseSql = Visit(conditional.IfFalse, paramDicts);

            return new PartialSqlString(string.Format("(case when {0} then {1} else {2} end)", test, trueSql, falseSql));
        }
        protected object VisitLambda(LambdaExpression lambda, Dictionary<ParameterExpression, string> paramDicts)
        {
            if (lambda.Body.NodeType == ExpressionType.MemberAccess) {
                MemberExpression m = lambda.Body as MemberExpression;

                if (m.Expression != null) {
                    string r = VisitMemberAccess(m, paramDicts).ToString();
                    return string.Format("{0}={1}", r, GetQuotedTrueValue());
                }
            }
            return Visit(lambda.Body, paramDicts);
        }

        protected object VisitBinary(BinaryExpression b, Dictionary<ParameterExpression, string> paramDicts)
        {
            var operand = BindOperant(b.NodeType);   //sep= " " ??
            if (operand == "AND" || operand == "OR") {
                return VisitBinary_And_Or(b, operand, paramDicts);
            }
            object left = Visit(b.Left, paramDicts);
            object right = Visit(b.Right, paramDicts);

            if (left as PartialSqlString == null && right as PartialSqlString == null) {
                var result = Expression.Lambda(b).Compile().DynamicInvoke();
                return result;
            } else {
                if (left as PartialSqlString == null) {
                    left = GetQuotedValue(left, left != null ? left.GetType() : null);
                    return CreatePartialSqlString(left, operand, right, paramDicts);
                } else if (right as PartialSqlString == null) {
                    if (right == null) {
                        right = GetQuotedValue(right, null);
                        if (operand == "=") operand = "IS";
                        else if (operand == "<>") operand = "IS NOT";
                        return new PartialSqlString(left + sep + operand + sep + right);
                    } else {
                        right = GetQuotedValue(right, right != null ? right.GetType() : null);
                        return CreatePartialSqlString(left, operand, right, paramDicts);
                    }
                }
            }

            if (operand == "=" && right.ToString().Equals("null", StringComparison.OrdinalIgnoreCase)) operand = "IS";
            else if (operand == "<>" && right.ToString().Equals("null", StringComparison.OrdinalIgnoreCase)) operand = "IS NOT";

            return CreatePartialSqlString(left, operand, right, paramDicts);
        }

        private PartialSqlString CreatePartialSqlString(object left, string operand, object right, Dictionary<ParameterExpression, string> paramDicts)
        {
            if (operand == "MOD" || operand == "COALESCE") {
                return new PartialSqlString(string.Format("{0}({1},{2})", operand, left, right));
            }
            return new PartialSqlString(left + sep + operand + sep + right);
        }

        protected object VisitBinary_And_Or(BinaryExpression b, string operand, Dictionary<ParameterExpression, string> paramDicts)
        {
            object left, right;
            var m = b.Left as MemberExpression;
            if (m != null && m.Expression != null
                && m.Expression.NodeType == ExpressionType.Parameter)
                left = new PartialSqlString(string.Format("{0}={1}", VisitMemberAccess(m, paramDicts), GetQuotedTrueValue()));
            else
                left = Visit(b.Left, paramDicts);

            m = b.Right as MemberExpression;
            if (m != null && m.Expression != null
                && m.Expression.NodeType == ExpressionType.Parameter)
                right = new PartialSqlString(string.Format("{0}={1}", VisitMemberAccess(m, paramDicts), GetQuotedTrueValue()));
            else
                right = Visit(b.Right, paramDicts);

            if (left as PartialSqlString == null && right as PartialSqlString == null) {
                var result = Expression.Lambda(b).Compile().DynamicInvoke();
                return new PartialSqlString(GetQuotedValue(result, result.GetType()));
            }

            if (left as PartialSqlString == null)
                left = ((bool)left) ? GetTrueExpression() : GetFalseExpression();
            if (right as PartialSqlString == null)
                right = ((bool)right) ? GetTrueExpression() : GetFalseExpression();
            if (operand == "OR") {
                return new PartialSqlString("(" + left + sep + operand + sep + right + ")");
            }
            return new PartialSqlString(left + sep + operand + sep + right);
        }

        protected object VisitMemberAccess(MemberExpression m, Dictionary<ParameterExpression, string> paramDicts)
        {
            if (m.Expression != null
                && (m.Expression.NodeType == ExpressionType.Parameter || m.Expression.NodeType == ExpressionType.Convert)) {
                var me = m;
                if (m.Expression.NodeType == ExpressionType.Convert) {
                    me = (m.Expression as UnaryExpression).Operand.Reduce() as MemberExpression;
                }

                var p = me.Expression as ParameterExpression;
                var tableDef = PocoData.ForType(p.Type);
                var colName = m.Member.Name;

                var col = tableDef.Columns.FirstOrDefault(q => q.PropertyInfo.Name == colName);
                if (col != null) {
                    colName = col.ColumnName;
                }
                return new PartialSqlString(string.Format("{0}.{1}", paramDicts[p], colName));
            }

            var member = Expression.Convert(m, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(member);
            var getter = lambda.Compile();
            return getter();
        }

        private static bool IsEnum(PropertyInfo propertyInfo)
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            return propertyInfo.PropertyType.IsEnum || (underlyingType != null && underlyingType.IsEnum);
        }

        protected object VisitMemberInit(MemberInitExpression exp, Dictionary<ParameterExpression, string> paramDicts)
        {
            return Expression.Lambda(exp).Compile().DynamicInvoke();
        }

        protected object VisitNew(NewExpression nex, Dictionary<ParameterExpression, string> paramDicts)
        {
            var member = Expression.Convert(nex, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(member);
            try {
                return lambda.Compile()();
            } catch (System.InvalidOperationException) {
                List<PartialSqlString> exprs = VisitExpressionList(nex.Arguments, paramDicts).OfType<PartialSqlString>().ToList();
                StringBuilder r = new StringBuilder();
                //for (int i = 0; i < exprs.Count; i++) {


                //if (exprs[i] is MemberAccessString) {
                //    selectMembers.Add(new SelectMember() {
                //        EntityType = ((MemberAccessString)exprs[i]).Type,
                //        PocoColumn = ((MemberAccessString)exprs[i]).PocoColumn,
                //        PocoColumns = ((MemberAccessString)exprs[i]).PocoColumns,
                //    });
                //}
                //}
                return r.ToString();
            }

            //// TODO : check !
            //var member = Expression.Convert(nex, typeof(object));
            //var lambda = Expression.Lambda<Func<object>>(member);
            //try {
            //    return lambda.Compile();
            //} catch (InvalidOperationException) { // FieldName ?
            //    return string.Join(",", VisitExpressionList(nex.Arguments,  paramDicts));
            //}
        }

        protected object VisitParameter(ParameterExpression p, Dictionary<ParameterExpression, string> paramDicts)
        {
            return paramDicts[p];
        }

        protected object VisitConstant(ConstantExpression c, Dictionary<ParameterExpression, string> paramDicts)
        {
            if (c.Value == null) return new PartialSqlString("NULL");
            return c.Value;
        }

        protected object VisitUnary(UnaryExpression u, Dictionary<ParameterExpression, string> paramDicts)
        {
            switch (u.NodeType) {
                case ExpressionType.Not:
                    var o = Visit(u.Operand, paramDicts);
                    if (o as PartialSqlString == null) return !((bool)o);
                    if (u.Operand.NodeType == ExpressionType.MemberAccess) {
                        o = o + "=" + GetQuotedTrueValue();
                    }
                    return new PartialSqlString("NOT (" + o + ")");

                case ExpressionType.Convert:
                    if (u.Method != null)
                        return Expression.Lambda(u).Compile().DynamicInvoke();
                    break;
            }
            return Visit(u.Operand, paramDicts);
        }


        protected string BindOperant(ExpressionType e)
        {
            switch (e) {
                case ExpressionType.Equal: return "=";
                case ExpressionType.NotEqual: return "<>";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
                case ExpressionType.AndAlso: return "AND";
                case ExpressionType.OrElse: return "OR";
                case ExpressionType.Add: return "+";
                case ExpressionType.Subtract: return "-";
                case ExpressionType.Multiply: return "*";
                case ExpressionType.Divide: return "/";
                case ExpressionType.Modulo: return "MOD";
                case ExpressionType.Coalesce: return "COALESCE";
                default: return e.ToString();
            }
        }

        protected string RemoveQuoteFromAlias(string exp, Dictionary<ParameterExpression, string> paramDicts)
        {
            if ((exp.StartsWith("\"") || exp.StartsWith("`") || exp.StartsWith("'"))
                &&
                (exp.EndsWith("\"") || exp.EndsWith("`") || exp.EndsWith("'"))) {
                exp = exp.Remove(0, 1);
                exp = exp.Remove(exp.Length - 1, 1);
            }
            return exp;
        }

        #endregion Expression Visit

        #region IsNumericType IsOrHasGenericInterfaceTypeOf

        private bool IsOrHasGenericInterfaceTypeOf(Type type, Type genericTypeDefinition,
            Dictionary<ParameterExpression, string> paramDicts)
        {
            if (GetTypeWithGenericTypeDefinitionOf(type, genericTypeDefinition, paramDicts) == null) {
                return (type == genericTypeDefinition);
            }
            return true;
        }

        private Type GetTypeWithGenericTypeDefinitionOf(Type type, Type genericTypeDefinition,
            Dictionary<ParameterExpression, string> paramDicts)
        {
            foreach (Type t in type.GetInterfaces()) {
                if (t.IsGenericType && (t.GetGenericTypeDefinition() == genericTypeDefinition)) {
                    return t;
                }
            }
            Type genericType = GetGenericType(type);
            if ((genericType != null) && (genericType.GetGenericTypeDefinition() == genericTypeDefinition)) {
                return genericType;
            }
            return null;
        }

        private Type GetGenericType(Type type)
        {
            while (type != null) {
                if (type.IsGenericType) {
                    return type;
                }
                type = type.BaseType;
            }
            return null;
        }

        #endregion IsNumericType IsOrHasGenericInterfaceTypeOf

        #region 分析方法调用


        private bool IsStaticArrayMethod(MethodCallExpression m, Dictionary<ParameterExpression, string> paramDicts)
        {
            if (m.Object == null && m.Method.Name == "Contains") {
                return m.Arguments.Count == 2;
            }
            return false;
        }

        protected object VisitStaticArrayMethodCall(MethodCallExpression m, Dictionary<ParameterExpression, string> paramDicts)
        {
            switch (m.Method.Name) {
                case "Contains":
                    List<Object> _args = this.VisitExpressionList(m.Arguments, paramDicts);
                    object quotedColName = _args[1];

                    Expression memberExpr = m.Arguments[0];
                    if (memberExpr.NodeType == ExpressionType.MemberAccess)
                        memberExpr = (m.Arguments[0] as MemberExpression);

                    return ToInPartialString(memberExpr, quotedColName, "IN", paramDicts);

                default:
                    throw new NotSupportedException();
            }
        }

        private bool IsEnumerableMethod(MethodCallExpression m, Dictionary<ParameterExpression, string> paramDicts)
        {
            if (m.Object != null
                && IsOrHasGenericInterfaceTypeOf(m.Object.Type, typeof(IEnumerable<>), paramDicts)
                && m.Object.Type != typeof(string)
                && m.Method.Name == "Contains") {
                return m.Arguments.Count == 1;
            }
            return false;
        }

        protected object VisitEnumerableMethodCall(MethodCallExpression m, Dictionary<ParameterExpression, string> paramDicts)
        {
            switch (m.Method.Name) {
                case "Contains":
                    List<Object> _args = this.VisitExpressionList(m.Arguments, paramDicts);
                    object quotedColName = _args[0];
                    return ToInPartialString(m.Object, quotedColName, "IN", paramDicts);

                default:
                    throw new NotSupportedException();
            }
        }

        private object ToInPartialString(Expression memberExpr, object quotedColName, string option,
            Dictionary<ParameterExpression, string> paramDicts)
        {
            var member = Expression.Convert(memberExpr, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(member);
            var getter = lambda.Compile();
            List<object> inArgs = new List<object>();
            foreach (var item in getter() as IEnumerable) {
                inArgs.Add(item);
            }
            if (inArgs.Count == 0) {
                if (option == "IN") {
                    return new PartialSqlString("1 <> 1");
                } else {
                    return new PartialSqlString("1 == 1");
                }
            }
            if (inArgs.Count == 1) {
                if (option == "IN") {
                    return new PartialSqlString(string.Format("{0}={1}", quotedColName,
                        GetQuotedValue(inArgs[0], inArgs[0].GetType()), paramDicts));
                } else {
                    return new PartialSqlString(string.Format("{0}<>{1}", quotedColName,
                        GetQuotedValue(inArgs[0], inArgs[0].GetType()), paramDicts));
                }
            }

            var sIn = new StringBuilder();
            if (inArgs.Count > 0) {
                foreach (object e in inArgs) {
                    if (sIn.Length > 0)
                        sIn.Append(",");
                    sIn.Append(GetQuotedValue(e, e.GetType()));
                }
            }

            var statement = string.Format("{0} {1} ({2})", quotedColName, option, sIn);
            return new PartialSqlString(statement);
        }

        protected bool IsColumnAccess(MethodCallExpression m, Dictionary<ParameterExpression, string> paramDicts)
        {
            if (m.Object != null && m.Object as MethodCallExpression != null)
                return IsColumnAccess(m.Object as MethodCallExpression, paramDicts);

            var exp = m.Object as MemberExpression;
            return exp != null
                && exp.Expression != null
                && exp.Expression.NodeType == ExpressionType.Parameter;
        }


        private object VisitObjectExtendMethodCall(MethodCallExpression m, Dictionary<ParameterExpression, string> paramDicts)
        {
            if (m.Arguments[0].NodeType != ExpressionType.MemberAccess) {
                return Expression.Lambda(m).Compile().DynamicInvoke();
            }
            var quotedColName = VisitMemberAccess((MemberExpression)m.Arguments[0], paramDicts);
            var option = m.Method.Name == "IsIn" ? "IN" : "NOT IN";
            return ToInPartialString(m.Arguments[1], quotedColName, option, paramDicts);
        }





        private string EscapeWildcards(string value)
        {
            if (value == null)
                return null;
            return value
                .Replace("^", @"^^")
                .Replace("_", @"^_")
                .Replace("%", @"^%");
        }

        #endregion 分析方法调用

        #region 获取 集合

        protected List<Object> VisitExpressionList(ReadOnlyCollection<Expression> original, Dictionary<ParameterExpression, string> paramDicts)
        {
            List<Object> list = new List<Object>();
            for (int i = 0, n = original.Count; i < n; i++) {
                if (original[i].NodeType == ExpressionType.NewArrayInit ||
                        original[i].NodeType == ExpressionType.NewArrayBounds)
                    list.AddRange(VisitNewArrayFromExpressionList(original[i] as NewArrayExpression, paramDicts));
                else
                    list.Add(Visit(original[i], paramDicts));
            }
            return list;
        }

        protected object VisitNewArray(NewArrayExpression na, Dictionary<ParameterExpression, string> paramDicts)
        {
            return string.Join(",", VisitExpressionList(na.Expressions, paramDicts));
        }

        protected List<Object> VisitNewArrayFromExpressionList(NewArrayExpression na, Dictionary<ParameterExpression, string> paramDicts)
        {
            return VisitExpressionList(na.Expressions, paramDicts);
        }

        #endregion 获取 集合

    }
}
