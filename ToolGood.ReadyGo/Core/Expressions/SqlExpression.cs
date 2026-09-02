using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using ToolGood.ReadyGo.NPoco.Internal;
using ToolGood.ReadyGo.NPoco.Linq;

namespace ToolGood.ReadyGo.NPoco.Expressions
{
    /// <summary>
    /// 泛型 SQL 表达式生成器基类，负责将 Lambda 表达式树转换为数据库方言的 SQL 语句。
    /// </summary>
    /// <typeparam name="T">查询对应的实体类型。</typeparam>
    public abstract class SqlExpression<T> : ISqlExpression<T>
    {
        private List<OrderByMember> orderByMembers = new List<OrderByMember>();
        private List<SelectMember> selectMembers = new List<SelectMember>();
        private List<GeneralMember> generalMembers = new List<GeneralMember>();
        private string whereExpression;
        private string groupBy = string.Empty;
        private string orderBy = string.Empty;
        private string tableHint = string.Empty;

        List<OrderByMember> ISqlExpression.OrderByMembers { get { return orderByMembers; } }
        List<SelectMember> ISqlExpression.SelectMembers { get { return selectMembers; } }
        List<GeneralMember> ISqlExpression.GeneralMembers { get { return generalMembers; } }
        string ISqlExpression.WhereSql { get { return whereExpression; } }
        int? ISqlExpression.Rows { get { return Rows; } }
        int? ISqlExpression.Skip { get { return Skip; } }
        Type ISqlExpression.Type { get { return _type; } }
        object[] ISqlExpression.Params { get { return Context.Params; } }
        string ISqlExpression.TableHint { get { return tableHint; } }

        string ISqlExpression.ApplyPaging(string sql, IEnumerable<PocoColumn[]> columns, Dictionary<string, JoinData> joinSqlExpressions)
        {
            return ApplyPaging(sql, columns, joinSqlExpressions);
        }

        private string sep = string.Empty;
        /// <summary>
        /// LIKE 语句中使用的转义字符。
        /// </summary>
        protected string EscapeChar = "\\";
        private PocoData _pocoData;
        private readonly IDatabase _database;
        private readonly IDatabaseType _databaseType;
        private bool PrefixFieldWithTableName { get; set; }
        private Type _type { get; set; }

        /// <summary>
        /// 使用指定数据库、Poco 元数据与表名前缀标志初始化实例。
        /// </summary>
        /// <param name="database">数据库实例。</param>
        /// <param name="pocoData">实体的 Poco 元数据。</param>
        /// <param name="prefixTableName">是否在字段前添加表名前缀。</param>
        public SqlExpression(IDatabase database, PocoData pocoData, bool prefixTableName)
        {
            _type = typeof(T);
            _pocoData = pocoData;
            _database = database;
            _databaseType = database.DatabaseType;
            PrefixFieldWithTableName = prefixTableName;
            paramPrefix = "@";
            Context = new SqlExpressionContext(this);
        }

        /// <summary>
        /// SQL 表达式上下文，提供语句生成与参数、更新字段访问。
        /// </summary>
        public class SqlExpressionContext : ISqlExpression<T>.ISqlExpressionContext
        {
            private readonly SqlExpression<T> _expression;

            /// <summary>
            /// 使用所属表达式初始化上下文。
            /// </summary>
            /// <param name="expression">所属的 SQL 表达式。</param>
            public SqlExpressionContext(SqlExpression<T> expression)
            {
                _expression = expression;
                UpdateFields = new List<string>();
            }

            /// <summary>
            /// 更新字段名称集合。
            /// </summary>
            public List<string> UpdateFields { get; set; }
            /// <summary>
            /// 查询参数数组。
            /// </summary>
            public object[] Params { get { return _expression._params.ToArray(); } }

            /// <summary>
            /// 生成删除语句。
            /// </summary>
            /// <returns>删除 SQL。</returns>
            public virtual string ToDeleteStatement()
            {
                return _expression.ToDeleteStatement();
            }

            /// <summary>
            /// 生成更新语句。
            /// </summary>
            /// <param name="item">待更新的实体。</param>
            /// <param name="excludeDefaults">是否排除默认值字段。</param>
            /// <param name="allFields">是否更新所有字段。</param>
            /// <returns>更新 SQL。</returns>
            public virtual string ToUpdateStatement(T item, bool excludeDefaults, bool allFields)
            {
                if (allFields)
                    _expression.generalMembers = _expression.GetAllMembers().Select(x => new GeneralMember { EntityType = typeof(T), PocoColumn = x }).ToList();

                return _expression.ToUpdateStatement(item, excludeDefaults);
            }

            /// <summary>
            /// 生成 WHERE 条件语句。
            /// </summary>
            /// <returns>WHERE 条件 SQL。</returns>
            public string ToWhereStatement()
            {
                return _expression.ToWhereStatement();
            }

            /// <summary>
            /// 生成查询语句（应用分页、不去重）。
            /// </summary>
            /// <returns>查询 SQL。</returns>
            public virtual string ToSelectStatement()
            {
                return ToSelectStatement(true, false);
            }

            /// <summary>
            /// 生成查询语句。
            /// </summary>
            /// <param name="applyPaging">是否应用分页。</param>
            /// <param name="distinct">是否去重。</param>
            /// <returns>查询 SQL。</returns>
            public virtual string ToSelectStatement(bool applyPaging, bool distinct)
            {
                return _expression.ToSelectStatement(applyPaging, distinct);
            }
        }

        /// <summary>
        /// 设置查询字段。
        /// </summary>
        /// <typeparam name="TKey">字段返回类型。</typeparam>
        /// <param name="fields">字段选择器，如 x=&gt;x.SomeProperty1 或 x=&gt;new{ x.SomeProperty1, x.SomeProperty2 }。</param>
        /// <returns>当前表达式。</returns>
        public virtual ISqlExpression<T> Select<TKey>(Expression<Func<T, TKey>> fields)
        {
            sep = string.Empty;
            selectMembers.Clear();
            Visit(fields);
            return this;
        }

        /// <summary>
        /// 设置投影查询字段并返回选择成员集合。
        /// </summary>
        /// <typeparam name="TKey">字段返回类型。</typeparam>
        /// <param name="fields">字段选择器。</param>
        /// <returns>投影选择成员集合。</returns>
        public virtual List<SelectMember> SelectProjection<TKey>(Expression<Func<T, TKey>> fields)
        {
            sep = string.Empty;
            selectMembers.Clear();
            _projection = true;
            var exp = PartialEvaluator.Eval(fields, CanBeEvaluatedLocally);
            Visit(exp);
            _projection = false;
            var proj = selectMembers.Union(generalMembers.Select(x => new SelectMember() { EntityType = x.EntityType, PocoColumn = x.PocoColumn, PocoColumns = x.PocoColumns })).ToList();
            selectMembers.Clear();
            return proj;
        }

        /// <summary>
        /// 设置去重查询字段并返回选择成员集合。
        /// </summary>
        /// <typeparam name="TKey">字段返回类型。</typeparam>
        /// <param name="fields">字段选择器。</param>
        /// <returns>去重选择成员集合。</returns>
        public virtual List<SelectMember> SelectDistinct<TKey>(Expression<Func<T, TKey>> fields)
        {
            return SelectProjection(fields);
        }

        /// <summary>
        /// 添加 WHERE 条件 SQL。
        /// </summary>
        /// <param name="sqlFilter">条件 SQL。</param>
        /// <param name="filterParams">条件参数。</param>
        /// <returns>当前表达式。</returns>
        public virtual ISqlExpression<T> Where(string sqlFilter, params object[] filterParams)
        {
            if (string.IsNullOrEmpty(sqlFilter))
                return this;

            sqlFilter = ParameterHelper.ProcessParams(sqlFilter, filterParams, _params);

            appendSqlFilter("(" + sqlFilter + ")");

            return this;
        }

