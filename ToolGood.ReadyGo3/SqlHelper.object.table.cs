using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo3
{
    public partial class SqlHelper
    {
        #region FirstOrDefault PK

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Table_FirstOrDefault<T>(string table, int condition) where T : class
        {
            return Table_SingleOrDefaultById<T>(table, condition);
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Table_FirstOrDefault<T>(string table, int? condition) where T : class
        {
            return Table_SingleOrDefaultById<T>(table, condition ?? 0);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Table_FirstOrDefault<T>(string table, uint condition) where T : class
        {
            return Table_SingleOrDefaultById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Table_FirstOrDefault<T>(string table, uint? condition) where T : class
        {
            return Table_SingleOrDefaultById<T>(table, condition ?? 0);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Table_FirstOrDefault<T>(string table, long condition) where T : class
        {
            return Table_SingleOrDefaultById<T>(table, condition);
        }
        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Table_FirstOrDefault<T>(string table, long? condition) where T : class
        {
            return Table_SingleOrDefaultById<T>(table, condition ?? 0);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Table_FirstOrDefault<T>(string table, ulong condition) where T : class
        {
            return Table_SingleOrDefaultById<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Table_FirstOrDefault<T>(string table, ulong? condition) where T : class
        {
            return Table_SingleOrDefaultById<T>(table, condition ?? 0);
        }

        #endregion FirstOrDefault PK

        /// <summary>
        /// 根据条件查询第一个
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public T Table_FirstOrDefault<T>(string table, object condition) where T : class
        {
            return Table_FirstOrDefault<T>(table, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> Table_Select<T>(string table, int limit, int offset, object condition) where T : class
        {
            return Table_Select<T>(table, limit, offset, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> Table_Select<T>(string table, int limit, object condition) where T : class
        {
            return Table_Select<T>(table, limit, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> Table_Select<T>(string table, object condition) where T : class
        {
            return Table_Select<T>(table, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> Table_SelectPage<T>(string table, int page, int itemsPerPage, object condition)
            where T : class
        {
            return Table_SelectPage<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询页
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Page<T> Table_Page<T>(string table, int page, int itemsPerPage, object condition)
            where T : class
        {
            return Table_Page<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 更新表
        /// </summary>
        /// <param name="table"></param>
        /// <param name="set"></param>
        /// <param name="condition"></param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        public int Table_Update(string table, object set, object condition, IEnumerable<string> ignoreFields = null)
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
        public int Table_Delete(string table, object condition)
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
        public int Table_Count(string table, object condition)
        {
            return Table_Count(table, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件判断是否存在
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public bool Table_Exists(string table, object condition)
        {
            return Table_Count(table, ConditionObjectToWhere(condition)) > 0;
        }

        #region FirstOrDefault_Async PK

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> Table_FirstOrDefault_Async<T>(string table, int condition) where T : class
        {
            return Table_SingleOrDefaultById_Async<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> Table_FirstOrDefault_Async<T>(string table, int? condition) where T : class
        {
            return Table_SingleOrDefaultById_Async<T>(table, condition ?? 0);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> Table_FirstOrDefault_Async<T>(string table, uint condition) where T : class
        {
            return Table_SingleOrDefaultById_Async<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> Table_FirstOrDefault_Async<T>(string table, uint? condition) where T : class
        {
            return Table_SingleOrDefaultById_Async<T>(table, condition ?? 0);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> Table_FirstOrDefault_Async<T>(string table, long condition) where T : class
        {
            return Table_SingleOrDefaultById_Async<T>(table, condition);
        }
        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> Table_FirstOrDefault_Async<T>(string table, long? condition) where T : class
        {
            return Table_SingleOrDefaultById_Async<T>(table, condition ?? 0);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> Table_FirstOrDefault_Async<T>(string table, ulong condition) where T : class
        {
            return Table_SingleOrDefaultById_Async<T>(table, condition);
        }

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> Table_FirstOrDefault_Async<T>(string table, ulong? condition) where T : class
        {
            return Table_SingleOrDefaultById_Async<T>(table, condition ?? 0);
        }

        #endregion FirstOrDefault_Async PK

        /// <summary>
        /// 根据条件查询第一个，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<T> Table_FirstOrDefault_Async<T>(string table, object condition) where T : class
        {
            return Table_FirstOrDefault_Async<T>(table, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="offset">位移</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> Table_Select_Async<T>(string table, int limit, int offset, object condition) where T : class
        {
            return Table_Select_Async<T>(table, limit, offset, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="table"></param>
        /// <param name="limit">个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> Table_Select_Async<T>(string table, int limit, object condition) where T : class
        {
            return Table_Select_Async<T>(table, limit, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> Table_Select_Async<T>(string table, object condition) where T : class
        {
            return Table_Select_Async<T>(table, ConditionObjectToWhere(condition));
        }

        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<List<T>> Table_SelectPage_Async<T>(string table, int page, int itemsPerPage, object condition)
            where T : class
        {
            return Table_SelectPage_Async<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition));
        }

        /// <summary>
        ///  根据条件查询页，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="page">页数</param>
        /// <param name="itemsPerPage">每页个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<Page<T>> Table_Page_Async<T>(string table, int page, int itemsPerPage, object condition)
            where T : class
        {
            return Table_Page_Async<T>(table, page, itemsPerPage, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 更新表
        /// </summary>
        /// <param name="table"></param>
        /// <param name="set"></param>
        /// <param name="condition"></param>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        public Task<int> Table_Update_Async(string table, object set, object condition, IEnumerable<string> ignoreFields = null)
        {
            var tbn = _provider.GetTableName(table);
            return Update_Async("UPDATE " + tbn + " " + ConditionObjectToUpdateSetWhere(set, condition, ignoreFields));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Task<int> Table_Delete_Async(string table, object condition)
        {
            if (null == condition) {
                throw new ArgumentNullException(nameof(condition));
            }
            var tbn = _provider.GetTableName(table);
            return Delete_Async("DELETE FROM " + tbn + " " + ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件查询个数，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public Task<int> Table_Count_Async(string table, object condition)
        {
            return Table_Count_Async(table, ConditionObjectToWhere(condition));
        }

        /// <summary>
        /// 根据条件是判断否存在，异步操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public async Task<bool> Table_Exists_Async(string table, object condition)
        {
            return await Table_Count_Async(table, ConditionObjectToWhere(condition)) > 0;
        }
    }
}