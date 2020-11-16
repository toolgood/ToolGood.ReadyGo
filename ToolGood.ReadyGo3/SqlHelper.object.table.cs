using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3
{
    partial class SqlHelper
    {


        #region Single PK

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
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Page<T> PageTableWithOrderBy<T>(string table, int page, int itemsPerPage, object condition, string orderField = null, string ascOrDesc = "ASC")
            where T : class
        {
            return this.WhereTable<T>(table).Where(ConditionObjectToWhere(condition))
                   .IfSet(orderField).OrderBy(BuildOrderBy(orderField, ascOrDesc))
                   .Page(page, itemsPerPage);
        }


 

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


        /// <summary>
        /// 根据条件查询个数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int CountTable(string table, object condition)
        {
            return CountTable(table, ConditionObjectToWhere(condition));
        }


        /// <summary>
        /// 根据条件判断是否存在
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public bool ExistsTable(string table, object condition)
        {
            return CountTable(table, ConditionObjectToWhere(condition)) > 0;
        }





#if !NET40



        #region FirstAsync PK

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
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstTableAsync<T>(string table, object condition) where T : class
        {
            return FirstTableAsync<T>(table, ConditionObjectToWhere(condition));
        }


        #region FirstOrDefaultAsync PK

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
        public Task<T> FirstOrDefaultTableAsync<T>(string table, object condition) where T : class
        {
            return FirstOrDefaultTableAsync<T>(table, ConditionObjectToWhere(condition));
        }

        #region SingleAsync PK

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
        /// <param name="table"></param>
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
        public Task<List<T>> SelectTableAsync<T>(string table, object condition) where T : class
        {
            return SelectTableAsync<T>(table, ConditionObjectToWhere(condition));
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
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Task<List<T>> SelectPageTableWithOrderByAsync<T>(string table, int page, int itemsPerPage, object condition, string orderField = null, string ascOrDesc = "ASC")
            where T : class
        {
            return this.WhereTable<T>(table).Where(ConditionObjectToWhere(condition))
                   .IfSet(orderField).OrderBy(BuildOrderBy(orderField, ascOrDesc))
                   .SelectPageAsync(page, itemsPerPage);
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
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="ascOrDesc">升序或降序</param>
        /// <returns></returns>
        public Task<Page<T>> PageWithTableOrderByAsync<T>(string table, int page, int itemsPerPage, object condition, string orderField = null, string ascOrDesc = "ASC")
             where T : class
        {
            return this.WhereTable<T>(table).Where(ConditionObjectToWhere(condition))
                       .IfSet(orderField).OrderBy(BuildOrderBy(orderField, ascOrDesc))
                       .PageAsync(page, itemsPerPage);
        }
 
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

 
        /// <summary>
        /// 根据条件查询个数，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<int> CountTableAsync(string table, object condition)
        {
            return CountTableAsync(table, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件是判断否存在，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public async Task<bool> ExistsTableAsync(string table, object condition)
        {
            return await CountTableAsync(table, ConditionObjectToWhere(condition))>0;
 
        }

#endif




    }
}
