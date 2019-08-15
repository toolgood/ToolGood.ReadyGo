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
        /// <param name="condition"></param>
        /// <returns></returns>
        public T Single<T>(object condition)
        {
            return Single<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public T SingleOrDefault<T>(object condition)
        {
            return SingleOrDefault<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public T First<T>(object condition)
        {
            return First<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public T FirstOrDefault<T>(object condition)
        {
            return FirstOrDefault<T>(ConditionObjectToWhere(condition));
        }


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<T1> Select<T1>(int limit, int offset, object condition)
        {
            if (null == condition && condition.GetType() == typeof(string)) {
                return Select<T1>(limit, offset, (string)condition);
            }
            return Select<T1>(limit, offset, ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<T1> Select<T1>(int limit, object condition)
        {
            if (null == condition && condition.GetType() == typeof(string)) {
                return Select<T1>(limit, (string)condition);
            }
            return Select<T1>(limit, ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<T1> Select<T1>(object condition)
        {
            if (null == condition && condition.GetType() == typeof(string)) {
                return Select<T1>((string)condition);
            }
            return Select<T1>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Page<T> Page<T>(int page, int itemsPerPage, object condition)
        {
            if (null != condition && typeof(string) == condition.GetType()) {
                return Page<T>(page, itemsPerPage, (string)condition, new object[0]);
            }
            return Page<T>(page, itemsPerPage, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件更新对象
        /// </summary>
        /// <param name="set"></param>
        /// <param name="condition"></param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        public int Update<T1>(object set, object condition, params string[] ignoreFields)
        {
            return Update<T1>(ConditionObjectToUpdateSetWhere(set, condition, ignoreFields));
        }

        /// <summary>
        /// 根据条件从数据库中删除对象
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int Delete<T>(object condition)
        {
            return Delete<T>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询个数
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int Count<T>(object condition)
        {
            return Count<T>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件判断是否存在
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool Exists<T>(object condition)
        {
            if (condition.GetType().IsClass) {
                return Exists<T>(ConditionObjectToWhere(condition));
            } else {
                var pd = PocoData.ForType(typeof(T));
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
        /// <param name="condition"></param>
        /// <returns></returns>
        public Task<T> FirstAsync<T>(object condition)
        {
            return FirstAsync<T>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync<T>(object condition)
        {
            return FirstOrDefaultAsync<T>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询唯一项，异步操作
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Task<T> SingleAsync<T>(object condition)
        {
            return SingleAsync<T>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询唯一项，异步操作
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync<T>(object condition)
        {
            return SingleOrDefaultAsync<T>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync<T>(int limit, int offset, object condition)
        {
            return SelectAsync<T>(limit, offset, ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync<T>(int limit, object condition)
        {
            return SelectAsync<T>(limit, ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync<T>(object condition)
        {
            return SelectAsync<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Task<Page<T>> PageAsync<T>(int page, int itemsPerPage, object condition)
        {
            if (null != condition && typeof(string) == condition.GetType()) {
                return PageAsync<T>(page, itemsPerPage, (string)condition, new object[0]);
            }
            return PageAsync<T>(page, itemsPerPage, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件更新对象
        /// </summary>
        /// <param name="set"></param>
        /// <param name="condition"></param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync<T1>(object set, object condition, params string[] ignoreFields)
        {
            return UpdateAsync<T1>(ConditionObjectToUpdateSetWhere(set, condition, ignoreFields));
        }

        /// <summary>
        /// 根据条件从数据库中删除对象 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync<T>(object condition)
        {
            return DeleteAsync<T>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询个数，异步操作
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Task<int> CountAsync<T>(object condition)
        {
            return CountAsync<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件是判断否存在，异步操作
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync<T>(object condition)
        {
            if (condition.GetType().IsClass) {
                return await ExistsAsync<T>(ConditionObjectToWhere(condition));
            } else {
                var pd = PocoData.ForType(typeof(T));
                var table = _provider.GetTableName(pd);
                var pk = _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);
                var sql = $"SELECT COUNT(*) FROM {table} WHERE {pk}=@0";

                var args = new object[] { condition };
                return await GetDatabase().ExecuteScalarAsync<int>(sql, args) > 0;
            }
        }

#endif

        private string ConditionObjectToWhere(object condition)
        {
            if (condition == null) {
                return "";
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("WHERE ");
            ObjectToSql(stringBuilder, condition, " AND ", null);
            return stringBuilder.ToString();
        }

        private string ConditionObjectToUpdateSetWhere(object set, object condition, string[] ignoreFields)
        {
            if (set == null) {
                throw new ArgumentException("set is  null object!");
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SET ");
            ObjectToSql(stringBuilder, set, ",", ignoreFields);
            if (condition != null) {
                stringBuilder.Append(" WHERE ");

                ObjectToSql(stringBuilder, condition, " AND ", null);
            }
            return stringBuilder.ToString();
        }

        private void ObjectToSql(StringBuilder stringBuilder, object condition, string middelStr, string[] ignoreFields)
        {
            if (condition is IEnumerable) { throw new ArgumentException("condition is IEnumerable object!"); }
            bool hasColumn = false;

            //if (condition is IUpdateChange) {
            //    var dict = ((IUpdateChange)condition).__GetChanges__();
            //    foreach (var item in dict) {
            //        if (ignoreFields != null) {
            //            if (ignoreFields.Any(q => string.Equals(q, item.Key, StringComparison.CurrentCultureIgnoreCase))) { continue; }
            //        }
            //        if (hasColumn == false) {
            //            hasColumn = true;
            //        } else {
            //            stringBuilder.Append(middelStr);
            //        }
            //        if (middelStr == " AND ") {
            //            if (item.Value == null) {
            //                stringBuilder.Append($"[{item.Key}] is Null");
            //            } else {
            //                stringBuilder.Append($"[{item.Key}]=");
            //                stringBuilder.Append(EscapeParam(item.Value));
            //            }
            //        } else {
            //            stringBuilder.Append($"[{item.Key}]=");
            //            stringBuilder.Append(EscapeParam(item.Value));
            //        }
            //    }
            //    return;
            //}


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
