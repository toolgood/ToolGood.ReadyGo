using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.Internals;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3
{
    partial class SqlHelper
    {
        #region Select Update
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T1 Single<T1>(int condition) where T1 : class
        {
            return SingleById<T1>(condition);
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T1 Single<T1>(long condition) where T1 : class
        {
            return SingleById<T1>(condition);
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T1 Single<T1>(object condition) where T1 : class
        {
            return Single<T1>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T1 SingleOrDefault<T1>(int condition) where T1 : class
        {
            return SingleOrDefaultById<T1>(condition);
        }
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T1 SingleOrDefault<T1>(long condition) where T1 : class
        {
            return SingleOrDefaultById<T1>(condition);
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T1 SingleOrDefault<T1>(object condition) where T1 : class
        {
            return SingleOrDefault<T1>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T1 First<T1>(int condition) where T1 : class
        {
            return SingleById<T1>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T1 First<T1>(long condition) where T1 : class
        {
            return SingleById<T1>(condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T1 First<T1>(object condition) where T1 : class
        {
            return First<T1>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T1 FirstOrDefault<T1>(int condition) where T1 : class
        {
            return SingleOrDefaultById<T1>(condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T1 FirstOrDefault<T1>(long condition) where T1 : class
        {
            return SingleOrDefaultById<T1>(condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T1 FirstOrDefault<T1>(object condition) where T1 : class
        {
            return FirstOrDefault<T1>(ConditionObjectToWhere(condition));
        }


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T1> Select<T1>(int limit, int offset, object condition) where T1 : class
        {
            return Select<T1>(limit, offset, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T1> Select<T1>(int limit, object condition) where T1 : class
        {
            return Select<T1>(limit, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T1> Select<T1>(object condition) where T1 : class
        {
            return Select<T1>(ConditionObjectToWhere(condition));
        }


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public List<T1> SelectWithOrderBy<T1>(int limit, int offset, object condition, string orderField = null, string ascOrDesc = "ASC")
             where T1 : class
        {
            return Select<T1>(limit, offset, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public List<T1> SelectWithOrderBy<T1>(int limit, object condition, string orderField = null, string ascOrDesc = "ASC")
             where T1 : class
        {
            return Select<T1>(limit, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public List<T1> SelectWithOrderBy<T1>(object condition, string orderField = null, string ascOrDesc = "ASC")
             where T1 : class
        {
            return Select<T1>(ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }

        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T1> SelectPage<T1>(int page, int itemsPerPage, object condition)
            where T1 : class
        {
            return SelectPage<T1>(page, itemsPerPage, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public List<T1> SelectPageWithOrderBy<T1>(int page, int itemsPerPage, object condition, string orderField = null, string ascOrDesc = "ASC")
            where T1 : class
        {
            return SelectPage<T1>(page, itemsPerPage, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }


        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Page<T1> Page<T1>(int page, int itemsPerPage, object condition)
            where T1 : class
        {
            return Page<T1>(page, itemsPerPage, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Page<T1> PageWithOrderBy<T1>(int page, int itemsPerPage, object condition, string orderField = null, string ascOrDesc = "ASC")
            where T1 : class
        {
            return this.Where<T1>().Where(ConditionObjectToWhere(condition))
                   .IfSet(orderField).OrderBy(BuildOrderBy(orderField, ascOrDesc))
                   .Page(page, itemsPerPage);
        }


        /// <summary>
        /// 根据条件更新对象
        /// </summary>
        /// <param name="set"></param>
        /// <param name="condition">条件</param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        public int Update<T1>(object set, object condition, IEnumerable<string> ignoreFields = null) where T1 : class
        {
            return Update<T1>(ConditionObjectToUpdateSetWhere(set, condition, ignoreFields));
        }

        /// <summary>
        /// 根据条件从数据库中删除对象
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int Delete<T1>(object condition) where T1 : class
        {
            return Delete<T1>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询个数
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int Count<T1>(object condition) where T1 : class
        {
            return Count<T1>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件判断是否存在
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public bool Exists<T1>(object condition) where T1 : class
        {
            if (condition.GetType().IsClass) {
                return Exists<T1>(ConditionObjectToWhere(condition));
            } else {
                var pd = PocoData.ForType(typeof(T1));
                var table = _provider.GetTableName(pd);
                var pk = _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);
                var sql = $"SELECT COUNT(*) FROM {table} WHERE {pk}=@0";

                var args = new object[] { condition };
                return GetDatabase().ExecuteScalar<int>(sql, args) > 0;
            }
        }

        #endregion

#if !NET40
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T1> FirstAsync<T1>(int condition) where T1 : class
        {
            return SingleByIdAsync<T1>(condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T1> FirstAsync<T1>(long condition) where T1 : class
        {
            return SingleByIdAsync<T1>(condition);
        }


        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T1> FirstAsync<T1>(object condition) where T1 : class
        {
            return FirstAsync<T1>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T1> FirstOrDefaultAsync<T1>(int condition) where T1 : class
        {
            return SingleOrDefaultByIdAsync<T1>(condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T1> FirstOrDefaultAsync<T1>(long condition) where T1 : class
        {
            return SingleOrDefaultByIdAsync<T1>(condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T1> FirstOrDefaultAsync<T1>(object condition) where T1 : class
        {
            return FirstOrDefaultAsync<T1>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T1> SingleAsync<T1>(int condition) where T1 : class
        {
            return SingleByIdAsync<T1>(condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T1> SingleAsync<T1>(long condition) where T1 : class
        {
            return SingleByIdAsync<T1>(condition);
        }


        /// <summary>
        /// 根据条件查询唯一项，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T1> SingleAsync<T1>(object condition) where T1 : class
        {
            return SingleAsync<T1>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T1> SingleOrDefaultAsync<T1>(int condition) where T1 : class
        {
            return SingleOrDefaultByIdAsync<T1>(condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T1> SingleOrDefaultAsync<T1>(long condition) where T1 : class
        {
            return SingleOrDefaultByIdAsync<T1>(condition);
        }

        /// <summary>
        /// 根据条件查询唯一项，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T1> SingleOrDefaultAsync<T1>(object condition) where T1 : class
        {
            return SingleOrDefaultAsync<T1>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T1>> SelectAsync<T1>(int limit, int offset, object condition) where T1 : class
        {
            return SelectAsync<T1>(limit, offset, ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T1>> SelectAsync<T1>(int limit, object condition) where T1 : class
        {
            return SelectAsync<T1>(limit, ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T1>> SelectAsync<T1>(object condition) where T1 : class
        {
            return SelectAsync<T1>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Task<List<T1>> SelectWithOrderByAsync<T1>(int limit, int offset, object condition, string orderField = null, string ascOrDesc = "ASC") where T1 : class
        {
            return SelectAsync<T1>(limit, offset, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Task<List<T1>> SelectWithOrderByAsync<T1>(int limit, object condition, string orderField = null, string ascOrDesc = "ASC") where T1 : class
        {
            return SelectAsync<T1>(limit, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }
        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Task<List<T1>> SelectWithOrderByAsync<T1>(object condition, string orderField = null, string ascOrDesc = "ASC") where T1 : class
        {
            return SelectAsync<T1>(ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }

        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T1>> SelectPageAsync<T1>(int page, int itemsPerPage, object condition)
            where T1 : class
        {
            return SelectPageAsync<T1>(page, itemsPerPage, ConditionObjectToWhere(condition));
        }

        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Task<List<T1>> SelectPageWithOrderByAsync<T1>(int page, int itemsPerPage, object condition, string orderField = null, string ascOrDesc = "ASC")
            where T1 : class
        {
            return this.Where<T1>().Where(ConditionObjectToWhere(condition))
                   .IfSet(orderField).OrderBy(BuildOrderBy(orderField, ascOrDesc))
                   .SelectPageAsync(page, itemsPerPage);
        }


        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<Page<T1>> PageAsync<T1>(int page, int itemsPerPage, object condition)
            where T1 : class
        {
            return PageAsync<T1>(page, itemsPerPage, ConditionObjectToWhere(condition));
        }

        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Task<Page<T1>> PageWithOrderByAsync<T1>(int page, int itemsPerPage, object condition, string orderField = null, string ascOrDesc = "ASC")
             where T1 : class
        {
            return this.Where<T1>().Where(ConditionObjectToWhere(condition))
                       .IfSet(orderField).OrderBy(BuildOrderBy(orderField, ascOrDesc))
                       .PageAsync(page, itemsPerPage);
        }



        /// <summary>
        /// 根据条件更新对象
        /// </summary>
        /// <param name="set"></param>
        /// <param name="condition">条件</param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync<T1>(object set, object condition, IEnumerable<string> ignoreFields = null) where T1 : class
        {
            return UpdateAsync<T1>(ConditionObjectToUpdateSetWhere(set, condition, ignoreFields));
        }

        /// <summary>
        /// 根据条件从数据库中删除对象 
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<int> DeleteAsync<T1>(object condition) where T1 : class
        {
            return DeleteAsync<T1>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询个数，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<int> CountAsync<T1>(object condition) where T1 : class
        {
            return CountAsync<T1>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件是判断否存在，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync<T1>(object condition) where T1 : class
        {
            if (condition.GetType().IsClass) {
                return await ExistsAsync<T1>(ConditionObjectToWhere(condition));
            } else {
                var pd = PocoData.ForType(typeof(T1));
                var table = _provider.GetTableName(pd);
                var pk = _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);
                var sql = $"SELECT COUNT(*) FROM {table} WHERE {pk}=@0";

                var args = new object[] { condition };
                return await GetDatabase().ExecuteScalarAsync<int>(sql, args) > 0;
            }
        }

#endif
        public string GetTableName(Type type)
        {
            var pd = PetaPoco.Core.PocoData.ForType(type);
            return this._provider.GetTableName(pd);
        }
        public string GetTableName<T>() where T:class
        {
            return GetTableName(typeof(T));
        }

        private string BuildOrderBy(string orderField, string ascOrDesc)
        {
            if (orderField == null) {
                return "";
            }
            ascOrDesc = ascOrDesc.ToUpper();
            if (ascOrDesc != "DESC") {
                ascOrDesc = "ASC";
            }
            return " ORDER BY " + orderField + " " + ascOrDesc;
        }

        private string ConditionObjectToWhere(object condition)
        {
            if (condition == null) { return ""; }
            if (condition.GetType() == typeof(string)) { return (string)condition; }


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("WHERE ");
            ObjectToSql(stringBuilder, condition, " AND ", null);
            return stringBuilder.ToString();
        }

        private string ConditionObjectToUpdateSetWhere(object set, object condition, IEnumerable<string> ignoreFields)
        {
            if (set == null) { throw new ArgumentException("set is  null object!"); }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SET ");
            ObjectToSql(stringBuilder, set, ",", ignoreFields);
            if (condition != null) {
                if (condition.GetType() == typeof(string)) {
                    var str = ((string)condition).Trim();
                    if (str.StartsWith("WHERE ", StringComparison.CurrentCultureIgnoreCase) == false) {
                        stringBuilder.Append(" WHERE");
                    }
                    stringBuilder.Append(" ");
                    stringBuilder.Append(str);
                    return stringBuilder.ToString();
                }
                stringBuilder.Append(" WHERE ");
                ObjectToSql(stringBuilder, condition, " AND ", null);
            }
            return stringBuilder.ToString();
        }

        private void ObjectToSql(StringBuilder stringBuilder, object condition, string middelStr, IEnumerable<string> ignoreFields)
        {
            if (condition is IEnumerable) { throw new ArgumentException("condition is IEnumerable object!"); }
            bool hasColumn = false;

            var type = condition.GetType();
            var pis = type.GetProperties();
            for (int i = 0; i < pis.Length; i++) {
                var pi = pis[i];
                if (ignoreFields != null) {
                    if (ignoreFields.Any(q => string.Equals(q, pi.Name, StringComparison.CurrentCultureIgnoreCase))) {
                        continue;
                    }
                }
                if (hasColumn == false) {
                    hasColumn = true;
                } else {
                    stringBuilder.Append(middelStr);
                }

                var pt = pi.PropertyType;
                var value = pi.GetGetMethod().Invoke(condition, null);
                if (middelStr == " AND ") {
                    if (value == null) {
                        stringBuilder.Append($"[{pi.Name}] is Null");
                    } else {
                        if (value is IEnumerable && !(value is string)) {
                            List<object> objs = new List<object>();
                            foreach (var item in (IEnumerable)value) { objs.Add(item); }

                            if (objs.Count == 0) {
                                stringBuilder.Append($"1=2");
                            } else if (objs.Count == 1) {
                                stringBuilder.Append($"[{pi.Name}]=");
                                stringBuilder.Append(EscapeParam(objs[0]));
                            } else {
                                stringBuilder.Append($"[{pi.Name}] in (");
                                for (int j = 0; j < objs.Count; j++) {
                                    if (j > 0) { stringBuilder.Append(","); }
                                    stringBuilder.Append(EscapeParam(objs[j]));
                                }
                                stringBuilder.Append($")");
                            }
                        } else {
                            stringBuilder.Append($"[{pi.Name}]=");
                            stringBuilder.Append(EscapeParam(value));
                        }
                    }
                } else {
                    stringBuilder.Append($"[{pi.Name}]=");
                    stringBuilder.Append(EscapeParam(value));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string EscapeParam(object value)
        {
            if (object.Equals(value, null)) return "NULL";

            var fieldType = value.GetType();
            if (fieldType.IsEnum) {
                if (EnumHelper.UseEnumString(fieldType)) {
                    var txt = (value.ToString()).ToEscapeParam();
                    return "'" + txt + "'";
                }
                return $"'{Convert.ToInt64(value)}'";

                //var isEnumFlags = fieldType.IsEnum;
                //if (!isEnumFlags && Int64.TryParse(value.ToString(), out long enumValue)) {
                //    value = Enum.ToObject(fieldType, enumValue).ToString();
                //}
                //var enumString = value.ToString();
                //return !isEnumFlags ? "'" + enumString.Trim('"') + "'" : enumString;
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
                var txt = (value.ToString()).ToEscapeParam();
                return "'" + txt + "'";
            }
            if (fieldType == typeof(DateTime)) return "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
            if (fieldType == typeof(TimeSpan)) return ((TimeSpan)value).Ticks.ToString(CultureInfo.InvariantCulture);
            if (fieldType == typeof(byte[])) {
                var txt = Encoding.BigEndianUnicode.GetString((byte[])value);
                return "X'" + txt + "'";
            }
            return "'" + value.ToString() + "'";
        }

    }
}
