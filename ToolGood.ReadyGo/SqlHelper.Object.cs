using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo
{
    public partial class SqlHelper
    {
        #region Select Update

        #region FirstOrDefault PK

        /// <summary>
        /// 根据主键查询第一个
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">主键</param>
        /// <returns>匹配主键的实体，无结果时返回 null</returns>
        public T FirstOrDefault<T>(int condition) where T : class
        {
            return SingleOrDefaultById<T>(condition);
        }

        /// <summary>
        /// 根据主键查询第一个
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">主键</param>
        /// <returns>匹配主键的实体，无结果时返回 null</returns>
        public T FirstOrDefault<T>(uint condition) where T : class
        {
            return SingleOrDefaultById<T>(condition);
        }

        /// <summary>
        /// 根据主键查询第一个
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">主键</param>
        /// <returns>匹配主键的实体，无结果时返回 null</returns>
        public T FirstOrDefault<T>(long condition) where T : class
        {
            return SingleOrDefaultById<T>(condition);
        }

        /// <summary>
        /// 根据主键查询第一个
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">主键</param>
        /// <returns>匹配主键的实体，无结果时返回 null</returns>
        public T FirstOrDefault<T>(ulong condition) where T : class
        {
            return SingleOrDefaultById<T>(condition);
        }

        #endregion FirstOrDefault PK

        /// <summary>
        /// 根据条件查询第一个。
        /// 传 null 或条件对象表示按条件查询（null 为无条件，取第一条）；传整数主键表示按主键查询；
        /// 传字符串时，若实体主键为字符串且字符串不像 SQL 片段则按主键查询，否则作为 SQL 片段。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">条件：整数主键 / 字符串主键或 SQL 片段 / 条件对象 / null</param>
        /// <returns>匹配条件的实体，无结果时返回 null</returns>
        public T FirstOrDefault<T>(object condition) where T : class
        {
            if (condition != null && condition.GetType().IsClass == false) {
                if (IsIntegerType(condition.GetType()) == false) {
                    throw new ArgumentException($"condition 类型 {condition.GetType()} 不支持作为主键，仅支持整数类型主键。");
                }
                return SingleOrDefaultById<T>(condition);
            }
            if (condition is string str && IsStringPrimaryKeyValue<T>(str)) {
                return SingleOrDefaultById<T>(str);
            }
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return FirstOrDefault<T>(sql, args);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns>查询结果集合</returns>
        public List<T> Select<T>(int limit, int offset, object condition) where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return Select<T>(limit, offset, sql, args);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns>查询结果集合</returns>
        public List<T> Select<T>(int limit, object condition) where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return Select<T>(limit, sql, args);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">条件</param>
        /// <returns>查询结果集合</returns>
        public List<T> Select<T>(object condition) where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return Select<T>(sql, args);
        }

        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns>查询结果集合</returns>
        public List<T> SelectPage<T>(int page, int itemsPerPage, object condition)
            where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return SelectPage<T>(page, itemsPerPage, sql, args);
        }

        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns>分页结果</returns>
        public Page<T> Page<T>(int page, int itemsPerPage, object condition)
            where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return Page<T>(page, itemsPerPage, sql, args);
        }

        /// <summary>
        /// 根据条件更新对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="set">要更新的字段与值对象</param>
        /// <param name="condition">条件</param>
        /// <param name="ignoreFields">忽略的字段名集合</param>
        /// <returns>受影响的行数</returns>
        public int Update<T>(object set, object condition, IEnumerable<string> ignoreFields = null) where T : class
        {
            var (sql, args) = ConditionObjectToUpdateSetWhere<T>(set, condition, ignoreFields);
            return Update<T>(sql, args);
        }

        /// <summary>
        /// 根据条件从数据库中删除对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">条件</param>
        /// <returns>受影响的行数</returns>
        public int Delete<T>(object condition) where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return Delete<T>(sql, args);
        }

        /// <summary>
        /// 根据条件查询个数
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">条件</param>
        /// <returns>记录数量</returns>
        public int Count<T>(object condition) where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return Count<T>(sql, args);
        }

        /// <summary>
        /// 根据条件判断是否存在
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">条件</param>
        /// <returns>存在返回 true，否则返回 false</returns>
        public bool Exists<T>(object condition) where T : class
        {
            if (condition != null && condition.GetType().IsClass == false) {
                if (IsIntegerType(condition.GetType()) == false) {
                    throw new ArgumentException($"condition 类型 {condition.GetType()} 不支持作为主键，仅支持整数类型主键。");
                }
                var (sql, args) = BuildPrimaryKeyExistsQuery<T>(condition);
                return GetDatabase().ExecuteScalar<int>(sql, args) > 0;
            }
            if (condition is string str && IsStringPrimaryKeyValue<T>(str)) {
                var (sql, args) = BuildPrimaryKeyExistsQuery<T>(str);
                return GetDatabase().ExecuteScalar<int>(sql, args) > 0;
            }
            var (w, wa) = ConditionObjectToWhere<T>(condition);
            return Exists<T>(w, wa);
        }

        #endregion Select Update

        #region FirstOrDefault_Async PK

        /// <summary>
        /// 根据主键查询第一个，异步操作
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">主键</param>
        /// <returns>匹配主键的实体，无结果时返回 null</returns>
        public Task<T> FirstOrDefault_Async<T>(int condition) where T : class
        {
            return SingleOrDefaultById_Async<T>(condition);
        }

        /// <summary>
        /// 根据主键查询第一个，异步操作
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">主键</param>
        /// <returns>匹配主键的实体，无结果时返回 null</returns>
        public Task<T> FirstOrDefault_Async<T>(uint condition) where T : class
        {
            return SingleOrDefaultById_Async<T>(condition);
        }

        /// <summary>
        /// 根据主键查询第一个，异步操作
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">主键</param>
        /// <returns>匹配主键的实体，无结果时返回 null</returns>
        public Task<T> FirstOrDefault_Async<T>(long condition) where T : class
        {
            return SingleOrDefaultById_Async<T>(condition);
        }

        /// <summary>
        /// 根据主键查询第一个，异步操作
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">主键</param>
        /// <returns>匹配主键的实体，无结果时返回 null</returns>
        public Task<T> FirstOrDefault_Async<T>(ulong condition) where T : class
        {
            return SingleOrDefaultById_Async<T>(condition);
        }

        #endregion FirstOrDefault_Async PK

        /// <summary>
        /// 根据条件查询第一个，异步操作。
        /// 传 null 或条件对象表示按条件查询（null 为无条件，取第一条）；传整数主键表示按主键查询；
        /// 传字符串时，若实体主键为字符串且字符串不像 SQL 片段则按主键查询，否则作为 SQL 片段。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">条件：整数主键 / 字符串主键或 SQL 片段 / 条件对象 / null</param>
        /// <returns>匹配条件的实体，无结果时返回 null</returns>
        public Task<T> FirstOrDefault_Async<T>(object condition) where T : class
        {
            if (condition != null && condition.GetType().IsClass == false) {
                if (IsIntegerType(condition.GetType()) == false) {
                    throw new ArgumentException($"condition 类型 {condition.GetType()} 不支持作为主键，仅支持整数类型主键。");
                }
                return SingleOrDefaultById_Async<T>(condition);
            }
            if (condition is string str && IsStringPrimaryKeyValue<T>(str)) {
                return SingleOrDefaultById_Async<T>(str);
            }
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return FirstOrDefault_Async<T>(sql, args);
        }

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns>查询结果集合</returns>
        public Task<List<T>> Select_Async<T>(int limit, int offset, object condition) where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return Select_Async<T>(limit, offset, sql, args);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns>查询结果集合</returns>
        public Task<List<T>> Select_Async<T>(int limit, object condition) where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return Select_Async<T>(limit, sql, args);
        }

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">条件</param>
        /// <returns>查询结果集合</returns>
        public Task<List<T>> Select_Async<T>(object condition) where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return Select_Async<T>(sql, args);
        }

        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns>查询结果集合</returns>
        public Task<List<T>> SelectPage_Async<T>(int page, int itemsPerPage, object condition)
            where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return SelectPage_Async<T>(page, itemsPerPage, sql, args);
        }

        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns>分页结果</returns>
        public Task<Page<T>> Page_Async<T>(int page, int itemsPerPage, object condition)
            where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return Page_Async<T>(page, itemsPerPage, sql, args);
        }

        /// <summary>
        /// 根据条件更新对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="set">要更新的字段与值对象</param>
        /// <param name="condition">条件</param>
        /// <param name="ignoreFields">忽略的字段名集合</param>
        /// <returns>受影响的行数</returns>
        public Task<int> Update_Async<T>(object set, object condition, IEnumerable<string> ignoreFields = null) where T : class
        {
            var (sql, args) = ConditionObjectToUpdateSetWhere<T>(set, condition, ignoreFields);
            return Update_Async<T>(sql, args);
        }

        /// <summary>
        /// 根据条件从数据库中删除对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">条件</param>
        /// <returns>受影响的行数</returns>
        public Task<int> Delete_Async<T>(object condition) where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return Delete_Async<T>(sql, args);
        }

        /// <summary>
        /// 根据条件查询个数，异步操作
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">条件</param>
        /// <returns>记录数量</returns>
        public Task<int> Count_Async<T>(object condition) where T : class
        {
            var (sql, args) = ConditionObjectToWhere<T>(condition);
            return Count_Async<T>(sql, args);
        }

        /// <summary>
        /// 根据条件是判断否存在，异步操作
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">条件</param>
        /// <returns>存在返回 true，否则返回 false</returns>
        public async Task<bool> Exists_Async<T>(object condition) where T : class
        {
            if (condition != null && condition.GetType().IsClass == false) {
                if (IsIntegerType(condition.GetType()) == false) {
                    throw new ArgumentException($"condition 类型 {condition.GetType()} 不支持作为主键，仅支持整数类型主键。");
                }
                var (sql, args) = BuildPrimaryKeyExistsQuery<T>(condition);
                return await GetDatabase().ExecuteScalarAsync<int>(sql, args) > 0;
            }
            if (condition is string str && IsStringPrimaryKeyValue<T>(str)) {
                var (sql, args) = BuildPrimaryKeyExistsQuery<T>(str);
                return await GetDatabase().ExecuteScalarAsync<int>(sql, args) > 0;
            }
            var (w, wa) = ConditionObjectToWhere<T>(condition);
            return await Exists_Async<T>(w, wa);
        }

        /// <summary>
        /// 构建"按主键判断是否存在"的 SQL 与参数，供同步/异步 Exists 复用。
        /// </summary>
        private (string sql, object[] args) BuildPrimaryKeyExistsQuery<T>(object primaryKey) where T : class
        {
            var db = GetDatabase();
            var pd = db.PocoDataFactory.ForType(typeof(T));
            var table = db.DatabaseType.EscapeTableName(pd.TableInfo.TableName);
            var pk = db.DatabaseType.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);
            var sql = $"SELECT COUNT(*) FROM {table} WHERE {pk}=@0";
            return (sql, new object[] { primaryKey });
        }

        /// <summary>
        /// 获取唯一一个类型，若数量大于1，则抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey">主键名</param>
        /// <returns></returns>
        private T SingleOrDefaultById<T>(object primaryKey) where T : class
        {
            return GetDatabase().SingleOrDefaultById<T>(primaryKey)!;
        }

        /// <summary>
        /// 判断条件是否为整数主键类型（排除枚举与可空类型）。
        /// </summary>
        private static bool IsIntegerType(Type type)
        {
            if (type == null || type.IsEnum) { return false; }
            switch (Type.GetTypeCode(type)) {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 判断字符串是否为 SQL 片段（以 WHERE 开头、包含空白或 SQL 运算符/定界符）。
        /// </summary>
        private static bool IsSqlFragment(string value)
        {
            if (string.IsNullOrEmpty(value)) { return false; }
            var s = value.TrimStart();
            if (IsWhereClause(s)) { return true; }
            if (s.Any(char.IsWhiteSpace)) { return true; }
            return s.IndexOfAny(_sqlOperatorChars) >= 0;
        }

        private static readonly char[] _sqlOperatorChars = { '=', '<', '>', '!', '(', ')', ',', ';', '\'', '"', '`' };

        /// <summary>
        /// 获取实体主键列对应的属性类型；复合主键或未映射时返回 null。
        /// </summary>
        private Type GetPrimaryKeyType(Type entityType)
        {
            var pd = GetPocoData(entityType);
            var pkName = pd.TableInfo.PrimaryKey;
            if (string.IsNullOrEmpty(pkName)) { return null; }
            var member = pd.Members.FirstOrDefault(m => m.PocoColumn != null
                && string.Equals(m.PocoColumn.ColumnName, pkName, StringComparison.OrdinalIgnoreCase));
            return member?.PocoColumn?.ColumnType;
        }

        /// <summary>
        /// 判断字符串是否应按"字符串主键值"处理：仅当实体主键为字符串、值非空且不像 SQL 片段时成立。
        /// </summary>
        private bool IsStringPrimaryKeyValue<T>(string value) where T : class
        {
            if (GetPrimaryKeyType(typeof(T)) != typeof(string)) { return false; }
            var str = value.Trim();
            if (str.Length == 0) { return false; }
            return IsSqlFragment(str) == false;
        }

        private (string sql, object[] args) ConditionObjectToWhere<T>(object condition) where T : class
        {
            if (condition == null) { return ("", new object[0]); }
            if (condition.GetType() == typeof(string)) {
                var str = ((string)condition).Trim();
                if (str.Length == 0) { return ("", new object[0]); }
                return (IsWhereClause(str) ? str : "WHERE " + str, new object[0]);
            }

            var args = new List<object>();
            StringBuilder stringBuilder = new StringBuilder();
            if (ObjectToSql(stringBuilder, condition, ObjectSqlMode.Where, null, GetPocoData(typeof(T)), args) == false) {
                return ("", new object[0]);
            }
            stringBuilder.Insert(0, "WHERE ");
            return (stringBuilder.ToString(), args.ToArray());
        }

        private (string sql, object[] args) ConditionObjectToUpdateSetWhere<T>(object set, object condition, IEnumerable<string> ignoreFields) where T : class
        {
            if (set == null) { throw new ArgumentException("set is  null object!"); }

            var pocoData = GetPocoData(typeof(T));
            var args = new List<object>();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SET ");
            if (ObjectToSql(stringBuilder, set, ObjectSqlMode.UpdateSet, ignoreFields, pocoData, args) == false) {
                throw new ArgumentException("set 对象没有可更新的字段！");
            }
            if (condition != null) {
                if (condition.GetType() == typeof(string)) {
                    var str = ((string)condition).Trim();
                    if (str.Length > 0) {
                        stringBuilder.Append(IsWhereClause(str) ? " " : " WHERE ");
                        stringBuilder.Append(str);
                    }
                } else {
                    StringBuilder whereBuilder = new StringBuilder();
                    if (ObjectToSql(whereBuilder, condition, ObjectSqlMode.Where, null, pocoData, args)) {
                        stringBuilder.Append(" WHERE ");
                        stringBuilder.Append(whereBuilder);
                    }
                }
            }
            return (stringBuilder.ToString(), args.ToArray());
        }

        /// <summary>
        /// 对象转 SQL 的用途：作为 WHERE 条件，或作为 UPDATE 的 set 字段。
        /// </summary>
        private enum ObjectSqlMode
        {
            Where,
            UpdateSet
        }

        /// <summary>
        /// 缓存的属性访问器：避免每次调用都执行 GetProperties() 全量反射。
        /// </summary>
        private sealed class PropertyAccessor
        {
            public PropertyInfo Property;
            public Func<object, object> Getter;
        }

        private static readonly ConcurrentDictionary<Type, PropertyAccessor[]> _propertyAccessors = new ConcurrentDictionary<Type, PropertyAccessor[]>();

        private static PropertyAccessor[] GetPropertyAccessors(Type type)
        {
            return _propertyAccessors.GetOrAdd(type, t => {
                return t.GetProperties().Select(p => new PropertyAccessor {
                    Property = p,
                    Getter = BuildPropertyGetter(p)
                }).ToArray();
            });
        }

        private static Func<object, object> BuildPropertyGetter(PropertyInfo pi)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var body = Expression.Convert(
                Expression.Property(Expression.Convert(instance, pi.DeclaringType), pi),
                typeof(object));
            return Expression.Lambda<Func<object, object>>(body, instance).Compile();
        }

        /// <summary>
        /// 追加参数占位符 @N 并将值收集到参数列表；枚举统一转换为字符串或底层整数。
        /// </summary>
        private static void AppendParam(StringBuilder stringBuilder, object value, List<object> args)
        {
            stringBuilder.Append('@').Append(args.Count);
            if (value != null && value.GetType().IsEnum) {
                value = EnumHelper.UseEnumString(value.GetType()) ? value.ToString() : Convert.ToInt64(value);
            }
            args.Add(value);
        }

        /// <summary>
        /// 将条件对象/更新对象转换为 SQL 片段，值统一参数化。
        /// </summary>
        /// <param name="stringBuilder">SQL 片段输出</param>
        /// <param name="condition">条件对象或更新字段对象</param>
        /// <param name="mode">Where（生成 AND 条件）或 UpdateSet（生成逗号分隔的赋值）</param>
        /// <param name="ignoreFields">忽略的字段名集合</param>
        /// <param name="pocoData">实体映射元数据</param>
        /// <param name="args">收集到的 SQL 参数</param>
        /// <returns>是否生成了至少一个列条件；为 false 表示对象没有任何可用字段。</returns>
        private bool ObjectToSql(StringBuilder stringBuilder, object condition, ObjectSqlMode mode, IEnumerable<string> ignoreFields, PocoData pocoData, List<object> args)
        {
            if (condition is IEnumerable) { throw new ArgumentException("condition is IEnumerable object!"); }
            var db = GetDatabase();
            bool hasColumn = false;

            var type = condition.GetType();
            var accessors = GetPropertyAccessors(type);
            for (int i = 0; i < accessors.Length; i++) {
                var accessor = accessors[i];
                var pi = accessor.Property;
                if (ignoreFields != null) {
                    if (ignoreFields.Any(q => string.Equals(q, pi.Name, StringComparison.CurrentCultureIgnoreCase))) {
                        continue;
                    }
                }
                if (hasColumn == false) {
                    hasColumn = true;
                } else {
                    stringBuilder.Append(mode == ObjectSqlMode.Where ? " AND " : ",");
                }

                var columnName = GetColumnName(pocoData, pi.Name) ?? pi.Name;
                var escapedColumn = db.DatabaseType.EscapeSqlIdentifier(columnName);
                var value = accessor.Getter(condition);
                if (mode == ObjectSqlMode.Where) {
                    if (value == null) {
                        stringBuilder.Append(escapedColumn);
                        stringBuilder.Append(" is Null");
                    } else if (value is IEnumerable && !(value is string) && !(value is byte[])) {
                        List<object> objs = new List<object>();
                        foreach (var item in (IEnumerable)value) { objs.Add(item); }

                        var hasNull = objs.Any(o => o == null);
                        var values = objs.Where(o => o != null).ToList();
                        if (hasNull == false && values.Count == 0) {
                            stringBuilder.Append("1=2");
                        } else if (hasNull) {
                            // null 元素应生成 is Null，且与 in/等值用 OR 连接时需要括号保证优先级
                            stringBuilder.Append('(');
                            stringBuilder.Append(escapedColumn);
                            stringBuilder.Append(" is Null OR ");
                            stringBuilder.Append(escapedColumn);
                            if (values.Count == 1) {
                                stringBuilder.Append('=');
                                AppendParam(stringBuilder, values[0], args);
                            } else {
                                stringBuilder.Append(" in (");
                                for (int j = 0; j < values.Count; j++) {
                                    if (j > 0) { stringBuilder.Append(","); }
                                    AppendParam(stringBuilder, values[j], args);
                                }
                                stringBuilder.Append(')');
                            }
                            stringBuilder.Append(')');
                        } else if (values.Count == 1) {
                            stringBuilder.Append(escapedColumn);
                            stringBuilder.Append('=');
                            AppendParam(stringBuilder, values[0], args);
                        } else {
                            stringBuilder.Append(escapedColumn);
                            stringBuilder.Append(" in (");
                            for (int j = 0; j < values.Count; j++) {
                                if (j > 0) { stringBuilder.Append(","); }
                                AppendParam(stringBuilder, values[j], args);
                            }
                            stringBuilder.Append(')');
                        }
                    } else {
                        stringBuilder.Append(escapedColumn);
                        stringBuilder.Append('=');
                        AppendParam(stringBuilder, value, args);
                    }
                } else {
                    if (value is IEnumerable && !(value is string) && !(value is byte[])) {
                        throw new ArgumentException($"set 对象属性 '{pi.Name}' 不支持集合值，无法生成 UPDATE SQL。");
                    }
                    stringBuilder.Append(escapedColumn);
                    stringBuilder.Append('=');
                    AppendParam(stringBuilder, value, args);
                }
            }
            return hasColumn;
        }

        private PocoData GetPocoData(Type type)
        {
            return GetDatabase().PocoDataFactory.ForType(type);
        }

        /// <summary>
        /// 判断字符串是否为 WHERE 子句（以 WHERE 开头），用于决定是否自动补全 WHERE 前缀。
        /// </summary>
        private static bool IsWhereClause(string str)
        {
            return str.TrimStart().StartsWith("WHERE", StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 根据属性名解析数据库列名，遵循 PocoData 的列名映射（如 [Column] 特性）；找不到映射时返回 null。
        /// </summary>
        private static string GetColumnName(PocoData pocoData, string propertyName)
        {
            if (pocoData == null) { return null; }
            var member = pocoData.Members.FirstOrDefault(x => x.PocoColumn != null
                && x.ReferenceType == ReferenceType.None
                && string.Equals(x.Name, propertyName, StringComparison.OrdinalIgnoreCase));
            return member?.PocoColumn.ColumnName;
        }

        /// <summary>
        /// 将值转义为 SQL 字面量
        /// </summary>
        /// <param name="value">要转义的值</param>
        /// <returns>转义后的 SQL 字面量字符串</returns>
        protected string EscapeParam(object value)
        {
            if (object.Equals(value, null)) return "NULL";

            var fieldType = value.GetType();
            if (fieldType.IsEnum) {
                if (EnumHelper.UseEnumString(fieldType)) {
                    var txt = SqlUtil.ToEscapeParam(value.ToString());
                    return "'" + txt + "'";
                }
                return $"'{Convert.ToInt64(value)}'";
            }

            var typeCode = Type.GetTypeCode(fieldType);
            switch (typeCode) {
                case TypeCode.Boolean: return (bool)value ? "1" : "0";
                case TypeCode.Single: return ((float)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Double: return ((double)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Decimal: return ((decimal)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64: return value.ToString();
                default: break;
            }
            if (value is string || value is char) {
                var txt = SqlUtil.ToEscapeParam(value.ToString());
                return "'" + txt + "'";
            }
            if (fieldType == typeof(DateTime)) return "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
            if (fieldType == typeof(TimeSpan)) return ((TimeSpan)value).Ticks.ToString(CultureInfo.InvariantCulture);
            if (fieldType == typeof(byte[])) {
                var txt = BitConverter.ToString((byte[])value).Replace("-", "");
                return "X'" + txt + "'";
            }
            var text = SqlUtil.ToEscapeParam(value.ToString());
            return "'" + text + "'";
        }
    }
}
