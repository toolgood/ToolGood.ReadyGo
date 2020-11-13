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
        /// <summary>
        /// 更新表
        /// </summary>
        /// <param name="table"></param>
        /// <param name="set"></param>
        /// <param name="condition"></param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        public int UpdateTable(string table, object set, object condition, IEnumerable<string> ignoreFields = null)
        {
            var tbn = _provider.GetTableName(table);
            return Update("UPDATE " + tbn + " " + ConditionObjectToUpdateSetWhere(set, condition, ignoreFields));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int DeleteTable(string table, object condition)
        {
            if (null == condition) {
                throw new ArgumentNullException(nameof(condition));
            }
            var tbn = _provider.GetTableName(table);
            return Delete("DELETE FROM " + tbn + " " + ConditionObjectToWhere(condition));
        }

        #region Select Update
        #region Single PK
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Single<T>(int condition) where T : class
        {
            return SingleById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T SingleTable<T>(string table, int condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Single<T>(uint condition) where T : class
        {
            return SingleById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T SingleTable<T>(string table, uint condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T Single<T>(long condition) where T : class
        {
            return SingleById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleTable<T>(string table, long condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">主键</param>
        public T Single<T>(ulong condition) where T : class
        {
            return SingleById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        public T SingleTable<T>(string table, ulong condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }

        #endregion
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Single<T>(object condition) where T : class
        {
            return Single<T>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T SingleTable<T>(string table, object condition) where T : class
        {
            return SingleTable<T>(table, ConditionObjectToWhere(condition));
        }

        #region SingleOrDefault PK

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleOrDefault<T>(int condition) where T : class
        {
            return SingleOrDefaultById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleOrDefaultTable<T>(string table, int condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }


        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleOrDefault<T>(uint condition) where T : class
        {
            return SingleOrDefaultById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleOrDefaultTable<T>(string table, uint condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleOrDefault<T>(long condition) where T : class
        {
            return SingleOrDefaultById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleOrDefaultTable<T>(string table, long condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }


        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        public T SingleOrDefault<T>(ulong condition) where T : class
        {
            return SingleOrDefaultById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleOrDefaultTable<T>(string table, ulong condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }
        #endregion

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T SingleOrDefault<T>(object condition) where T : class
        {
            return SingleOrDefault<T>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleOrDefaultTable<T>(string table, object condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }

        #region First PK
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T First<T>(int condition) where T : class
        {
            return SingleById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T FirstTable<T>(string table, int condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T First<T>(uint condition) where T : class
        {
            return SingleById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T FirstTable<T>(string table, uint condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }


        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T First<T>(long condition) where T : class
        {
            return SingleById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T FirstTable<T>(string table, long condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T First<T>(ulong condition) where T : class
        {
            return SingleById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T FirstTable<T>(string table, ulong condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }

        #endregion
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T First<T>(object condition) where T : class
        {
            return First<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T FirstTable<T>(string table, object condition) where T : class
        {
            return FirstTable<T>(table, ConditionObjectToWhere(condition));
        }

        #region FirstOrDefault PK
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefault<T>(int condition) where T : class
        {
            return SingleOrDefaultById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefaultTable<T>(string table, int condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }


        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefault<T>(uint condition) where T : class
        {
            return SingleOrDefaultById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefaultTable<T>(string table, uint condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefault<T>(long condition) where T : class
        {
            return SingleOrDefaultById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefaultTable<T>(string table, long condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefault<T>(ulong condition) where T : class
        {
            return SingleOrDefaultById<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefaultTable<T>(string table, ulong condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }

        #endregion
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefault<T>(object condition) where T : class
        {
            return FirstOrDefault<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefaultTable<T>(string table, object condition) where T : class
        {
            return FirstOrDefaultTable<T>(table, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> Select<T>(int limit, int offset, object condition) where T : class
        {
            return Select<T>(limit, offset, ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> SelectTable<T>(string table, int limit, int offset, object condition) where T : class
        {
            return SelectTable<T>(table, limit, offset, ConditionObjectToWhere(condition));
        }


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> Select<T>(int limit, object condition) where T : class
        {
            return Select<T>(limit, ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> SelectTable<T>(string table, int limit, object condition) where T : class
        {
            return SelectTable<T>(table, limit, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> Select<T>(object condition) where T : class
        {
            return Select<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> SelectTable<T>(string table, object condition) where T : class
        {
            return SelectTable<T>(table, ConditionObjectToWhere(condition));
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
        public List<T> SelectWithOrderBy<T>(int limit, int offset, object condition, string orderField = null, string ascOrDesc = "ASC")
             where T : class
        {
            return Select<T>(limit, offset, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public List<T> SelectTableWithOrderBy<T>(string table, int limit, int offset, object condition, string orderField = null, string ascOrDesc = "ASC")
             where T : class
        {
            return SelectTable<T>(table, limit, offset, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public List<T> SelectWithOrderBy<T>(int limit, object condition, string orderField = null, string ascOrDesc = "ASC")
             where T : class
        {
            return Select<T>(limit, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public List<T> SelectTableWithOrderBy<T>(string table, int limit, object condition, string orderField = null, string ascOrDesc = "ASC")
             where T : class
        {
            return SelectTable<T>(table, limit, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public List<T> SelectWithOrderBy<T>(object condition, string orderField = null, string ascOrDesc = "ASC")
             where T : class
        {
            return Select<T>(ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public List<T> SelectTableWithOrderBy<T>(string table, object condition, string orderField = null, string ascOrDesc = "ASC")
             where T : class
        {
            return SelectTable<T>(table, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }


        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> SelectPage<T>(int page, int itemsPerPage, object condition)
            where T : class
        {
            return SelectPage<T>(page, itemsPerPage, ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> SelectPageTable<T>(string table, int page, int itemsPerPage, object condition)
            where T : class
        {
            return SelectPageTable<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition));
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
        public List<T> SelectPageWithOrderBy<T>(int page, int itemsPerPage, object condition, string orderField = null, string ascOrDesc = "ASC")
            where T : class
        {
            return SelectPage<T>(page, itemsPerPage, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }
        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public List<T> SelectPageTableWithOrderBy<T>(string table, int page, int itemsPerPage, object condition, string orderField = null, string ascOrDesc = "ASC")
            where T : class
        {
            return SelectPageTable<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }



        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Page<T> Page<T>(int page, int itemsPerPage, object condition)
            where T : class
        {
            return Page<T>(page, itemsPerPage, ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Page<T> PageTable<T>(string table, int page, int itemsPerPage, object condition)
            where T : class
        {
            return PageTable<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition));
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
        public Page<T> PageWithOrderBy<T>(int page, int itemsPerPage, object condition, string orderField = null, string ascOrDesc = "ASC")
            where T : class
        {
            return this.Where<T>().Where(ConditionObjectToWhere(condition))
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
        public int Update<T>(object set, object condition, IEnumerable<string> ignoreFields = null) where T : class
        {
            return Update<T>(ConditionObjectToUpdateSetWhere(set, condition, ignoreFields));
        }
        /// <summary>
        /// 根据条件更新对象
        /// </summary>
        /// <param name="table"></param>
        /// <param name="set"></param>
        /// <param name="condition">条件</param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        public int UpdateTable<T>(string table, object set, object condition, IEnumerable<string> ignoreFields = null) where T : class
        {
            return UpdateTable<T>(table, ConditionObjectToUpdateSetWhere(set, condition, ignoreFields));
        }


        /// <summary>
        /// 根据条件从数据库中删除对象
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int Delete<T>(object condition) where T : class
        {
            return Delete<T>(ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件从数据库中删除对象
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int DeleteTable<T>(string table, object condition) where T : class
        {
            return DeleteTable<T>(table, ConditionObjectToWhere(condition));
        }


        /// <summary>
        /// 根据条件查询个数
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int Count<T>(object condition) where T : class
        {
            return Count<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询个数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int CountTable<T>(string table, object condition) where T : class
        {
            return CountTable<T>(table, ConditionObjectToWhere(condition));
        }


        /// <summary>
        /// 根据条件判断是否存在
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public bool Exists<T>(object condition) where T : class
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
        /// <summary>
        /// 根据条件判断是否存在
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public bool ExistsTable<T>(string table, object condition) where T : class
        {
            if (condition.GetType().IsClass) {
                return Exists<T>(ConditionObjectToWhere(condition));
            } else {
                var pd = PocoData.ForType(typeof(T));
                var pk = _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);
                var sql = $"SELECT COUNT(*) FROM {table} WHERE {pk}=@0";

                var args = new object[] { condition };
                return GetDatabase().ExecuteScalar<int>(sql, args) > 0;
            }
        }

        #endregion

#if !NET40

        /// <summary>
        /// 更新表
        /// </summary>
        /// <param name="table"></param>
        /// <param name="set"></param>
        /// <param name="condition"></param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        public Task<int> UpdateTableAsync(string table, object set, object condition, IEnumerable<string> ignoreFields = null)
        {
            var tbn = _provider.GetTableName(table);
            return UpdateAsync("UPDATE " + tbn + " " + ConditionObjectToUpdateSetWhere(set, condition, ignoreFields));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Task<int> DeleteTableAsync(string table, object condition)
        {
            if (null == condition) {
                throw new ArgumentNullException(nameof(condition));
            }
            var tbn = _provider.GetTableName(table);
            return DeleteAsync("DELETE FROM " + tbn + " " + ConditionObjectToWhere(condition));
        }


        #region FirstAsync PK
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstAsync<T>(int condition) where T : class
        {
            return SingleByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstTableAsync<T>(string table, int condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstAsync<T>(uint condition) where T : class
        {
            return SingleByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstTableAsync<T>(string table, uint condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstAsync<T>(long condition) where T : class
        {
            return SingleByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstTableAsync<T>(string table, long condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstAsync<T>(ulong condition) where T : class
        {
            return SingleByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstTableAsync<T>(string table, ulong condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }

        #endregion

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstAsync<T>(object condition) where T : class
        {
            return FirstAsync<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstAsync<T>(string table, object condition) where T : class
        {
            return FirstAsync<T>(table, ConditionObjectToWhere(condition));
        }


        #region FirstOrDefaultAsync PK
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync<T>(int condition) where T : class
        {
            return SingleOrDefaultByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultTableAsync<T>(string table, int condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }


        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync<T>(uint condition) where T : class
        {
            return SingleOrDefaultByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultTableAsync<T>(string table, uint condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync<T>(long condition) where T : class
        {
            return SingleOrDefaultByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultTableAsync<T>(string table, long condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync<T>(ulong condition) where T : class
        {
            return SingleOrDefaultByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultTableAsync<T>(string table, ulong condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        #endregion

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync<T>(object condition) where T : class
        {
            return FirstOrDefaultAsync<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultTableAsync<T>(string table, object condition) where T : class
        {
            return FirstOrDefaultTableAsync<T>(table, ConditionObjectToWhere(condition));
        }

        #region SingleAsync PK
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleAsync<T>(int condition) where T : class
        {
            return SingleByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleTableAsync<T>(string table, int condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }


        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleAsync<T>(uint condition) where T : class
        {
            return SingleByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleTableAsync<T>(string table, uint condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }


        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleAsync<T>(long condition) where T : class
        {
            return SingleByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleTableAsync<T>(string table, long condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleAsync<T>(ulong condition) where T : class
        {
            return SingleByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleTableAsync<T>(string table, ulong condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }

        #endregion

        /// <summary>
        /// 根据条件查询唯一项，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleAsync<T>(object condition) where T : class
        {
            return SingleAsync<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询唯一项，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleTableAsync<T>(string table, object condition) where T : class
        {
            return SingleTableAsync<T>(table, ConditionObjectToWhere(condition));
        }


        #region SingleOrDefaultAsync PK
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync<T>(int condition) where T : class
        {
            return SingleOrDefaultByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultTableAsync<T>(string table, int condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }


        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync<T>(uint condition) where T : class
        {
            return SingleOrDefaultByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultTableAsync<T>(string table, uint condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync<T>(long condition) where T : class
        {
            return SingleOrDefaultByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultTableAsync<T>(string table, long condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync<T>(ulong condition) where T : class
        {
            return SingleOrDefaultByIdAsync<T>(condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultTableAsync<T>(string table, ulong condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        #endregion

        /// <summary>
        /// 根据条件查询唯一项，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync<T>(object condition) where T : class
        {
            return SingleOrDefaultAsync<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询唯一项，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultTableAsync<T>(string table, object condition) where T : class
        {
            return SingleOrDefaultAsync<T>(table, ConditionObjectToWhere(condition));
        }


        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync<T>(int limit, int offset, object condition) where T : class
        {
            return SelectAsync<T>(limit, offset, ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> SelectTableAsync<T>(string table, int limit, int offset, object condition) where T : class
        {
            return SelectTableAsync<T>(table, limit, offset, ConditionObjectToWhere(condition));
        }


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync<T>(int limit, object condition) where T : class
        {
            return SelectAsync<T>(limit, ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> SelectTableAsync<T>(string table, int limit, object condition) where T : class
        {
            return SelectTableAsync<T>(table, limit, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync<T>(object condition) where T : class
        {
            return SelectAsync<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> SelectTableAsync<T>(string table, object condition) where T : class
        {
            return SelectTableAsync<T>(table, ConditionObjectToWhere(condition));
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
        public Task<List<T>> SelectWithOrderByAsync<T>(int limit, int offset, object condition, string orderField = null, string ascOrDesc = "ASC") where T : class
        {
            return SelectAsync<T>(limit, offset, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }
        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Task<List<T>> SelectTableWithOrderByAsync<T>(string table, int limit, int offset, object condition, string orderField = null, string ascOrDesc = "ASC") where T : class
        {
            return SelectAsync<T>(table, limit, offset, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Task<List<T>> SelectWithOrderByAsync<T>(int limit, object condition, string orderField = null, string ascOrDesc = "ASC") where T : class
        {
            return SelectAsync<T>(limit, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Task<List<T>> SelectTableWithOrderByAsync<T>(string table, int limit, object condition, string orderField = null, string ascOrDesc = "ASC") where T : class
        {
            return SelectTableAsync<T>(table, limit, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Task<List<T>> SelectWithOrderByAsync<T>(object condition, string orderField = null, string ascOrDesc = "ASC") where T : class
        {
            return SelectAsync<T>(ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }
        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Task<List<T>> SelectWithOrderByAsync<T>(string table, object condition, string orderField = null, string ascOrDesc = "ASC") where T : class
        {
            return SelectTableAsync<T>(table, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }

        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> SelectPageAsync<T>(int page, int itemsPerPage, object condition)
            where T : class
        {
            return SelectPageAsync<T>(page, itemsPerPage, ConditionObjectToWhere(condition));
        }
        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> SelectPageTableAsync<T>(string table, int page, int itemsPerPage, object condition)
            where T : class
        {
            return SelectPageTableAsync<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition));
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
        public Task<List<T>> SelectPageWithOrderByAsync<T>(int page, int itemsPerPage, object condition, string orderField = null, string ascOrDesc = "ASC")
            where T : class
        {
            return this.Where<T>().Where(ConditionObjectToWhere(condition))
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
        public Task<Page<T>> PageAsync<T>(int page, int itemsPerPage, object condition)
            where T : class
        {
            return PageAsync<T>(page, itemsPerPage, ConditionObjectToWhere(condition));
        }
        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<Page<T>> PageTableAsync<T>(string table, int page, int itemsPerPage, object condition)
            where T : class
        {
            return PageTableAsync<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition));
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
        public Task<Page<T>> PageWithOrderByAsync<T>(int page, int itemsPerPage, object condition, string orderField = null, string ascOrDesc = "ASC")
             where T : class
        {
            return this.Where<T>().Where(ConditionObjectToWhere(condition))
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
        public Task<int> UpdateAsync<T>(object set, object condition, IEnumerable<string> ignoreFields = null) where T : class
        {
            return UpdateAsync<T>(ConditionObjectToUpdateSetWhere(set, condition, ignoreFields));
        }
        /// <summary>
        /// 根据条件更新对象
        /// </summary>
        /// <param name="table"></param>
        /// <param name="set"></param>
        /// <param name="condition">条件</param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        public Task<int> UpdateTableAsync<T>(string table, object set, object condition, IEnumerable<string> ignoreFields = null) where T : class
        {
            return UpdateTableAsync<T>(table, ConditionObjectToUpdateSetWhere(set, condition, ignoreFields));
        }


        /// <summary>
        /// 根据条件从数据库中删除对象 
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<int> DeleteAsync<T>(object condition) where T : class
        {
            return DeleteAsync<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件从数据库中删除对象 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<int> DeleteTableAsync<T>(string table, object condition) where T : class
        {
            return DeleteTableAsync<T>(table, ConditionObjectToWhere(condition));
        }


        /// <summary>
        /// 根据条件查询个数，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<int> CountAsync<T>(object condition) where T : class
        {
            return CountAsync<T>(ConditionObjectToWhere(condition));
        }
        /// <summary>
        /// 根据条件查询个数，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<int> CountTableAsync<T>(string table, object condition) where T : class
        {
            return CountTableAsync<T>(table, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件是判断否存在，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync<T>(object condition) where T : class
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

        /// <summary>
        /// 根据条件是判断否存在，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public async Task<bool> ExistsTableAsync<T>(string table,object condition) where T : class
        {
            if (condition.GetType().IsClass) {
                return await ExistsAsync<T>(table, ConditionObjectToWhere(condition));
            } else {
                var pd = PocoData.ForType(typeof(T));
                var pk = _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey);
                var sql = $"SELECT COUNT(*) FROM {table} WHERE {pk}=@0";

                var args = new object[] { condition };
                return await GetDatabase().ExecuteScalarAsync<int>(sql, args) > 0;
            }
        }

#endif


        private string BuildOrderBy(string orderField, string ascOrDesc)
        {
            if (orderField == null) {
                return "";
            }
            if (!ascOrDesc.Equals("DESC", StringComparison.OrdinalIgnoreCase)) {
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
                var txt = BitConverter.ToString((byte[])value).Replace("-", "");
                return "X'" + txt + "'";
            }
            return "'" + value.ToString() + "'";
        }

    }
}