        private void appendSqlFilter(string sqlFilter)
        {
            if (string.IsNullOrEmpty(whereExpression))
            {
                whereExpression = "WHERE " + sqlFilter;
            }
            else
            {
                whereExpression += " AND " + sqlFilter;
            }
        }

        /// <summary>
        /// 生成关联（JOIN ON）条件 SQL。
        /// </summary>
        /// <typeparam name="T2">关联实体类型。</typeparam>
        /// <param name="predicate">关联条件。</param>
        /// <returns>ON 条件 SQL。</returns>
        public string On<T2>(Expression<Func<T, T2, bool>> predicate)
        {
            sep = " ";
            var onSql = Visit(predicate).ToString();
            return onSql;
        }

        /// <summary>
        /// 添加 WHERE 条件。
        /// </summary>
        /// <param name="predicate">条件表达式，为 null 时清空条件。</param>
        /// <returns>当前表达式。</returns>
        public virtual ISqlExpression<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                And(predicate);
            }
            else
            {
                whereExpression = string.Empty;
            }

            return this;
        }

        /// <summary>
        /// 以 AND 方式追加 WHERE 条件。
        /// </summary>
        /// <param name="predicate">条件表达式。</param>
        /// <returns>当前表达式。</returns>
        protected virtual ISqlExpression<T> And(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                ProcessInternalExpression(predicate);
            }
            return this;
        }

        private void ProcessInternalExpression(Expression<Func<T, bool>> predicate)
        {
            sep = " ";
            var exp = PartialEvaluator.Eval(predicate, CanBeEvaluatedLocally);
            var sqlFilter = Visit(exp).ToString();

            if (!string.IsNullOrEmpty(sqlFilter))
            {
                appendSqlFilter(sqlFilter);
            }
        }

        private bool CanBeEvaluatedLocally(Expression expression)
        {
            // any operation on a query can't be done locally
            ConstantExpression cex = expression as ConstantExpression;
            if (cex != null)
            {
                IQueryable query = cex.Value as IQueryable;
                if (query != null && query.Provider == this)
                    return false;
            }
            MethodCallExpression mc = expression as MethodCallExpression;
            if (mc != null &&
                (mc.Method.DeclaringType == typeof(Enumerable) ||
                 mc.Method.DeclaringType == typeof(Queryable)))
            {
                return false;
            }
            if (expression.NodeType == ExpressionType.Convert &&
                expression.Type == typeof(object))
                return true;
            return expression.NodeType != ExpressionType.Parameter &&
                   expression.NodeType != ExpressionType.Lambda;
        }

        /// <summary>
        /// 添加分组字段。
        /// </summary>
        /// <typeparam name="TKey">分组字段类型。</typeparam>
        /// <param name="keySelector">分组字段选择器。</param>
        /// <returns>当前表达式。</returns>
        public virtual ISqlExpression<T> GroupBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            sep = string.Empty;
            groupBy = Visit(keySelector).ToString();
            if (!string.IsNullOrEmpty(groupBy)) groupBy = string.Format("GROUP BY {0}", groupBy);
            return this;
        }

        /// <summary>
        /// 添加升序排序字段。
        /// </summary>
        /// <typeparam name="TKey">排序字段类型。</typeparam>
        /// <param name="keySelector">排序字段选择器。</param>
        /// <returns>当前表达式。</returns>
        public virtual ISqlExpression<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            sep = string.Empty;
            orderByMembers.Clear();
            generalMembers.Clear();
            var memberAccess = (MemberAccessString)Visit(keySelector);
            orderByMembers.Add(new OrderByMember { AscDesc = "ASC", PocoColumn = memberAccess.PocoColumn, EntityType = memberAccess.Type, PocoColumns = memberAccess.PocoColumns });
            generalMembers.Clear();
            BuildOrderByClauseInternal();
            return this;
        }

        /// <summary>
        /// 追加升序排序字段。
        /// </summary>
        /// <typeparam name="TKey">排序字段类型。</typeparam>
        /// <param name="keySelector">排序字段选择器。</param>
        /// <returns>当前表达式。</returns>
        public virtual ISqlExpression<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            sep = string.Empty;
            generalMembers.Clear();
            var memberAccess = (MemberAccessString)Visit(keySelector);
            orderByMembers.Add(new OrderByMember { AscDesc = "ASC", PocoColumn = memberAccess.PocoColumn, EntityType = memberAccess.Type, PocoColumns = memberAccess.PocoColumns });
            generalMembers.Clear();
            BuildOrderByClauseInternal();
            return this;
        }

        /// <summary>
        /// 添加降序排序字段。
        /// </summary>
        /// <typeparam name="TKey">排序字段类型。</typeparam>
        /// <param name="keySelector">排序字段选择器。</param>
        /// <returns>当前表达式。</returns>
        public virtual ISqlExpression<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            sep = string.Empty;
            orderByMembers.Clear();
            generalMembers.Clear();
            var memberAccess = (MemberAccessString)Visit(keySelector);
            orderByMembers.Add(new OrderByMember { AscDesc = "DESC", PocoColumn = memberAccess.PocoColumn, EntityType = memberAccess.Type, PocoColumns = memberAccess.PocoColumns });
            generalMembers.Clear();
            BuildOrderByClauseInternal();
            return this;
        }

        /// <summary>
        /// 追加降序排序字段。
        /// </summary>
        /// <typeparam name="TKey">排序字段类型。</typeparam>
        /// <param name="keySelector">排序字段选择器。</param>
        /// <returns>当前表达式。</returns>
        public virtual ISqlExpression<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            sep = string.Empty;
            generalMembers.Clear();
            var memberAccess = (MemberAccessString)Visit(keySelector);
            orderByMembers.Add(new OrderByMember { AscDesc = "DESC", PocoColumn = memberAccess.PocoColumn, EntityType = memberAccess.Type, PocoColumns = memberAccess.PocoColumns });
            generalMembers.Clear();
            BuildOrderByClauseInternal();
            return this;
        }

        private void BuildOrderByClauseInternal()
        {
            if (orderByMembers.Count > 0)
            {
                orderBy = "ORDER BY " + string.Join(", ", orderByMembers.Select(x => (PrefixFieldWithTableName ? _databaseType.EscapeSqlIdentifier(x.PocoColumns.Last().MemberInfoKey) : _databaseType.EscapeSqlIdentifier(x.PocoColumns.Last().MemberInfoKey)) + " " + x.AscDesc).ToArray());
            }
            else
            {
                orderBy = null;
            }
        }

        /// <summary>
        /// 添加表提示。
        /// </summary>
        /// <param name="hint">表提示内容。</param>
        public virtual void Hint(string hint)
        {
            tableHint += " " + hint;
        }

        /// <summary>
        /// 设置 LIMIT 子句的返回行数与偏移量。
        /// </summary>
        /// <param name="rows">返回行数。</param>
        /// <param name="skip">跳过的行数（首行偏移量为 0）。</param>
        /// <returns>当前表达式。</returns>
        public virtual ISqlExpression<T> Limit(int rows, int skip)
        {
            Rows = rows;
            Skip = skip;
            return this;
        }

        /// <summary>
        /// Set the specified rows for Sql Limit clause.
        /// </summary>
        /// <param name='rows'>
        /// Number of rows returned by a SELECT statement
        /// </param>
        public virtual ISqlExpression<T> Limit(int rows)
        {
            Rows = rows;
            Skip = 0;
            return this;
        }

        /// <summary>
        /// Fields to be updated.
        /// </summary>
        /// <param name='fields'>
        /// x=> x.SomeProperty1 or x=> new{ x.SomeProperty1, x.SomeProperty2}
        /// </param>
        /// <typeparam name='TKey'>
        /// objectWithProperties
        /// </typeparam>
        public virtual ISqlExpression<T> Update<TKey>(Expression<Func<T, TKey>> fields)
        {
            sep = string.Empty;
            generalMembers.Clear();
            Visit(fields);
            Context.UpdateFields = new List<string>(generalMembers.Select(x => x.PocoColumn.MemberInfoData.Name));
            generalMembers.Clear();
            return this;
        }

        /// <summary>
        /// 生成删除语句。
        /// </summary>
        /// <returns>删除 SQL。</returns>
        protected virtual string ToDeleteStatement()
        {
            return string.Format("DELETE {0} FROM {1} {2}",
                (PrefixFieldWithTableName ? _databaseType.EscapeTableName(_pocoData.TableInfo.AutoAlias) : string.Empty),
                _databaseType.EscapeTableName(_pocoData.TableInfo.TableName) + (PrefixFieldWithTableName ? " " + _databaseType.EscapeTableName(_pocoData.TableInfo.AutoAlias) : string.Empty),
                WhereExpression);
        }

        /// <summary>
        /// 生成更新语句。
        /// </summary>
        /// <param name="item">待更新的实体。</param>
        /// <param name="excludeDefaults">是否排除默认值字段。</param>
        /// <returns>更新 SQL。</returns>
        protected virtual string ToUpdateStatement(T item, bool excludeDefaults)
        {
            var setFields = new StringBuilder();
            var primaryKeys = _pocoData.TableInfo.PrimaryKey.Split(',');

            foreach (var fieldDef in _pocoData.Columns)
            {
                if (_pocoData.TableInfo.AutoIncrement && primaryKeys.Contains(fieldDef.Value.ColumnName, StringComparer.OrdinalIgnoreCase))
                    continue;

                if (Context.UpdateFields.Count > 0 && !Context.UpdateFields.Contains(fieldDef.Value.MemberInfoData.Name)) continue; // added

                object value = fieldDef.Value.GetColumnValue(_pocoData, item, (pocoColumn, val) => _database.ProcessMapper(pocoColumn, val));

                if (excludeDefaults && (value == null || value.Equals(MappingHelper.GetDefault(value.GetType())))) continue; //GetDefaultValue?

                if (setFields.Length > 0)
                    setFields.Append(", ");

                setFields.AppendFormat("{0} = {1}", (PrefixFieldWithTableName ? _databaseType.EscapeTableName(_pocoData.TableInfo.AutoAlias) + "." : string.Empty) + _databaseType.EscapeSqlIdentifier(fieldDef.Value.ColumnName), CreateParam(value));
            }

            if (PrefixFieldWithTableName)
                return string.Format("UPDATE {0} SET {2} FROM {1} {3}", _databaseType.EscapeTableName(_pocoData.TableInfo.AutoAlias), _databaseType.EscapeTableName(_pocoData.TableInfo.TableName) + " " + _databaseType.EscapeTableName(_pocoData.TableInfo.AutoAlias), setFields, WhereExpression);
            else
                return string.Format("UPDATE {0} SET {1} {2}", _databaseType.EscapeTableName(_pocoData.TableInfo.TableName), setFields, WhereExpression);
        }

        /// <summary>
        /// 生成 WHERE 条件语句。
        /// </summary>
        /// <returns>WHERE 条件 SQL。</returns>
        protected string ToWhereStatement()
        {
            return WhereExpression;
        }

        /// <summary>
        /// 生成查询语句。
        /// </summary>
        /// <param name="applyPaging">是否应用分页。</param>
        /// <param name="isDistinct">是否去重。</param>
        /// <returns>查询 SQL。</returns>
        protected virtual string ToSelectStatement(bool applyPaging, bool isDistinct)
        {
            var sql = new StringBuilder();

            sql.Append(GetSelectExpression(isDistinct));
            sql.Append(string.IsNullOrEmpty(WhereExpression) ?
                       "" :
                       " \n" + WhereExpression);
            sql.Append(string.IsNullOrEmpty(GroupByExpression) ?
                       "" :
                       " \n" + GroupByExpression);
            sql.Append(string.IsNullOrEmpty(OrderByExpression) ?
                       "" :
                       " \n" + OrderByExpression);

            return applyPaging ? ApplyPaging(sql.ToString(), ModelDef.QueryColumns.Select(x => new[] { x.Value }), new Dictionary<string, JoinData>()) : sql.ToString();
        }

        private string GetSelectExpression(bool distinct)
        {
            // 未显式选择列时，直接返回默认列集合，无需构建 orderBy 去重
            if (selectMembers.Count == 0)
                return BuildSelectExpression(null, distinct);

            // 提前构建已选列的 (实体类型, 成员名) 集合，将去重判断由 O(n*m) 嵌套扫描降为 O(1)
            var selectedKeys = new HashSet<(Type, string)>(selectMembers.Count);
            foreach (var member in selectMembers)
                selectedKeys.Add((member.EntityType, member.PocoColumn.MemberInfoData.Name));

            var cols = new List<SelectMember>(selectMembers.Count + orderByMembers.Count);
            cols.AddRange(selectMembers);
            foreach (var x in orderByMembers)
            {
                if (!selectedKeys.Contains((x.EntityType, x.PocoColumn.MemberInfoData.Name)))
                {
                    cols.Add(new SelectMember() { PocoColumn = x.PocoColumn, EntityType = x.EntityType, PocoColumns = new[] { x.PocoColumn } });
                }
            }
            return BuildSelectExpression(cols, distinct);
        }

        private string WhereExpression
        {
            get
            {
                return whereExpression;
            }
            set
            {
                whereExpression = value;
            }
        }

        private string GroupByExpression
        {
            get
            {
                return groupBy;
            }
            set
            {
                groupBy = value;
            }
        }

        private string OrderByExpression
        {
            get
            {
                return orderBy;
            }
            set
            {
                orderBy = value;
            }
        }

        private int? Rows { get; set; }
        private int? Skip { get; set; }

        /// <summary>
        /// 当前实体的 POCO 元数据定义。
        /// </summary>
        protected internal PocoData ModelDef
        {
            get
            {
                return _pocoData;
            }
            set
            {
                _pocoData = value;
            }
        }

        /// <summary>
        /// 访问表达式节点，根据节点类型分派到对应的访问方法。
        /// </summary>
        /// <param name="exp">待访问的表达式。</param>
        /// <returns>访问结果（可能是 SQL 片段、参数值或成员信息）。</returns>
        protected internal virtual object Visit(Expression exp)
        {

            if (exp == null) return string.Empty;
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return VisitLambda(exp as LambdaExpression);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess(exp as MemberExpression);
                case ExpressionType.Constant:
                    return VisitConstant(exp as ConstantExpression);
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
                case ExpressionType.ExclusiveOr:
                    //return "(" + VisitBinary(exp as BinaryExpression) + ")";
                    return VisitBinary(exp as BinaryExpression);
                case ExpressionType.Conditional:
                    return VisitConditional(exp as ConditionalExpression);
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary(exp as UnaryExpression);
                case ExpressionType.Parameter:
                    return VisitParameter(exp as ParameterExpression);
                case ExpressionType.Call:
                    return VisitMethodCall(exp as MethodCallExpression);
                case ExpressionType.New:
                    return VisitNew(exp as NewExpression);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray(exp as NewArrayExpression);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                default:
                    return exp.ToString();
            }
        }
        /// <summary>
        /// 访问成员初始化表达式。
        /// </summary>
        /// <param name="init">待访问的成员初始化表达式。</param>
        /// <returns>访问后的表达式。</returns>
        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression n = init.NewExpression;
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            if (n != init.NewExpression || bindings != init.Bindings)
            {
                return Expression.MemberInit(n, bindings);
            }
            return init;
        }

        /// <summary>
        /// 访问成员绑定列表。
        /// </summary>
        /// <param name="original">待访问的成员绑定列表。</param>
        /// <returns>访问后的成员绑定列表。</returns>
        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            for (int i = 0, n = original.Count; i < n; i++)
            {
                this.VisitBinding(original[i]);
            }
            return original;
        }

        /// <summary>
        /// 访问成员绑定。
        /// </summary>
        /// <param name="binding">待访问的成员绑定。</param>
        /// <returns>访问结果。</returns>
        protected virtual object VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                //case MemberBindingType.ListBinding:
                //    return this.VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        /// <summary>
        /// 访问嵌套的成员成员绑定。
        /// </summary>
        /// <param name="binding">待访问的成员成员绑定。</param>
        /// <returns>访问结果。</returns>
        protected virtual object VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            return VisitBindingList(binding.Bindings);
        }

        /// <summary>
        /// 访问成员赋值。
        /// </summary>
        /// <param name="assignment">待访问的成员赋值。</param>
        /// <returns>访问结果。</returns>
        protected virtual object VisitMemberAssignment(MemberAssignment assignment)
        {
            return this.Visit(assignment.Expression);
        }

        /// <summary>
        /// 访问 Lambda 表达式。
        /// </summary>
        /// <param name="lambda">待访问的 Lambda 表达式。</param>
        /// <returns>访问结果。</returns>
        protected virtual object VisitLambda(LambdaExpression lambda)
        {
            if (lambda.Body.NodeType == ExpressionType.MemberAccess && sep == " ")
            {
                MemberExpression m = lambda.Body as MemberExpression;

                if (m.Expression != null)
                {
                    if (IsNullableMember(m))
                    {
                        string r = VisitMemberAccess(m.Expression as MemberExpression).ToString();
                        return string.Format("{0} is not null", r);
                    }
                    else
                    {
                        var o = VisitMemberAccess(m);
                        if (o is MemberAccessString memberAccessString)
                        {
                            return string.Format("{0}={1}", o,
                                _database.TryGetMapper(memberAccessString.PocoColumn, out var converter)
                                    ? CreateParam(converter(true))
                                    : GetQuotedTrueValue());
                        }
                        return string.Format("{0}={1}", o, GetQuotedTrueValue());
                    }
                }
            }
            else if (lambda.Body.NodeType == ExpressionType.Constant)
            {
                var result = Visit(lambda.Body);
                if (result is bool)
                {
                    return ((bool)result) ? "1=1" : "1<>1";
                }
            }
            return Visit(lambda.Body);
        }

        private static bool IsNullableMember(MemberExpression m)
        {
            var member = m.Expression as MemberExpression;
            return member != null
                && member.Type.GetTypeInfo().IsGenericType && member.Type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 访问二元表达式。
        /// </summary>
        /// <param name="b">待访问的二元表达式。</param>
        /// <returns>访问结果（通常为 PartialSqlString）。</returns>
        protected virtual object VisitBinary(BinaryExpression b)
        {
            // Fix VB and CompareString
            b = FixExpressionForVb(b);

            object left, right;
            bool switchLeftRight = false;
            var operand = BindOperant(b.NodeType);   //sep= " " ??

            if (b.NodeType == ExpressionType.AndAlso || b.NodeType == ExpressionType.OrElse)
            {
                var m = b.Left as MemberExpression;
                if (m?.Expression != null
                    && m.Expression.NodeType == ExpressionType.Parameter)
                    left = new PartialSqlString($"{VisitMemberAccess(m)} = {GetQuotedTrueValue()}");
                else
                    left = Visit(b.Left);

                if (left is NullableMemberAccess)
                {
                    left = new PartialSqlString("(" + left + " is not null)");
                }

                m = b.Right as MemberExpression;
                if (m?.Expression != null
                    && m.Expression.NodeType == ExpressionType.Parameter)
                    right = new PartialSqlString($"{VisitMemberAccess(m)} = {GetQuotedTrueValue()}");
                else
                    right = Visit(b.Right);

                if (right is NullableMemberAccess)
                {
                    right = new PartialSqlString("(" + right + " is not null)");
                }

                if (left is not PartialSqlString && right is not PartialSqlString)
                {
                    var result = Expression.Lambda(b).Compile().DynamicInvoke();
                    return new PartialSqlString(CreateParam(result));
                }

                if (left is not PartialSqlString)
                    left = ((bool)left) ? GetTrueExpression() : GetFalseExpression();
                if (right is not PartialSqlString)
                    right = ((bool)right) ? GetTrueExpression() : GetFalseExpression();
            }
            else
            {
                left = Visit(b.Left);
                right = Visit(b.Right);

                var isLeftMemberAccessString = left.CanBeCastTo<MemberAccessString>(out var leftMemberAccessString);
                var isRightMemberAccessString = right.CanBeCastTo<MemberAccessString>(out var rightMemberAccessString);

                // Enums
                if (left.CanBeCastTo<EnumMemberAccess>(out var leftEnumMemberAccess) && !(right is PartialSqlString))
                {
                    var pc = leftEnumMemberAccess.PocoColumn;
                    if (pc.ColumnType == typeof(string))
                        right = CreateParam(Enum.Parse(GetMemberInfoTypeForEnum(pc), right.ToString()).ToString());
                    else if (Int64.TryParse(right.ToString(), out long numvericVal))
                        right = CreateParam(Enum.ToObject(GetMemberInfoTypeForEnum(pc), numvericVal));
                    else
                        right = CreateParam(right);
                }
                else if (right.CanBeCastTo<EnumMemberAccess>(out var rightEnumMemberAccess) && !(left is PartialSqlString))
                {
                    var pc = rightEnumMemberAccess.PocoColumn;
                    if (pc.ColumnType == typeof(string))
                        left = CreateParam(Enum.Parse(GetMemberInfoTypeForEnum(pc), left.ToString()).ToString());
                    else if (Int64.TryParse(left.ToString(), out long numvericVal))
                        left = CreateParam(Enum.ToObject(GetMemberInfoTypeForEnum(pc), numvericVal));
                    else
                        left = CreateParam(left);
                }
                // Nullable Members
                else if (left is NullableMemberAccess && right is not PartialSqlString)
                {
                    operand = ((bool)right) ? "is not" : "is";
                    right = new PartialSqlString("null");
                }
                else if (right is NullableMemberAccess && left is not PartialSqlString)
                {
                    operand = ((bool)left) ? "is not" : "is";
                    left = new PartialSqlString("null");
                    switchLeftRight = true;
                }
                // Chars
                else if (isLeftMemberAccessString && right is int
                    && new[] { typeof(char), typeof(char?) }.Contains(leftMemberAccessString.PocoColumn.MemberInfoData.MemberType))
                {
                    right = CreateParam(Convert.ToChar(right));
                }
                else if (isRightMemberAccessString && left is int
                         && new[] { typeof(char), typeof(char?) }.Contains(rightMemberAccessString.PocoColumn.MemberInfoData.MemberType))
                {
                    left = CreateParam(Convert.ToChar(left));
                }
                // AnsiString
                else if (isLeftMemberAccessString && right is string && leftMemberAccessString.PocoColumn.ColumnType == typeof(AnsiString))
                {
                    right = CreateParam(new AnsiString((string)right));
                }
                else if (isRightMemberAccessString && left is string && rightMemberAccessString.PocoColumn.ColumnType == typeof(AnsiString))
                {
                    left = CreateParam(new AnsiString((string)left));
                }
                // ValueObject
                else if (isLeftMemberAccessString && leftMemberAccessString.PocoColumn.ValueObjectColumn)
                {
                    right = CreateParam(leftMemberAccessString.PocoColumn.GetValueObjectValue(right));
                }
                else if (isRightMemberAccessString && rightMemberAccessString.PocoColumn.ValueObjectColumn)
                {
                    left = CreateParam(rightMemberAccessString.PocoColumn.GetValueObjectValue(left));
                }
                // Mappers
                else if (isLeftMemberAccessString && right is not PartialSqlString && _database.TryGetMapper(leftMemberAccessString.PocoColumn, out var converterRight))
                {
                    right = CreateParam(converterRight(right));
                }
                else if (isRightMemberAccessString && left is not PartialSqlString && _database.TryGetMapper(rightMemberAccessString.PocoColumn, out var converterLeft))
                {
                    left = CreateParam(converterLeft(left));
                }
                else if (left is not PartialSqlString && right is not PartialSqlString)
                {
                    var result = Expression.Lambda(b).Compile().DynamicInvoke();
                    return result;
                }
                else if (left is not PartialSqlString)
                    left = CreateParam(left);
                else if (right is not PartialSqlString)
                    right = CreateParam(right);

            }

            if (operand == "=" && right.ToString().Equals("null", StringComparison.OrdinalIgnoreCase)) { operand = "is"; }
            else if (operand == "=" && left.ToString().Equals("null", StringComparison.OrdinalIgnoreCase)) { operand = "is"; switchLeftRight = true; }
            else if (operand == "<>" && right.ToString().Equals("null", StringComparison.OrdinalIgnoreCase)) { operand = "is not"; }
            else if (operand == "<>" && left.ToString().Equals("null", StringComparison.OrdinalIgnoreCase)) { operand = "is not"; switchLeftRight = true; }

            // Switch left and right for situtations like is null
            if (switchLeftRight)
            {
                var saveleft = left;
                left = right;
                right = saveleft;
            }

            switch (operand)
            {
                case "MOD":
                case "COALESCE":
                    return new PartialSqlString($"{operand}({left},{right})");
                default:
                    return new PartialSqlString("(" + left + sep + operand + sep + right + ")");
            }
        }

        private static BinaryExpression FixExpressionForVb(BinaryExpression b)
        {
            if (b.Left is MethodCallExpression)
            {
                var method = (MethodCallExpression)b.Left;
                if (method.Method.Name == "CompareString"
                    && method.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Operators")
                {
                    var left = method.Arguments[0];
                    var right = method.Arguments[1];

                    return b.NodeType == ExpressionType.Equal ? Expression.Equal(left, right) : Expression.NotEqual(left, right);
                }
            }
            return b;
        }

        private static Type GetMemberInfoTypeForEnum(PocoColumn pc)
        {
            if (pc.MemberInfoData.MemberType.GetTypeInfo().IsEnum)
                return pc.MemberInfoData.MemberType;

            return Nullable.GetUnderlyingType(pc.MemberInfoData.MemberType);
        }

        /// <summary>
        /// 访问成员访问表达式，将其转换为对应的列访问信息。
        /// </summary>
        /// <param name="m">待访问的成员访问表达式。</param>
        /// <returns>成员访问结果。</returns>
        protected virtual object VisitMemberAccess(MemberExpression m)
        {
            bool isNull = false;

            if (IsNullableMember(m))
            {
                if (m.Member.Name == "HasValue")
                {
                    isNull = true;
                }
                m = m.Expression as MemberExpression;
            }

            if (m.Member.DeclaringType == typeof(DateTime) || m.Member.DeclaringType == typeof(DateTime?))
            {
                if (m.Expression is MemberExpression m1)
                {
                    var p = Expression.Convert(m1, typeof(object));
                    if (p.NodeType == ExpressionType.Convert)
                    {
                        var pp = m1.Expression as ParameterExpression;
                        if (pp == null)
                        {
                            m1 = m1.Expression as MemberExpression;
                            if (m1 != null)
                            {
                                pp = m1.Expression as ParameterExpression;
                            }
                        }
                        if (pp != null)
                        {
                            if (m.Member.Name == "TableName")
                                return Visit(m1);
                            return new PartialSqlString(GetDateTimeSql(m.Member.Name, Visit(m1)));
                        }
                    }
                }
            }

            if (m.Expression != null
                && (m.Expression.NodeType == ExpressionType.Parameter
                    || m.Expression.NodeType == ExpressionType.Convert
                    || m.Expression.NodeType == ExpressionType.MemberAccess))
            {
                var propertyInfos = MemberChainHelper.GetMembers(m).ToArray();
                var type = GetCorrectType(m);

                var pocoMembers = ModelDef.GetAllMembers()
                    .Where(x => x.MemberInfoChain.Select(y => y.Name).SequenceEqual(propertyInfos.Select(y => y.Name)))
                    .ToArray();

                var pocoMember = pocoMembers.LastOrDefault();
                if (pocoMember == null)
                {
                    throw new Exception(
                        string.Format("Did you forget to include the property eg. Include(x => x.{0})",
                        string.Join(".", propertyInfos.Select(y => y.Name).Take(propertyInfos.Length - 1).ToArray())));
                }

                if (_projection &&
                    (pocoMember.ReferenceType == ReferenceType.Foreign
                    || pocoMember.ReferenceType == ReferenceType.OneToOne)
                    || pocoMember.PocoColumn == null)
                {
                    foreach (var member in pocoMember.PocoMemberChildren.Where(x => x.PocoColumn != null))
                    {
                        generalMembers.Add(new GeneralMember()
                        {
                            EntityType = pocoMember.MemberInfoData.MemberType,
                            PocoColumn = member.PocoColumn,
                            PocoColumns = new[] { member.PocoColumn }
                        });
                    }

                    return new PartialSqlString("");
                }

                var pocoColumn = pocoMember.PocoColumn;
                var pocoColumns = pocoMembers.Select(x => x.PocoColumn).ToArray();

                var columnName = (PrefixFieldWithTableName
                                          ? _databaseType.EscapeTableName(pocoColumn.TableInfo.AutoAlias) + "."
                                          : "")
                                     + _databaseType.EscapeSqlIdentifier(pocoColumn.ColumnName);

                generalMembers.Add(new GeneralMember()
                {
                    EntityType = type,
                    PocoColumn = pocoColumn,
                    PocoColumns = pocoColumns
                });

                if (isNull)
                    return new NullableMemberAccess(pocoColumn, pocoColumns, columnName, type);

                if (Database.IsEnum(pocoColumn.MemberInfoData))
                    return new EnumMemberAccess(pocoColumn, pocoColumns, columnName, type);

                return new MemberAccessString(pocoColumn, pocoColumns, columnName, type);
            }

            var memberExp = Expression.Convert(m, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(memberExp);
            var getter = lambda.Compile();
            return getter();
        }

        private Type GetCorrectType(MemberExpression m)
        {
            var type = m.Member.DeclaringType;
            if (m.Expression.NodeType == ExpressionType.MemberAccess)
            {
                type = ((PropertyInfo)((MemberExpression)m.Expression).Member).PropertyType;
            }
            else if (m.Expression.NodeType == ExpressionType.Parameter)
            {
                type = m.Expression.Type;
            }
            return type;
        }

        /// <summary>
        /// 访问 new 表达式。
        /// </summary>
        /// <param name="nex">待访问的 new 表达式。</param>
        /// <returns>访问结果。</returns>
        protected virtual object VisitNew(NewExpression nex)
        {
            var member = Expression.Convert(nex, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(member);
            try
            {
                var getter = lambda.Compile();
                return getter();
            }
            catch (System.InvalidOperationException)
            {
                List<PartialSqlString> exprs = VisitExpressionList(nex.Arguments).OfType<PartialSqlString>().ToList();
                StringBuilder r = new StringBuilder();
                for (int i = 0; i < exprs.Count; i++)
                {
                    if (exprs[i] is MemberAccessString)
                    {
                        selectMembers.Add(new SelectMember()
                        {
                            EntityType = ((MemberAccessString)exprs[i]).Type,
                            PocoColumn = ((MemberAccessString)exprs[i]).PocoColumn,
                            PocoColumns = ((MemberAccessString)exprs[i]).PocoColumns,
                        });
                    }
                }
                return r.ToString();
            }

        }

        /// <summary>
        /// 访问参数表达式。
        /// </summary>
        /// <param name="p">待访问的参数表达式。</param>
        /// <returns>参数名。</returns>
        protected virtual object VisitParameter(ParameterExpression p)
        {
            return p.Name;
        }

        List<object> _params = new List<object>();

        string paramPrefix;
        private bool _projection;
        /// <summary>
        /// 表达式上下文。
        /// </summary>
        public ISqlExpression<T>.ISqlExpressionContext Context { get; private set; }

        /// <summary>
        /// 访问常量表达式。
        /// </summary>
        /// <param name="c">待访问的常量表达式。</param>
        /// <returns>常量值，null 时返回 "null" 片段。</returns>
        protected virtual object VisitConstant(ConstantExpression c)
        {
            if (c.Value == null)
                return new PartialSqlString("null");

            return c.Value;
        }

        /// <summary>
        /// 访问条件表达式。
        /// </summary>
        /// <param name="conditional">待访问的条件表达式。</param>
        /// <returns>CASE WHEN 条件片段。</returns>
        protected virtual object VisitConditional(ConditionalExpression conditional)
        {
            sep = " ";
            var test = Visit(conditional.Test);
            var trueSql = Visit(conditional.IfTrue);
            var falseSql = Visit(conditional.IfFalse);

            return new PartialSqlString(string.Format("(case when {0} then {1} else {2} end)", test, trueSql, falseSql));
        }

        /// <summary>
        /// 创建参数占位符并记录参数值。
        /// </summary>
        /// <param name="value">参数值。</param>
        /// <returns>参数占位符。</returns>
        protected string CreateParam(object value)
        {
            string paramPlaceholder = paramPrefix + _params.Count;
            _params.Add(value);
            return paramPlaceholder;
        }

        /// <summary>
        /// 访问一元表达式。
        /// </summary>
        /// <param name="u">待访问的一元表达式。</param>
        /// <returns>访问结果。</returns>
        protected virtual object VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    var o = Visit(u.Operand);

                    if (o as PartialSqlString == null)
                        return !((bool)o);

                    if (o is MemberAccessString memberAccessString)
                    {
                        if (o as NullableMemberAccess != null)
                            o = o + " is not null";
                        else
                            o = o + " = " + (
                                _database.TryGetMapper(memberAccessString.PocoColumn, out var converter)
                                    ? CreateParam(converter(true))
                                    : GetQuotedTrueValue());
                    }

                    return new PartialSqlString("NOT (" + o + ")");
                case ExpressionType.Convert:
                    if (u.Method != null)
                        return Expression.Lambda(u).Compile().DynamicInvoke();
                    break;
            }

            return Visit(u.Operand);

        }

        private bool IsColumnAccess(MethodCallExpression m)
        {
            if (m.Object != null && m.Object as MethodCallExpression != null)
                return IsColumnAccess(m.Object as MethodCallExpression);

            var exp = m.Object as MemberExpression;
            return exp != null
                && exp.Expression != null
                && ((exp.Expression.Type == typeof(T) && exp.Expression.NodeType == ExpressionType.Parameter
                    || exp.Expression.NodeType == ExpressionType.MemberAccess));
        }

        /// <summary>
        /// 访问方法调用表达式。
        /// </summary>
        /// <param name="m">待访问的方法调用表达式。</param>
        /// <returns>访问结果。</returns>
        protected virtual object VisitMethodCall(MethodCallExpression m)
        {
            if (IsStaticArrayMethod(m))
                return VisitStaticArrayMethodCall(m);

            if (IsEnumerableMethod(m))
                return VisitEnumerableMethodCall(m);

            if (IsColumnAccess(m))
                return VisitColumnAccessMethod(m);

            if (_projection && VisitInnerMethodCall(m))
                return null;

            // Handle conversion operators (op_Implicit, op_Explicit) which cannot be dynamically invoked
            // These typically wrap constant values, so we visit the operand instead
            if (m.Method.IsSpecialName && 
                (m.Method.Name == "op_Implicit" || m.Method.Name == "op_Explicit") && 
                m.Arguments.Count == 1)
            {
                return Visit(m.Arguments[0]);
            }

            return Expression.Lambda(m).Compile().DynamicInvoke();
        }

        private bool VisitInnerMethodCall(MethodCallExpression m)
        {
            bool found = false;
            if (m.Arguments.Any(args => ProcessMethodSearchRecursively(args, ref found)))
            {
                return true;
            }
            return found;
        }

        private bool ProcessMethodSearchRecursively(Expression args, ref bool found)
        {
            if (args.NodeType == ExpressionType.Parameter && args.Type == typeof(T))
            {
                selectMembers.AddRange(_pocoData.QueryColumns.Select(x => new SelectMember { PocoColumn = x.Value, EntityType = _pocoData.Type, PocoColumns = new[] { x.Value } }));
                return true;
            }

            IEnumerable<Expression> nestedExpressions = null;
            var nested1 = args as MethodCallExpression;
            if (nested1 != null)
            {
                nestedExpressions = nested1.Arguments;
            }
            else
            {
                var nested2 = args as NewArrayExpression;
                if (nested2 != null) nestedExpressions = nested2.Expressions;
            }

            if (nestedExpressions != null)
            {
                foreach (var nestedExpression in nestedExpressions)
                {
                    if (ProcessMethodSearchRecursively(nestedExpression, ref found))
                        return true;
                }
            }

            var result = Visit(args) as MemberAccessString;
            found = found || result != null;

            return false;
        }

        private bool IsStaticArrayMethod(MethodCallExpression m)
        {
            if (m.Object == null && m.Method.Name == "Contains")
            {
                return m.Arguments.Count == 2;
            }

            return false;
        }

        private bool IsEnumerableMethod(MethodCallExpression m)
        {
            if (m.Object != null
                && m.Object.Type.IsOrHasGenericInterfaceTypeOf(typeof(IEnumerable<>))
                && m.Object.Type != typeof(string)
                && m.Method.Name == "Contains")
            {
                return m.Arguments.Count == 1;
            }

            return false;
        }

        /// <summary>
        /// 访问 IEnumerable 扩展方法调用（如 Contains）。
        /// </summary>
        /// <param name="m">待访问的方法调用表达式。</param>
        /// <returns>访问结果。</returns>
        protected virtual object VisitEnumerableMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Contains":
                    List<Object> args = this.VisitExpressionList(m.Arguments);
                    return new PartialSqlString(BuildInStatement(m.Object, args[0]));

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 访问静态数组方法调用（如 Contains）。
        /// </summary>
        /// <param name="m">待访问的方法调用表达式。</param>
        /// <returns>访问结果。</returns>
        protected virtual object VisitStaticArrayMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Contains":
                    List<Object> args = this.VisitExpressionList(m.Arguments);
                    Expression memberExpr = m.Arguments[0];
                    if (memberExpr.NodeType == ExpressionType.MemberAccess)
                        memberExpr = (m.Arguments[0] as MemberExpression);

                    // If args[0] is already an evaluated value (not a PartialSqlString), use it directly
                    if (!(args[0] is PartialSqlString) && args[0] is IEnumerable enumerable)
                    {
                        var inArgs = enumerable.Cast<object>().ToList();
                        if (inArgs.Count == 0)
                        {
                            return new PartialSqlString("1 = 0");
                        }
                        var sIn = FlattenList(inArgs, args[1]);
                        return new PartialSqlString(string.Format("{0} {1} ({2})", args[1], "IN", sIn));
                    }

                    return new PartialSqlString(BuildInStatement(memberExpr, args[1]));

                default:
                    throw new NotSupportedException();
            }
        }

        private StringBuilder FlattenList(List<object> inArgs, object partialSqlString)
        {
            var sIn = new StringBuilder();
            foreach (object e in inArgs)
            {
                if (!typeof(ICollection).IsAssignableFrom(e.GetType()))
                {
                    var v = FormatParameters(partialSqlString, e);
                    sIn.AppendFormat("{0}{1}", sIn.Length > 0 ? "," : "", CreateParam(v));
                }
                else
                {
                    foreach (object el in (ICollection)e)
                    {
                        var v = FormatParameters(partialSqlString, el);
                        sIn.AppendFormat("{0}{1}", sIn.Length > 0 ? "," : "", CreateParam(v));
                    }
                }
            }

            return sIn;
        }

        private object FormatParameters(object partialSqlString, object e)
        {
            switch (partialSqlString)
            {
                case EnumMemberAccess ema when ema.PocoColumn.ColumnType == typeof(string):
                    e = e.ToString();
                    break;
                case MemberAccessString mas:
                    if (_database.TryGetMapper(mas.PocoColumn, out var converter)) e = converter(e);
                    break;
            }

            return e;
        }

        /// <summary>
        /// 访问表达式列表。
        /// </summary>
        /// <param name="original">待访问的表达式列表。</param>
        /// <returns>访问结果列表。</returns>
        protected virtual List<Object> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Object> list = new List<Object>();
            for (int i = 0, n = original.Count; i < n; i++)
            {
                if (original[i].NodeType == ExpressionType.NewArrayInit ||
                 original[i].NodeType == ExpressionType.NewArrayBounds)
                {

                    list.AddRange(VisitNewArrayFromExpressionList(original[i] as NewArrayExpression));
                }
                else
                    list.Add(Visit(original[i]));

            }
            return list;
        }

        /// <summary>
        /// 访问数组创建表达式。
        /// </summary>
        /// <param name="na">待访问的数组创建表达式。</param>
        /// <returns>访问结果。</returns>
        protected virtual object VisitNewArray(NewArrayExpression na)
        {

            List<Object> exprs = VisitExpressionList(na.Expressions);
            StringBuilder r = new StringBuilder();
            foreach (Object e in exprs)
            {
                r.Append(r.Length > 0 ? "," + e : e);
            }

            return r.ToString();
        }

        /// <summary>
        /// 从表达式列表中访问数组创建表达式。
        /// </summary>
        /// <param name="na">待访问的数组创建表达式。</param>
        /// <returns>访问结果列表。</returns>
        protected virtual List<Object> VisitNewArrayFromExpressionList(NewArrayExpression na)
        {

            List<Object> exprs = VisitExpressionList(na.Expressions);
            return exprs;
        }


        /// <summary>
        /// 将表达式节点类型映射为对应的 SQL 运算符。
        /// </summary>
        /// <param name="e">表达式节点类型。</param>
        /// <returns>SQL 运算符字符串。</returns>
        protected virtual string BindOperant(ExpressionType e)
        {
            switch (e)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "MOD";
                case ExpressionType.Coalesce:
                    return "COALESCE";
                case ExpressionType.And:
                    return "&";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.ExclusiveOr:
                    return "^";
                case ExpressionType.Not:
                    return "~";
                default:
                    return e.ToString();
            }
        }

        /// <summary>
        /// 移除别名首尾的引号。
        /// </summary>
        /// <param name="exp">待处理的别名。</param>
        /// <returns>移除首尾引号后的别名。</returns>
        protected string RemoveQuoteFromAlias(string exp)
        {

            if ((exp.StartsWith("\"") || exp.StartsWith("`") || exp.StartsWith("'"))
                && (exp.EndsWith("\"") || exp.EndsWith("`") || exp.EndsWith("'")))
            {
                exp = exp.Remove(0, 1);
                exp = exp.Remove(exp.Length - 1, 1);
            }
            return exp;
        }

        /// <summary>
        /// 生成恒真的 SQL 表达式片段。
        /// </summary>
        /// <returns>真值表达式片段。</returns>
        protected object GetTrueExpression()
        {
            return new PartialSqlString(string.Format("({0}={1})", GetQuotedTrueValue(), GetQuotedTrueValue()));
        }

        /// <summary>
        /// 生成恒假的 SQL 表达式片段。
        /// </summary>
        /// <returns>假值表达式片段。</returns>
        protected object GetFalseExpression()
        {
            return new PartialSqlString(string.Format("({0}={1})", GetQuotedTrueValue(), GetQuotedFalseValue()));
        }

        /// <summary>
        /// 创建布尔 true 参数并返回其占位符。
        /// </summary>
        /// <returns>参数占位符。</returns>
        protected object GetQuotedTrueValue()
        {
            return CreateParam(true);
        }

        /// <summary>
        /// 创建布尔 false 参数并返回其占位符。
        /// </summary>
        /// <returns>参数占位符。</returns>
        protected object GetQuotedFalseValue()
        {
            return CreateParam(false);
        }

        private string BuildSelectExpression(List<SelectMember> fields, bool distinct)
        {
            var cols = fields ?? _pocoData.QueryColumns.Select(x => new SelectMember { PocoColumn = x.Value, EntityType = _pocoData.Type, PocoColumns = new[] { x.Value } });
            return string.Format("SELECT {0}{1} \nFROM {2}{3}",
                (distinct ? "DISTINCT " : ""),
                    string.Join(", ", cols.Select(x =>
                    {
                        if (x.SelectSql == null)
                        {
                            var pocoColumn = x.PocoColumns.Last();
                            return (PrefixFieldWithTableName
                                ? _databaseType.EscapeTableName(_pocoData.TableInfo.AutoAlias)
                                  + "." + _databaseType.EscapeSqlIdentifier(x.PocoColumn.ColumnName)
                                  + " as " + (string.IsNullOrWhiteSpace(pocoColumn.ColumnAlias)
                                      ? _databaseType.EscapeSqlIdentifier(pocoColumn.MemberInfoKey)
                                      : _databaseType.EscapeSqlIdentifier(pocoColumn.ColumnAlias))
                                : _databaseType.EscapeSqlIdentifier(x.PocoColumn.ColumnName));
                        }

                        return x.SelectSql;
                    }).ToArray()),
                    _databaseType.EscapeTableName(_pocoData.TableInfo.TableName) + (PrefixFieldWithTableName ? " " + _databaseType.EscapeTableName(_pocoData.TableInfo.AutoAlias) : string.Empty),
                    tableHint);
        }

        internal List<PocoColumn> GetAllMembers()
        {
            return _pocoData.Columns.Values.ToList();
        }

        /// <summary>
        /// 对 SQL 应用分页处理。
        /// </summary>
        /// <param name="sql">待分页的 SQL。</param>
        /// <param name="columns">分页涉及的列集合。</param>
        /// <param name="joinSqlExpressions">关联查询表达式集合。</param>
        /// <returns>分页后的 SQL。</returns>
        protected virtual string ApplyPaging(string sql, IEnumerable<PocoColumn[]> columns, Dictionary<string, JoinData> joinSqlExpressions)
        {
            if (!Rows.HasValue || Rows == 0)
                return sql;

            string sqlPage;
            var parms = _params.Select(x => x).ToArray();

            // Split the SQL
            SQLParts parts;
            if (!PagingHelper.SplitSQL(sql, out parts)) throw new Exception("Unable to parse SQL statement for paged query");

            if (columns != null && columns.Any() && _databaseType.UseColumnAliases())
            {
                parts.sqlColumns = string.Join(", ", columns.Select(x => _databaseType.EscapeSqlIdentifier(x.Last().MemberInfoKey)).ToArray());
            }

            sqlPage = _databaseType.BuildPageQuery(Skip ?? 0, Rows ?? 0, parts, ref parms);

            _params.Clear();
            _params.AddRange(parms);

            return sqlPage;
        }

        private string BuildInStatement(Expression m, object quotedColName)
        {
            var member = Expression.Convert(m, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(member);
            var getter = lambda.Compile();

            if (quotedColName == null)
                quotedColName = Visit(m);

            var inArgs = ((IEnumerable)getter()).Cast<object>().ToList();
            if (inArgs.Count == 0)
            {
                return "1 = 0";
            }

            var sIn = FlattenList(inArgs, quotedColName);
            var statement = string.Format("{0} {1} ({2})", quotedColName, "IN", sIn);
            return statement;
        }

        /// <summary>
        /// 访问列上的方法调用（如 ToUpper、StartsWith、Substring 等）。
        /// </summary>
        /// <param name="m">待访问的方法调用表达式。</param>
        /// <returns>对应的 SQL 片段。</returns>
        protected virtual object VisitColumnAccessMethod(MethodCallExpression m)
        {
            var expression = (PartialSqlString)Visit(m.Object);

            if (_projection && expression is MemberAccessString)
                return expression;

            string statement;
            List<Object> args = this.VisitExpressionList(m.Arguments);

            switch (m.Method.Name)
            {
                case "ToUpper":
                    statement = string.Format("upper({0})", expression);
                    break;
                case "ToLower":
                    statement = string.Format("lower({0})", expression);
                    break;
                case "StartsWith":
                    statement = CreateLikeStatement(expression, CreateParam(EscapeParam(args[0]) + "%"));
                    break;
                case "EndsWith":
                    statement = CreateLikeStatement(expression, CreateParam("%" + EscapeParam(args[0])));
                    break;
                case "Contains":
                    statement = CreateLikeStatement(expression, CreateParam("%" + EscapeParam(args[0]) + "%"));
                    break;
                case "Substring":
                    var startIndex = Int32.Parse(args[0].ToString()) + 1;
                    var length = (args.Count > 1) ? Int32.Parse(args[1].ToString()) : -1;
                    statement = SubstringStatement(expression, startIndex, length);
                    break;
                case "Trim":
                    statement = CreateTrimStatement(expression, true, true);
                    break;
                case "TrimStart":
                    statement = CreateTrimStatement(expression, true, false);
                    break;
                case "TrimEnd":
                    statement = CreateTrimStatement(expression, false, true);
                    break;
                case "Equals":
                    statement = string.Format("({0} = {1})", expression, CreateParam(args[0]));
                    break;
                case "ToString":
                    statement = string.Empty;
                    break;
                default:
                    throw new NotSupportedException();
            }

            return new PartialSqlString(statement);
        }

        /// <summary>
        /// 生成 LIKE 语句。
        /// </summary>
        /// <param name="expression">列表达式片段。</param>
        /// <param name="param">匹配模式参数占位符。</param>
        /// <returns>LIKE 语句。</returns>
        protected virtual string CreateLikeStatement(PartialSqlString expression, string param)
        {
            return string.Format("upper({0}) like {1} escape '{2}'", expression, param, EscapeChar);
        }

        /// <summary>
        /// 生成 TRIM 语句。
        /// </summary>
        /// <param name="expression">列表达式片段。</param>
        /// <param name="start">是否去除左侧空白。</param>
        /// <param name="end">是否去除右侧空白。</param>
        /// <returns>TRIM 语句。</returns>
        protected virtual string CreateTrimStatement(PartialSqlString expression, bool start, bool end)
        {
            var result = expression.ToString();

            if (end) result = string.Format("rtrim({0})", result);
            if (start) result = string.Format("ltrim({0})", result);

            return result;
        }

        /// <summary>
        /// 对模糊匹配参数中的特殊字符进行转义。
        /// </summary>
        /// <param name="par">待转义的参数。</param>
        /// <returns>转义后的字符串。</returns>
        protected virtual string EscapeParam(object par)
        {
            var param = par.ToString().ToUpper();
            param = param
                .Replace(EscapeChar, EscapeChar + EscapeChar)
                .Replace("_", EscapeChar + "_");
            return param;
        }

        // Easy to override
        /// <summary>
        /// 生成 substring 子串 SQL。
        /// </summary>
        /// <param name="columnName">列名 SQL 片段。</param>
        /// <param name="startIndex">起始位置。</param>
        /// <param name="length">子串长度，小于 0 表示取到末尾。</param>
        /// <returns>substring 子串 SQL。</returns>
        protected virtual string SubstringStatement(PartialSqlString columnName, int startIndex, int length)
        {
            if (length >= 0)
                return string.Format("substring({0},{1},{2})", columnName, CreateParam(startIndex), CreateParam(length));
            else
                return string.Format("substring({0},{1},8000)", columnName, CreateParam(startIndex));
        }

        /// <summary>
        /// 根据成员名生成日期时间取值 SQL。
        /// </summary>
        /// <param name="memberName">DateTime 成员名，如 Year、Month、Day 等。</param>
        /// <param name="m">字段的 SQL 片段。</param>
        /// <returns>对应的日期时间取值 SQL。</returns>
        protected virtual string GetDateTimeSql(string memberName, object m)
        {
            string sql;
            switch (memberName)
            {
                case "Year": sql = $"DATEPART(YEAR,{m})"; break;
                case "Month": sql = $"DATEPART(MONTH,{m})"; break;
                case "Day": sql = $"DATEPART(DAY,{m})"; break;
                case "Hour": sql = $"DATEPART(HOUR,{m})"; break;
                case "Minute": sql = $"DATEPART(MINUTE,{m})"; break;
                case "Second": sql = $"DATEPART(SECOND,{m})"; break;
                default: throw new NotSupportedException("Not Supported " + memberName);
            }
            return sql;
        }
    }

    /// <summary>
    /// 表示部分 SQL 字符串片段。
    /// </summary>
    public class PartialSqlString
    {
        /// <summary>
        /// 使用指定文本初始化片段。
        /// </summary>
        /// <param name="text">SQL 文本。</param>
        public PartialSqlString(string text)
        {
            Text = text;
        }
        /// <summary>
        /// SQL 文本内容。
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 返回 SQL 文本。
        /// </summary>
        /// <returns>SQL 文本。</returns>
        public override string ToString()
        {
            return Text;
        }
    }

    /// <summary>
    /// 表示列成员的访问片段，携带列与实体类型信息。
    /// </summary>
    public class MemberAccessString : PartialSqlString
    {
        /// <summary>
        /// 使用指定列信息、文本与类型初始化实例。
        /// </summary>
        /// <param name="pocoColumn">对应的列信息。</param>
        /// <param name="pocoColumns">关联的列信息数组。</param>
        /// <param name="text">SQL 文本。</param>
        /// <param name="type">实体类型。</param>
        public MemberAccessString(PocoColumn pocoColumn, PocoColumn[] pocoColumns, string text, Type type)
            : base(text)
        {
            PocoColumn = pocoColumn;
            PocoColumns = pocoColumns;
            Type = type;
        }

        /// <summary>
        /// 对应的列信息。
        /// </summary>
        public PocoColumn PocoColumn { get; private set; }
        /// <summary>
        /// 关联的列信息数组。
        /// </summary>
        public PocoColumn[] PocoColumns { get; private set; }
        /// <summary>
        /// 实体类型。
        /// </summary>
        public Type Type { get; set; }
    }

    /// <summary>
    /// 表示可空成员的列访问片段。
    /// </summary>
    public class NullableMemberAccess : MemberAccessString
    {
        /// <summary>
        /// 使用指定列信息、文本与类型初始化实例。
        /// </summary>
        /// <param name="pocoColumn">对应的列信息。</param>
        /// <param name="pocoColumns">关联的列信息数组。</param>
        /// <param name="text">SQL 文本。</param>
        /// <param name="type">实体类型。</param>
        public NullableMemberAccess(PocoColumn pocoColumn, PocoColumn[] pocoColumns, string text, Type type)
            : base(pocoColumn, pocoColumns, text, type)
        {
        }
    }

    /// <summary>
    /// 表示枚举成员的列访问片段。
    /// </summary>
    public class EnumMemberAccess : MemberAccessString
    {
        /// <summary>
        /// 使用指定列信息、文本与类型初始化实例。
        /// </summary>
        /// <param name="pocoColumn">对应的列信息。</param>
        /// <param name="pocoColumns">关联的列信息数组。</param>
        /// <param name="text">SQL 文本。</param>
        /// <param name="type">实体类型。</param>
        public EnumMemberAccess(PocoColumn pocoColumn, PocoColumn[] pocoColumns, string text, Type type)
            : base(pocoColumn, pocoColumns, text, type)
        {
        }
    }

}