using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo3.LinQ
{
    public partial class WhereHelper<T1>
    {
        #region 06 查询 Select Page FirstOrDefault

        #region Select Page SkipTake FirstOrDefault

        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T1>> Select_Async(string selectSql = null)
        {
            return await _sqlhelper.Select_Async<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }

        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T1>> Select_Async(int limit, string selectSql = null)
        {
            return await _sqlhelper.Select_Async<T1>(limit, 0, GetFullSelectSql(selectSql), _args.ToArray());
        }

        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T1>> Select_Async(int skip, int take, string selectSql = null)
        {
            return await _sqlhelper.Select_Async<T1>(skip, take, GetFullSelectSql(selectSql), _args.ToArray());
        }

        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T1>> SelectPage_Async(int page, int itemsPerPage, string selectSql = null)
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }
            return await _sqlhelper.Select_Async<T1>((page - 1) * itemsPerPage, itemsPerPage, GetFullSelectSql(selectSql), _args.ToArray());
        }

        /// <summary>
        /// 查询 返回Page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<Page<T1>> Page_Async(int page, int itemsPerPage, string selectSql = null)
        {
            return await _sqlhelper.Page_Async<T1>(page, itemsPerPage, GetFullSelectSql(selectSql), _args.ToArray());
        }

        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<T1> FirstOrDefault_Async(string selectSql = null)
        {
            return await _sqlhelper.FirstOrDefault_Async<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }

        #endregion Select Page SkipTake FirstOrDefault

        #region Select Page FirstOrDefault

        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> Select_Async<T>(Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.Select_Async<T>(GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> Select_Async<T>(int limit, Expression<Func<T1, T>> columns) where T : class
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.Select_Async<T>(limit, GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> Select_Async<T>(int skip, int take, Expression<Func<T1, T>> columns) where T : class
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.Select_Async<T>(skip, take, GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectPage_Async<T>(int page, int itemsPerPage, Expression<Func<T1, T>> columns) where T : class
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.Select_Async<T>((page - 1) * itemsPerPage, itemsPerPage, GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefault_Async<T>(Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.FirstOrDefault_Async<T>(GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 查询 返回Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<Page<T>> Page_Async<T>(int page, int itemsPerPage, Expression<Func<T1, T>> columns) where T : class
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.Page_Async<T>(page, itemsPerPage, GetFullSelectSql(sql), _args.ToArray());
        }

        #endregion Select Page FirstOrDefault

        #endregion 06 查询 Select Page FirstOrDefault

        #region 07 查询  Count ExecuteDataTable ExecuteDataSet Select Page FirstOrDefault

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="selectSql"></param>
        /// <param name="distinct"></param>
        /// <returns></returns>
        public async Task<int> SelectCount_Async(string selectSql = null, bool distinct = false)
        {
            return await _sqlhelper.GetDatabase().ExecuteScalar_Async<int>(GetCountSql(selectSql, distinct), _args.ToArray());
        }

        /// <summary>
        /// 执行返回DataTable
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTable_Async(string selectSql = null)
        {
            return await _sqlhelper.ExecuteDataTable_Async(GetFullSelectSql(selectSql), _args.ToArray());
        }

        /// <summary>
        /// 执行返回集合
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T>> Select_Async<T>(string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return await _sqlhelper.Select_Async<T>(GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 执行返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T>> Select_Async<T>(int limit, string selectSql = null) where T : class
        {
            var sql = getSelect<T>(selectSql);
            return await _sqlhelper.Select_Async<T>(limit, 0, GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 执行返回集合
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T>> Select_Async<T>(int skip, int take, string selectSql = null) where T : class
        {
            var sql = getSelect<T>(selectSql);
            return await _sqlhelper.Select_Async<T>(skip, take, GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 执行返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectPage_Async<T>(int page, int itemsPerPage, string selectSql = null) where T : class
        {
            if (page <= 0) { page = 1; }
            if (itemsPerPage <= 0) { itemsPerPage = 20; }

            var sql = getSelect<T>(selectSql);
            return await _sqlhelper.Select_Async<T>((page - 1) * itemsPerPage, itemsPerPage, GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefault_Async<T>(string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return await _sqlhelper.FirstOrDefault_Async<T>(GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 返回页，page类
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<Page<T>> Page_Async<T>(int page, int itemsPerPage, string selectSql = null) where T : class
        {
            var sql = getSelect<T>(selectSql);
            return await _sqlhelper.Page_Async<T>(page, itemsPerPage, GetFullSelectSql(sql), _args.ToArray());
        }

        #endregion 07 查询  Count ExecuteDataTable ExecuteDataSet Select Page FirstOrDefault

        #region 10 Update

        /// <summary>
        /// 更新数据库,仅支持单一表格更新，WHERE条件为空报错
        /// </summary>
        /// <param name="setData"></param>
        /// <returns></returns>
        public async Task<int> Update_Async(object setData)
        {
            if (object.Equals(null, setData)) { throw new Exception("No setData Error!"); }
            if (_where.Length == 0) { throw new Exception("No Where Error!"); }

            var pis = setData.GetType().GetProperties();
            StringBuilder sb = new StringBuilder();
            sb.Append("SET ");
            int index = 0;
            List<object> args = new List<object>();
            foreach (var pi in pis) {
                if (pi.CanRead == false) continue;
                if (index > 0) { sb.Append(","); }
                sb.AppendFormat("{0}=@{1}", pi.Name, index++);
                args.Add(pi.GetValue(setData, null));
            }
            var sql = BuildUpdateSql(sb.ToString(), args);
            return await _sqlhelper.Update_Async<T1>(sql, _args.ToArray());
        }

        /// <summary>
        /// 更新数据库,仅支持单一表格更新，WHERE条件为空报错
        /// </summary>
        /// <param name="setData"></param>
        /// <returns></returns>
        public async Task<int> Update_Async(IDictionary<string, object> setData)
        {
            if (setData.Count == 0) { throw new Exception("No setData Error!"); }
            if (_where.Length == 0) { throw new Exception("No Where Error!"); }

            StringBuilder sb = new StringBuilder();
            sb.Append("SET ");
            int index = 0;
            List<object> args = new List<object>();
            foreach (var item in setData) {
                if (index > 0) { sb.Append(","); }
                sb.AppendFormat("{0}=@{1}", item.Key, index++);
                args.Add(item.Value);
            }
            var sql = BuildUpdateSql(sb.ToString(), args);
            return await _sqlhelper.Update_Async<T1>(sql, _args.ToArray());
        }

        /// <summary>
        /// 更新数据库,仅支持单一表格更新，WHERE条件为空报错
        /// </summary>
        /// <param name="setSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<int> Update_Async(string setSql, params object[] args)
        {
            if (string.IsNullOrEmpty(setSql)) { throw new Exception("No SET Error!"); }
            if (_where.Length == 0) { throw new Exception("No Where Error!"); }
            setSql = setSql.Trim();
            if (setSql.StartsWith("SET ", StringComparison.CurrentCultureIgnoreCase) == false) {
                setSql = "SET " + setSql;
            }
            var sql = BuildUpdateSql(setSql, args);

            return await _sqlhelper.Update_Async<T1>(sql, _args.ToArray());
        }

        #endregion 10 Update

        #region 11 Delete_Async

        /// <summary>
        /// 删除，只支持单一表格，WHERE条件为空报错
        /// </summary>
        /// <returns></returns>
        public async Task<int> Delete_Async()
        {
            if (_where.Length == 0) { throw new Exception("No Where Error!"); }
            return await _sqlhelper.Delete_Async<T1>($"WHERE {_where.ToString()}", _args.ToArray());
        }

        #endregion 11 Delete_Async
    }
}