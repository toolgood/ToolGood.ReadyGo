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
using ToolGood.ReadyGo.Gadget;
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
        /// 传字符串时，若实体主键为字符串类型则按主键值参数化查询，否则作为 SQL 片段（请自行确保 SQL 安全）。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">条件：整数主键 / 字符串主键或 SQL 片段 / 条件对象 / null</param>
        /// <returns>匹配条件的实体，无结果时返回 null</returns>
        public T FirstOrDefault<T>(object condition) where T : class
        {
            if (TryGetPrimaryKey<T>(condition, out var primaryKey)) {
                return SingleOrDefaultById<T>(primaryKey);
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
        /// 根据条件更新对象。set 对象中的主键列会被自动忽略，避免意外修改主键。
        /// condition 无法解析出 WHERE 条件（null、空字符串、空对象等）时会抛出异常，防止意外全表更新。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="set">要更新的字段与值对象</param>
        /// <param name="condition">条件</param>
        /// <returns>受影响的行数</returns>
        public int Update<T>(object set, object condition) where T : class
        {
            var (sql, args) = ConditionObjectToUpdateSetWhere<T>(set, condition, null);
            return Update<T>(sql, args);
        }

        /// <summary>
        /// 根据条件更新对象，仅更新指定的字段（columns）。set 对象中的主键列仍会被自动忽略，避免意外修改主键。
        /// condition 无法解析出 WHERE 条件（null、空字符串、空对象等）时会抛出异常，防止意外全表更新。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="set">要更新的字段与值对象</param>
        /// <param name="condition">条件</param>
        /// <param name="columns">仅更新这些字段名</param>
        /// <returns>受影响的行数</returns>
        public int Update<T>(object set, object condition, IEnumerable<string> columns) where T : class
        {
            var (sql, args) = ConditionObjectToUpdateColumnsWhere<T>(set, condition, columns);
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
            if (TryGetPrimaryKey<T>(condition, out var primaryKey)) {
                var (sql, args) = BuildPrimaryKeyExistsQuery<T>(primaryKey);
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
        /// 传字符串时，若实体主键为字符串类型则按主键值参数化查询，否则作为 SQL 片段（请自行确保 SQL 安全）。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="condition">条件：整数主键 / 字符串主键或 SQL 片段 / 条件对象 / null</param>
        /// <returns>匹配条件的实体，无结果时返回 null</returns>
        public Task<T> FirstOrDefault_Async<T>(object condition) where T : class
        {
            if (TryGetPrimaryKey<T>(condition, out var primaryKey)) {
                return SingleOrDefaultById_Async<T>(primaryKey);
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
        /// 根据条件更新对象，异步操作。set 对象中的主键列会被自动忽略，避免意外修改主键。
        /// condition 无法解析出 WHERE 条件（null、空字符串、空对象等）时会抛出异常，防止意外全表更新。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="set">要更新的字段与值对象</param>
        /// <param name="condition">条件</param>
        /// <returns>受影响的行数</returns>
        public Task<int> Update_Async<T>(object set, object condition) where T : class
        {
            var (sql, args) = ConditionObjectToUpdateSetWhere<T>(set, condition, null);
            return Update_Async<T>(sql, args);
        }

        /// <summary>
        /// 根据条件更新对象，仅更新指定的字段（columns），异步操作。set 对象中的主键列仍会被自动忽略，避免意外修改主键。
        /// condition 无法解析出 WHERE 条件（null、空字符串、空对象等）时会抛出异常，防止意外全表更新。
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="set">要更新的字段与值对象</param>
        /// <param name="condition">条件</param>
        /// <param name="columns">仅更新这些字段名</param>
        /// <returns>受影响的行数</returns>
        public Task<int> Update_Async<T>(object set, object condition, IEnumerable<string> columns) where T : class
        {
            var (sql, args) = ConditionObjectToUpdateColumnsWhere<T>(set, condition, columns);
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
            if (TryGetPrimaryKey<T>(condition, out var primaryKey)) {
                var (sql, args) = BuildPrimaryKeyExistsQuery<T>(primaryKey);
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
            var (whereSql, args) = BuildPrimaryKeyWhereSql<T>(primaryKey, 0);
            return ($"SELECT COUNT(*) FROM {table} WHERE {whereSql}", args);
        }

        /// <summary>
        /// 构建"按主键查询"的 WHERE 片段与参数：仅支持单一主键列，复合主键会抛出明确异常。
        /// index 为参数占位符的起始编号，必须与调用方当前已收集的参数数量一致，避免占位符错位。
        /// </summary>
        private (string whereSql, object[] args) BuildPrimaryKeyWhereSql<T>(object primaryKey, int index) where T : class
        {
            var db = GetDatabase();
            var pd = db.PocoDataFactory.ForType(typeof(T));
            var pkColumns = (pd.TableInfo.PrimaryKey ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
            if (pkColumns.Length != 1) {
                throw new NotSupportedException($"类型 {typeof(T).Name} 的主键不是单一主键列，无法按主键生成查询条件。");
            }
            var pk = db.DatabaseType.EscapeSqlIdentifier(pkColumns[0]);
            return ($"{pk}=@{index}", new object[] { primaryKey });
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
        /// 获取实体主键列名数组；无主键或未映射时返回空数组。
        /// </summary>
        private string[] GetPrimaryKeyColumns(Type entityType)
        {
            var pkName = GetPocoData(entityType).TableInfo.PrimaryKey;
            if (string.IsNullOrEmpty(pkName)) { return Array.Empty<string>(); }
            return pkName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
        }

        /// <summary>
        /// 获取单一主键列对应的属性类型；复合主键或未映射时返回 null。
        /// </summary>
        private Type GetPrimaryKeyColumnType(Type entityType)
        {
            var pkColumns = GetPrimaryKeyColumns(entityType);
            if (pkColumns.Length != 1) { return null; }
            var pd = GetPocoData(entityType);
            var member = pd.Members.FirstOrDefault(m => m.PocoColumn != null
                && string.Equals(m.PocoColumn.ColumnName, pkColumns[0], StringComparison.OrdinalIgnoreCase));
            return member?.PocoColumn?.ColumnType;
        }

        /// <summary>
        /// 实体是否支持"字符串主键值"：主键为单一列且列类型为 string。
        /// </summary>
        private bool IsStringPrimaryKey(Type entityType)
        {
            return GetPrimaryKeyColumnType(entityType) == typeof(string);
        }

        /// <summary>
        /// 解析 object 条件：整数或字符串主键时返回 true 并输出主键值；否则返回 false，交由条件对象/SQL 片段路径处理。
        /// 非整数、非字符串的值类型会抛出明确异常；整数条件要求实体为单一主键列，复合主键同样抛出明确异常，
        /// 避免被静默当作无条件或生成非法 SQL。
        /// </summary>
        private bool TryGetPrimaryKey<T>(object condition, out object primaryKey) where T : class
        {
            primaryKey = null;
            if (condition == null) { return false; }
            var type = condition.GetType();
            if (type.IsClass == false) {
                if (IsIntegerType(type) == false) {
                    throw new ArgumentException($"condition 类型 {type} 不支持作为主键，仅支持整数类型主键。");
                }
                if (GetPrimaryKeyColumns(typeof(T)).Length != 1) {
                    throw new ArgumentException($"类型 {typeof(T).Name} 的主键不是单一主键列，无法使用整数条件 {condition} 作为主键条件。");
                }
                primaryKey = condition;
                return true;
            }
            if (condition is string str) {
                var trimmed = str.Trim();
                if (trimmed.Length > 0 && IsStringPrimaryKey(typeof(T))) {
                    primaryKey = str;
                    return true;
                }
            }
            return false;
        }

        private (string sql, object[] args) ConditionObjectToWhere<T>(object condition) where T : class
        {
            if (condition == null) { return ("", new object[0]); }
            if (TryGetPrimaryKey<T>(condition, out var primaryKey)) {
                var (whereSql, pkArgs) = BuildPrimaryKeyWhereSql<T>(primaryKey, 0);
                return ($"WHERE {whereSql}", pkArgs);
            }
            if (condition is string) {
                var str = ((string)condition).Trim();
                if (str.Length == 0) { return ("", new object[0]); }
                return (IsWhereClause(str) ? str : "WHERE " + str, new object[0]);
            }

            var args = new List<object>();
            StringBuilder stringBuilder = new StringBuilder();
            if (ObjectToSql(stringBuilder, condition, ObjectSqlMode.Where, null, GetPocoData(typeof(T)), args) == false) {
                // 空对象（无可用属性）视为显式的无条件查询，与 null 一致
                return ("", new object[0]);
            }
            stringBuilder.Insert(0, "WHERE ");
            return (stringBuilder.ToString(), args.ToArray());
        }

        /// <summary>
        /// 根据条件更新对象（忽略指定字段）：set 对象中的主键列会被自动忽略，避免意外修改主键。
        /// </summary>
        private (string sql, object[] args) ConditionObjectToUpdateSetWhere<T>(object set, object condition, IEnumerable<string> ignoreFields) where T : class
        {
            return BuildUpdateSetWhereSql<T>(set, condition, BuildIgnoredFields(GetPocoData(typeof(T)), ignoreFields));
        }

        /// <summary>
        /// 根据条件更新对象（仅更新指定字段）：除 updateColumns 指定的字段外，其余列一律忽略；主键列始终忽略。
        /// </summary>
        private (string sql, object[] args) ConditionObjectToUpdateColumnsWhere<T>(object set, object condition, IEnumerable<string> updateColumns) where T : class
        {
            return BuildUpdateSetWhereSql<T>(set, condition, BuildColumnsIgnoredFields(GetPocoData(typeof(T)), updateColumns));
        }

        /// <summary>
        /// 根据条件更新对象：ignored 为已构建好的忽略字段集合。
        /// condition 无法解析出 WHERE 条件（null、空字符串、空对象等）时会抛出异常，防止意外全表更新。
        /// </summary>
        private (string sql, object[] args) BuildUpdateSetWhereSql<T>(object set, object condition, HashSet<string> ignored) where T : class
        {
            if (set == null) { throw new ArgumentException("set is  null object!"); }

            var pocoData = GetPocoData(typeof(T));
            var args = new List<object>();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SET ");
            if (ObjectToSql(stringBuilder, set, ObjectSqlMode.UpdateSet, ignored, pocoData, args) == false) {
                throw new ArgumentException("set 对象没有可更新的字段！");
            }
            var hasWhere = false;
            if (condition != null) {
                if (TryGetPrimaryKey<T>(condition, out var primaryKey)) {
                    var (whereSql, pkArgs) = BuildPrimaryKeyWhereSql<T>(primaryKey, args.Count);
                    stringBuilder.Append(" WHERE ");
                    stringBuilder.Append(whereSql);
                    args.AddRange(pkArgs);
                    hasWhere = true;
                } else if (condition is string) {
                    var str = ((string)condition).Trim();
                    if (str.Length > 0) {
                        stringBuilder.Append(IsWhereClause(str) ? " " : " WHERE ");
                        stringBuilder.Append(str);
                        hasWhere = true;
                    }
                } else {
                    StringBuilder whereBuilder = new StringBuilder();
                    if (ObjectToSql(whereBuilder, condition, ObjectSqlMode.Where, null, pocoData, args)) {
                        stringBuilder.Append(" WHERE ");
                        stringBuilder.Append(whereBuilder);
                        hasWhere = true;
                    }
                }
            }
            if (hasWhere == false) {
                throw new ArgumentException("更新操作缺少 WHERE 条件，禁止无条件的 UPDATE，避免意外全表更新！");
            }
            return (stringBuilder.ToString(), args.ToArray());
        }

        /// <summary>
        /// 构建忽略字段集合：在用户显式 ignoreFields 之外，自动排除主键列，避免拿完整实体做 set 时意外修改主键。
        /// </summary>
        private static HashSet<string> BuildIgnoredFields(PocoData pocoData, IEnumerable<string> ignoreFields)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (ignoreFields != null) {
                foreach (var field in ignoreFields) {
                    if (field != null) { set.Add(field); }
                }
            }
            var primaryKey = pocoData?.TableInfo?.PrimaryKey;
            if (!string.IsNullOrEmpty(primaryKey)) {
                foreach (var pkName in primaryKey.Split(',')) {
                    var column = pkName.Trim();
                    var member = pocoData.Members.FirstOrDefault(m => m.PocoColumn != null
                        && string.Equals(m.PocoColumn.ColumnName, column, StringComparison.OrdinalIgnoreCase));
                    if (member != null) { set.Add(member.Name); }
                }
            }
            return set;
        }

        /// <summary>
        /// 构建"仅更新指定字段"的忽略集合：默认忽略实体的全部可更新列，仅保留 updateColumns 指定的列；主键列始终忽略。
        /// </summary>
        private static HashSet<string> BuildColumnsIgnoredFields(PocoData pocoData, IEnumerable<string> updateColumns)
        {
            var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (updateColumns != null) {
                foreach (var column in updateColumns) {
                    if (column != null) { columns.Add(column); }
                }
            }
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (pocoData != null) {
                foreach (var member in pocoData.Members) {
                    if (member.PocoColumn != null && member.ReferenceType == ReferenceType.None && columns.Contains(member.Name) == false) {
                        set.Add(member.Name);
                    }
                }
            }
            var primaryKey = pocoData?.TableInfo?.PrimaryKey;
            if (!string.IsNullOrEmpty(primaryKey)) {
                foreach (var pkName in primaryKey.Split(',')) {
                    var column = pkName.Trim();
                    var member = pocoData.Members.FirstOrDefault(m => m.PocoColumn != null
                        && string.Equals(m.PocoColumn.ColumnName, column, StringComparison.OrdinalIgnoreCase));
                    if (member != null) { set.Add(member.Name); }
                }
            }
            return set;
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
                return t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                    .Select(p => new PropertyAccessor {
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
                // 按枚举底层类型转换，避免 ulong 等大值枚举在 Convert.ToInt64 时溢出
                value = EnumHelper.UseEnumString(value.GetType())
                    ? value.ToString()
                    : Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
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

            var ignored = ignoreFields as ISet<string>
                ?? ignoreFields?.ToHashSet(StringComparer.OrdinalIgnoreCase);

            var type = condition.GetType();
            var accessors = GetPropertyAccessors(type);
            for (int i = 0; i < accessors.Length; i++) {
                var accessor = accessors[i];
                var pi = accessor.Property;
                if (ignored != null && ignored.Contains(pi.Name)) {
                    continue;
                }
                var columnName = GetColumnName(pocoData, pi.Name);
                if (columnName == null) {
                    // 跳过非标量映射属性（导航属性、[Ignore]、未映射属性），避免生成非法 SQL
                    continue;
                }
                if (hasColumn == false) {
                    hasColumn = true;
                } else {
                    stringBuilder.Append(mode == ObjectSqlMode.Where ? " AND " : ",");
                }

                var escapedColumn = db.DatabaseType.EscapeSqlIdentifier(columnName);
                var value = accessor.Getter(condition);
                if (mode == ObjectSqlMode.Where) {
                    if (value == null) {
                        stringBuilder.Append(escapedColumn);
                        stringBuilder.Append(" is Null");
                    } else if (value is IEnumerable && !(value is string) && !(value is byte[]) && !(value is char[]) && !(value is IDictionary)) {
                        List<object> objs = new List<object>();
                        foreach (var item in (IEnumerable)value) { objs.Add(item); }

                        var hasNull = objs.Any(o => o == null);
                        var values = objs.Where(o => o != null).ToList();
                        if (hasNull == false && values.Count == 0) {
                            stringBuilder.Append("1=2");
                        } else if (hasNull) {
                            // null 元素应生成 is Null，且与 in/等值用 OR 连接时需要括号保证优先级
                            if (values.Count == 0) {
                                // 全部为 null，直接生成 is Null，避免出现非法的 "in ()"
                                stringBuilder.Append(escapedColumn);
                                stringBuilder.Append(" is Null");
                            } else {
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
                            }
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
                        // 集合值先尝试按列序列化器转换（如 float[] 默认按 byte[] 保存、数值/字符串数组按文本保存），转换失败再报错
                        var pocoColumn = GetPocoColumn(pocoData, pi.Name);
                        if (pocoColumn != null && pocoColumn.SerializedColumn) {
                            value = (pocoColumn.ColumnSerializer ?? db.Mappers.ColumnSerializer).Serialize(value);
                        }
                        if (value is IEnumerable && !(value is string) && !(value is byte[])) {
                            throw new ArgumentException($"set 对象属性 '{pi.Name}' 不支持集合值，无法生成 UPDATE SQL。");
                        }
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
        /// 判断字符串是否为 WHERE 子句（以 WHERE 开头且其后为空白或行尾），用于决定是否自动补全 WHERE 前缀。
        /// </summary>
        private static bool IsWhereClause(string str)
        {
            var s = str.TrimStart();
            if (s.StartsWith("WHERE", StringComparison.CurrentCultureIgnoreCase) == false) { return false; }
            if (s.Length == 5) { return true; }
            return char.IsWhiteSpace(s[5]);
        }

        /// <summary>
        /// 每个 PocoData 缓存的"属性名 → 列名"映射。
        /// </summary>
        private static readonly ConcurrentDictionary<PocoData, IReadOnlyDictionary<string, string>> _columnNameCache = new ConcurrentDictionary<PocoData, IReadOnlyDictionary<string, string>>();

        /// <summary>
        /// 根据属性名解析数据库列名，遵循 PocoData 的列名映射（如 [Column] 特性）；找不到映射时返回 null。
        /// </summary>
        private static string GetColumnName(PocoData pocoData, string propertyName)
        {
            if (pocoData == null) { return null; }
            var map = _columnNameCache.GetOrAdd(pocoData, pd => {
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var member in pd.Members) {
                    if (member.PocoColumn != null && member.ReferenceType == ReferenceType.None) {
                        dict[member.Name] = member.PocoColumn.ColumnName;
                    }
                }
                return dict;
            });
            return map.TryGetValue(propertyName, out var columnName) ? columnName : null;
        }

        /// <summary>
        /// 每个 PocoData 缓存的"属性名 → PocoColumn"映射，用于获取列的序列化器等映射信息。
        /// </summary>
        private static readonly ConcurrentDictionary<PocoData, IReadOnlyDictionary<string, PocoColumn>> _pocoColumnCache = new ConcurrentDictionary<PocoData, IReadOnlyDictionary<string, PocoColumn>>();

        /// <summary>
        /// 根据属性名解析对应的 PocoColumn（含列序列化器信息）；找不到映射时返回 null。
        /// </summary>
        private static PocoColumn GetPocoColumn(PocoData pocoData, string propertyName)
        {
            if (pocoData == null) { return null; }
            var map = _pocoColumnCache.GetOrAdd(pocoData, pd => {
                var dict = new Dictionary<string, PocoColumn>(StringComparer.OrdinalIgnoreCase);
                foreach (var member in pd.Members) {
                    if (member.PocoColumn != null && member.ReferenceType == ReferenceType.None) {
                        dict[member.Name] = member.PocoColumn;
                    }
                }
                return dict;
            });
            return map.TryGetValue(propertyName, out var pocoColumn) ? pocoColumn : null;
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
                    var txt = ToEscapeParam(value.ToString());
                    return "'" + txt + "'";
                }
                // 按枚举底层类型转换，避免 ulong 等大值枚举在 Convert.ToInt64 时溢出
                return "'" + Convert.ChangeType(value, Enum.GetUnderlyingType(fieldType)) + "'";
            }

            var typeCode = Type.GetTypeCode(fieldType);
            switch (typeCode) {
                case TypeCode.Boolean: return (bool)value ? "1" : "0";
                case TypeCode.Single: {
                    var f = (float)value;
                    if (float.IsNaN(f) || float.IsInfinity(f)) {
                        throw new ArgumentException($"float 值 {f} 无法转换为 SQL 字面量。");
                    }
                    return f.ToString(CultureInfo.InvariantCulture);
                }
                case TypeCode.Double: {
                    var d = (double)value;
                    if (double.IsNaN(d) || double.IsInfinity(d)) {
                        throw new ArgumentException($"double 值 {d} 无法转换为 SQL 字面量。");
                    }
                    return d.ToString(CultureInfo.InvariantCulture);
                }
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
                var txt = ToEscapeParam(value.ToString());
                return "'" + txt + "'";
            }
            if (fieldType == typeof(DateTime)) return "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
            if (fieldType == typeof(TimeSpan)) return ((TimeSpan)value).Ticks.ToString(CultureInfo.InvariantCulture);
            if (fieldType == typeof(byte[])) {
                var txt = BitConverter.ToString((byte[])value).Replace("-", "");
                return "X'" + txt + "'";
            }
            var text = ToEscapeParam(value.ToString());
            return "'" + text + "'";
        }
        /// <summary>
        /// 转义 SQL 字符串中的特殊字符
        /// </summary>
        /// <param name="stringValue">原始字符串</param>
        /// <returns>转义后的字符串</returns>
        private string ToEscapeParam(string stringValue)
        {
            if(string.IsNullOrEmpty(stringValue)) {
                return "";
            }

            return stringValue.Replace(@"\", @"\\").Replace("'", "\\'")
                                  .Replace("\0", "\\0").Replace("\a", "\\a").Replace("\b", "\\b")
                                  .Replace("\f", "\\f").Replace("\n", "\\n").Replace("\r", "\\r")
                                  .Replace("\t", "\\t").Replace("\v", "\\v");
        }
    }
}
