using System;
using System.Dynamic;
using System.Linq.Expressions;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Gadget
{
    /// <summary>
    /// 表名动态类
    /// </summary>
    public class TableName : DynamicObject
    {
        internal string _asName;
        internal PocoData _pocoData;
        internal IDatabaseType _databaseType;

        /// <summary>
        /// 初始化表名动态对象
        /// </summary>
        /// <param name="pocoData">POCO 元数据</param>
        /// <param name="databaseType">数据库类型</param>
        /// <param name="asName">表别名</param>
        public TableName(PocoData pocoData, IDatabaseType databaseType, string asName)
        {
            _pocoData = pocoData;
            _databaseType = databaseType;
            _asName = asName;
        }

        /// <summary>
        /// 尝试获取成员（列名）对应的转义 SQL 标识符
        /// </summary>
        /// <param name="binder">成员绑定信息</param>
        /// <param name="result">输出的转义后的 SQL 标识符（含别名前缀）</param>
        /// <returns>是否成功获取到对应列</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var column = GetEscapedColumnName(binder.Name);
            if (column == null) {
                result = null;
                return false;
            }
            result = string.IsNullOrEmpty(_asName) ? column : _asName + "." + column;
            return true;
        }

        /// <summary>
        /// 根据字段名获取转义后的 SQL 标识符；支持忽略下划线的模糊匹配。
        /// </summary>
        /// <param name="fieldName">字段名。</param>
        /// <returns>转义后的列名，未匹配到时返回 null。</returns>
        protected string GetEscapedColumnName(string fieldName)
        {
            if (_pocoData.Columns.ContainsKey(fieldName)) {
                return _databaseType.EscapeSqlIdentifier(_pocoData.Columns[fieldName].ColumnName);
            }
            var lowerFieldName = fieldName.Replace("_", "");
            foreach (var item in _pocoData.Columns) {
                if (item.Value.MemberInfoKey.Replace("_", "").Equals(lowerFieldName, StringComparison.OrdinalIgnoreCase)) {
                    return _databaseType.EscapeSqlIdentifier(item.Value.ColumnName);
                }
            }
            return null;
        }

        /// <summary>
        /// 返回转义后的表名（含别名）
        /// </summary>
        /// <returns>转义后的表名</returns>
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
        /// <summary>
        /// 初始化强类型表名动态对象
        /// </summary>
        /// <param name="pocoData">POCO 元数据</param>
        /// <param name="databaseType">数据库类型</param>
        /// <param name="asName">表别名</param>
        public TableName(PocoData pocoData, IDatabaseType databaseType, string asName) : base(pocoData, databaseType, asName)
        {
        }

        /// <summary>
        /// 获取字段名
        /// </summary>
        /// <typeparam name="T1">字段类型</typeparam>
        /// <param name="field">字段选择表达式，如 x =&gt; x.Name</param>
        /// <returns>转义后的字段名（含别名前缀）</returns>
        public string F<T1>(Expression<Func<T, T1>> field)
        {
            var fieldName = GetFieldName(field.Body);
            var column = GetEscapedColumnName(fieldName);
            if (column == null) {
                throw new NotSupportedException($"属性 '{fieldName}' 未映射为数据库列。");
            }
            return string.IsNullOrEmpty(_asName) ? column : _asName + "." + column;
        }

        private static string GetFieldName(Expression body)
        {
            var member = GetMemberExpression(body);
            // x => x.A.B 这类嵌套成员访问无法映射为当前表的列名，直接报错而不是返回错误列名
            if (member.Expression is MemberExpression) {
                throw new NotSupportedException("不支持嵌套成员访问（如 x => x.A.B），仅支持单层属性访问（如 x => x.Name）。");
            }
            return member.Member.Name;
        }

        private static MemberExpression GetMemberExpression(Expression body)
        {
            if (body is MemberExpression memberExpression) {
                return memberExpression;
            }
            if (body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression memberExpression2) {
                return memberExpression2;
            }
            throw new NotSupportedException("仅支持 x => x.字段 形式的表达式。");
        }
    }
}
