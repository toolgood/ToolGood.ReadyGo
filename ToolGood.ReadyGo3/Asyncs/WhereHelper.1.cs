using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
#if !NET40

namespace ToolGood.ReadyGo3.LinQ
{
    partial class WhereHelper<T1>
    {

        #region 06 查询 Select Page SkipTake Single SingleOrDefault First FirstOrDefault


        #region Select Page SkipTake Single SingleOrDefault First FirstOrDefault
        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<List<T1>> SelectAsync(string selectSql = null)
        {
            return _sqlhelper.SelectAsync<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<List<T1>> SelectAsync(long limit, string selectSql = null)
        {
            return _sqlhelper.SelectAsync<T1>(limit, 0, GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<List<T1>> SelectAsync(long skip, long take, string selectSql = null)
        {
            return _sqlhelper.SelectAsync<T1>(skip, take, GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 查询 返回Page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<Page<T1>> PageAsync(long page, long itemsPerPage, string selectSql = null)
        {
            return _sqlhelper.PageAsync<T1>(page, itemsPerPage, GetFullSelectSql(selectSql), _args.ToArray());
        }

        /// <summary>
        /// 返回唯一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<T1> SingleAsync(string selectSql = null)
        {
            return _sqlhelper.SingleAsync<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 返回唯一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<T1> SingleOrDefaultAsync(string selectSql = null)
        {
            return _sqlhelper.SingleOrDefaultAsync<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<T1> FirstAsync(string selectSql = null)
        {
            return _sqlhelper.FirstAsync<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<T1> FirstOrDefaultAsync(string selectSql = null)
        {
            return _sqlhelper.FirstOrDefaultAsync<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }

        #endregion Select Page SkipTake Single SingleOrDefault First FirstOrDefault

        #region Select Page SkipTake Single SingleOrDefault First FirstOrDefault
        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync<T>(Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return _sqlhelper.SelectAsync<T>(GetFullSelectSql(sql), _args.ToArray());
        }
        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync<T>(long limit, Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return _sqlhelper.SelectAsync<T>(limit, GetFullSelectSql(sql), _args.ToArray());
        }
        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync<T>(long skip, long take, Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return _sqlhelper.SelectAsync<T>(skip, take, GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 返回唯一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<T> SingleAsync<T>(Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return _sqlhelper.SingleAsync<T>(GetFullSelectSql(sql), _args.ToArray());
        }
        /// <summary>
        /// 返回唯一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync<T>(Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return _sqlhelper.SingleOrDefaultAsync<T>(GetFullSelectSql(sql), _args.ToArray());
        }
        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<T> FirstAsync<T>(Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return _sqlhelper.FirstAsync<T>(GetFullSelectSql(sql), _args.ToArray());
        }
        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync<T>(Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return _sqlhelper.FirstOrDefaultAsync<T>(GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 查询 返回Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return _sqlhelper.PageAsync<T>(page, itemsPerPage, GetFullSelectSql(sql), _args.ToArray());
        }

        #endregion

        #endregion

        #region 07 查询  Count ExecuteDataTable ExecuteDataSet Select Page Single SingleOrDefault First FirstOrDefault
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="selectSql"></param>
        /// <param name="distinct"></param>
        /// <returns></returns>
        public Task<int> SelectCountAsync(string selectSql = null, bool distinct = false)
        {
            return _sqlhelper.getDatabase().ExecuteScalarAsync<int>(GetCountSql(selectSql, distinct), _args.ToArray());
        }
        /// <summary>
        /// 执行返回DataTable
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<DataTable> ExecuteDataTableAsync(string selectSql = null)
        {
            return _sqlhelper.ExecuteDataTableAsync(GetFullSelectSql(selectSql), _args.ToArray());
        }

        /// <summary>
        /// 执行返回集合
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync<T>(string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return _sqlhelper.SelectAsync<T>(GetFullSelectSql(sql), _args.ToArray());
        }
        /// <summary>
        /// 执行返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync<T>(long limit, string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return _sqlhelper.SelectAsync<T>(limit, 0, GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 执行返回集合
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<List<T>> SelectAsync<T>(long skip, long take, string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return _sqlhelper.SelectAsync<T>(skip, take, GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 返回唯一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<T> SingleAsync<T>(string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return _sqlhelper.SingleAsync<T>(GetFullSelectSql(sql), _args.ToArray());
        }
        /// <summary>
        /// 返回唯一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<T> SingleOrDefaultAsync<T>(string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return _sqlhelper.SingleOrDefaultAsync<T>(GetFullSelectSql(sql), _args.ToArray());
        }

        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<T> FirstAsync<T>(string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return _sqlhelper.FirstAsync<T>(GetFullSelectSql(sql), _args.ToArray());
        }
        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync<T>(string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return _sqlhelper.FirstOrDefaultAsync<T>(GetFullSelectSql(sql), _args.ToArray());
        }
        /// <summary>
        /// 返回页，page类
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return _sqlhelper.PageAsync<T>(page, itemsPerPage, GetFullSelectSql(sql), _args.ToArray());
        }

        #endregion

        #region 10 Update
        /// <summary>
        /// 更新数据库,仅支持单一表格更新，WHERE条件为空报错
        /// </summary>
        /// <param name="setData"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(object setData)
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
            return _sqlhelper.UpdateAsync<T1>(sql, _args.ToArray());
        }

        /// <summary>
        /// 更新数据库,仅支持单一表格更新，WHERE条件为空报错
        /// </summary>
        /// <param name="setData"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(IDictionary<string, object> setData)
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
            return _sqlhelper.UpdateAsync<T1>(sql, _args.ToArray());

        }

        /// <summary>
        /// 更新数据库,仅支持单一表格更新，WHERE条件为空报错
        /// </summary>
        /// <param name="setSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(string setSql, params object[] args)
        {
            if (string.IsNullOrEmpty(setSql)) { throw new Exception("No SET Error!"); }
            if (_where.Length == 0) { throw new Exception("No Where Error!"); }
            setSql = setSql.Trim();
            if (setSql.StartsWith("SET ", StringComparison.CurrentCultureIgnoreCase) == false) {
                setSql = "SET " + setSql;
            }
            var sql = BuildUpdateSql(setSql, args);

            return _sqlhelper.UpdateAsync<T1>(sql, _args.ToArray());
        }

        #endregion

        #region 11 DeleteAsync
        /// <summary>
        /// 删除，只支持单一表格，WHERE条件为空报错
        /// </summary>
        /// <returns></returns>
        public Task<int> DeleteAsync()
        {
            if (_where.Length == 0) { throw new Exception("No Where Error!"); }
            return _sqlhelper.DeleteAsync<T1>($"WHERE {_where.ToString()}", _args.ToArray());
        }

        #endregion

        #region 12 SelectInsertAsync
        /// <summary>
        /// 查询插入
        /// </summary>
        /// <param name="insertTableName"></param>
        /// <param name="replaceSelect"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task SelectInsertAsync(string insertTableName = null, string replaceSelect = null, params object[] args)
        {
            var sql = CreateSelectInsertSql(typeof(T1), insertTableName, replaceSelect, args);
            return _sqlhelper.ExecuteAsync(sql, _args.ToArray());
        }

        /// <summary>
        /// 查询插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="insertTableName"></param>
        /// <param name="replaceSelect"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task SelectInsertAsync<T>(string insertTableName = null, string replaceSelect = null, params object[] args)
        {
            var sql = CreateSelectInsertSql(typeof(T), insertTableName, replaceSelect, args);
            return _sqlhelper.ExecuteAsync(sql, _args.ToArray());
        }


        #endregion
    }
}
#endif