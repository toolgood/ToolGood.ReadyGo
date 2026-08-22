using System;
using System.Dynamic;
using System.Linq.Expressions;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Internals
{
    /// <summary>
    /// 表名动态类
    /// </summary>
    public class TableName : DynamicObject
    {
        internal string _asName;
        internal PocoData _pocoData;
        internal IDatabaseType _databaseType;

        public TableName(PocoData pocoData, IDatabaseType databaseType, string asName)
        {
            _pocoData = pocoData;
            _databaseType = databaseType;
            _asName = asName;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var fieldName = binder.Name;
            if (_pocoData.Columns.ContainsKey(fieldName)) {
                if (string.IsNullOrEmpty(_asName)) {
                    result = _databaseType.EscapeSqlIdentifier(_pocoData.Columns[fieldName].ColumnName);
                } else {
                    result = _asName + "." + _databaseType.EscapeSqlIdentifier(_pocoData.Columns[fieldName].ColumnName);
                }
                return true;
            }
            var lowerFieldName = fieldName.Replace("_", "");
            foreach (var item in _pocoData.Columns) {
                if (item.Value.MemberInfoKey.Replace("_", "").Equals(lowerFieldName, StringComparison.OrdinalIgnoreCase)) {
                    if (string.IsNullOrEmpty(_asName)) {
                        result = _databaseType.EscapeSqlIdentifier(item.Value.ColumnName);
                    } else {
                        result = _asName + "." + _databaseType.EscapeSqlIdentifier(item.Value.ColumnName);
                    }
                    return true;
                }
            }
            result = null;
            return false;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_asName)) {
                return _databaseType.EscapeTableName(_pocoData.TableInfo.TableName);
            }
            return _databaseType.EscapeTableName(_pocoData.TableInfo.TableName) + " " + _asName;
        }
    }

    /// <summary>
    /// 表名动态类
    /// </summary>
    public class TableName<T> : TableName
        where T : class, new()
    {
        public TableName(PocoData pocoData, IDatabaseType databaseType, string asName) : base(pocoData, databaseType, asName)
        {
        }

        /// <summary>
        /// 获取字段名
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public string F<T1>(Expression<Func<T, T1>> field)
        {
            var fieldName = GetFieldName(field.Body);
            if (string.IsNullOrEmpty(_asName)) {
                return fieldName;
            }
            return _asName + "." + fieldName;
        }

        private static string GetFieldName(Expression body)
        {
            if (body is MemberExpression memberExpression) {
                return memberExpression.Member.Name;
            }
            if (body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression memberExpression2) {
                return memberExpression2.Member.Name;
            }
            throw new NotSupportedException("仅支持 x => x.字段 形式的表达式。");
        }
    }
}
