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
        public T Single_Table<T>(string table, int condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Single_Table<T>(string table, uint condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T Single_Table<T>(string table, long condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        public T Single_Table<T>(string table, ulong condition) where T : class
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
        public T Single_Table<T>(string table, object condition) where T : class
        {
            return Single_Table<T>(table, ConditionObjectToWhere(condition));
        }

        #region SingleOrDefault PK
        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleOrDefault_Table<T>(string table, int condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleOrDefault_Table<T>(string table, uint condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleOrDefault_Table<T>(string table, long condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询唯一项
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T SingleOrDefault_Table<T>(string table, ulong condition) where T : class
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
        public T SingleOrDefault_Table<T>(string table, object condition) where T : class
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
        public T First_Table<T>(string table, int condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }


        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T First_Table<T>(string table, uint condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T First_Table<T>(string table, long condition) where T : class
        {
            return SingleTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">主键</param>
        /// <returns></returns>
        public T First_Table<T>(string table, ulong condition) where T : class
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
        public T First_Table<T>(string table, object condition) where T : class
        {
            return First_Table<T>(table, ConditionObjectToWhere(condition));
        }

        #region FirstOrDefault PK

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefault_Table<T>(string table, int condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefault_Table<T>(string table, uint condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefault_Table<T>(string table, long condition) where T : class
        {
            return SingleOrDefaultTableById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T FirstOrDefault_Table<T>(string table, ulong condition) where T : class
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
        public T FirstOrDefault_Table<T>(string table, object condition) where T : class
        {
            return FirstOrDefault_Table<T>(table, ConditionObjectToWhere(condition));
        }


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> Select_Table<T>(string table, int limit, int offset, object condition) where T : class
        {
            return Select_Table<T>(table, limit, offset, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> Select_Table<T>(string table, int limit, object condition) where T : class
        {
            return Select_Table<T>(table, limit, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> Select_Table<T>(string table, object condition) where T : class
        {
            return Select_Table<T>(table, ConditionObjectToWhere(condition));
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
            return Select_Table<T>(table, limit, offset, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
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
            return Select_Table<T>(table, limit, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
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
            return Select_Table<T>(table, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }

        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> SelectPage_Table<T>(string table, int page, int itemsPerPage, object condition)
            where T : class
        {
            return SelectPage_Table<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition));
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
            return SelectPage_Table<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }

        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Page<T> Page_Table<T>(string table, int page, int itemsPerPage, object condition)
            where T : class
        {
            return Page_Table<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition));
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
            return this.Where_Table<T>(table).Where(ConditionObjectToWhere(condition))
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
        public int Update_Table(string table, object set, object condition, IEnumerable<string> ignoreFields = null)
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
        public int Delete_Table(string table, object condition)
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
        public int Count_Table(string table, object condition)
        {
            return Count_Table(table, ConditionObjectToWhere(condition));
        }


        /// <summary>
        /// 根据条件判断是否存在
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public bool Exists_Table(string table, object condition)
        {
            return Count_Table(table, ConditionObjectToWhere(condition)) > 0;
        }





#if !NET40



        #region FirstAsync PK

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstAsync_Table<T>(string table, int condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }


        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstAsync_Table<T>(string table, uint condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstAsync_Table<T>(string table, long condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstAsync_Table<T>(string table, ulong condition) where T : class
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
        public Task<T> FirstAsync_Table<T>(string table, object condition) where T : class
        {
            return FirstAsync_Table<T>(table, ConditionObjectToWhere(condition));
        }


        #region FirstOrDefaultAsync PK

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync_Table<T>(string table, int condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync_Table<T>(string table, uint condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync_Table<T>(string table, long condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync_Table<T>(string table, ulong condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        #endregion

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync_Table<T>(string table, object condition) where T : class
        {
            return FirstOrDefaultAsync_Table<T>(table, ConditionObjectToWhere(condition));
        }

        #region SingleAsync PK

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleAsync_Table<T>(string table, int condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }


        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleAsync_Table<T>(string table, uint condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleAsync_Table<T>(string table, long condition) where T : class
        {
            return SingleTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleAsync_Table<T>(string table, ulong condition) where T : class
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
        public Task<T> SingleAsync_Table<T>(string table, object condition) where T : class
        {
            return SingleAsync_Table<T>(table, ConditionObjectToWhere(condition));
        }


        #region SingleOrDefaultAsync PK

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync_Table<T>(string table, int condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync_Table<T>(string table, uint condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync_Table<T>(string table, long condition) where T : class
        {
            return SingleOrDefaultTableByIdAsync<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync_Table<T>(string table, ulong condition) where T : class
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
        public Task<T> SingleOrDefaultAsync_Table<T>(string table, object condition) where T : class
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
        public Task<List<T>> SelectAsync_Table<T>(string table, int limit, int offset, object condition) where T : class
        {
            return SelectAsync_Table<T>(table, limit, offset, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync_Table<T>(string table, int limit, object condition) where T : class
        {
            return SelectAsync_Table<T>(table, limit, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync_Table<T>(string table, object condition) where T : class
        {
            return SelectAsync_Table<T>(table, ConditionObjectToWhere(condition));
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
            return SelectAsync_Table<T>(table, limit, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
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
            return SelectAsync_Table<T>(table, ConditionObjectToWhere(condition) + BuildOrderBy(orderField, ascOrDesc));
        }

        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> SelectPageAsync_Table<T>(string table, int page, int itemsPerPage, object condition)
            where T : class
        {
            return SelectPageAsync_Table<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition));
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
            return this.Where_Table<T>(table).Where(ConditionObjectToWhere(condition))
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
        public Task<Page<T>> PageAsync_Table<T>(string table, int page, int itemsPerPage, object condition)
            where T : class
        {
            return PageAsync_Table<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition));
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
            return this.Where_Table<T>(table).Where(ConditionObjectToWhere(condition))
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
        public Task<int> UpdateAsync_Table(string table, object set, object condition, IEnumerable<string> ignoreFields = null)
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
        public Task<int> DeleteAsync_Table(string table, object condition)
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
        public Task<int> CountAsync_Table(string table, object condition)
        {
            return CountAsync_Table(table, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件是判断否存在，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync_Table(string table, object condition)
        {
            return await CountAsync_Table(table, ConditionObjectToWhere(condition)) > 0;

        }

#endif




    }
}
