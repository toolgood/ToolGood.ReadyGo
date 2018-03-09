using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<List<T1>> SelectAsync(string selectSql = null)
        {
            return await _sqlhelper.SelectAsync<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T1>> SelectAsync(long limit, string selectSql = null)
        {
            return await _sqlhelper.SelectAsync<T1>(limit, 0, GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T1>> SelectAsync(long skip, long take, string selectSql = null)
        {
            return await _sqlhelper.SelectAsync<T1>(skip, take, GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 查询 返回Page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<Page<T1>> PageAsync(long page, long itemsPerPage, string selectSql = null)
        {
            return await _sqlhelper.PageAsync<T1>(page, itemsPerPage, GetFullSelectSql(selectSql), _args.ToArray());
        }

        /// <summary>
        /// 返回唯一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<T1> SingleAsync(string selectSql = null)
        {
            return await _sqlhelper.SingleAsync<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 返回唯一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<T1> SingleOrDefaultAsync(string selectSql = null)
        {
            return await _sqlhelper.SingleOrDefaultAsync<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<T1> FirstAsync(string selectSql = null)
        {
            return await _sqlhelper.FirstAsync<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }
        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<T1> FirstOrDefaultAsync(string selectSql = null)
        {
            return await _sqlhelper.FirstOrDefaultAsync<T1>(GetFullSelectSql(selectSql), _args.ToArray());
        }

        #endregion Select Page SkipTake Single SingleOrDefault First FirstOrDefault

        #region Select Page SkipTake Single SingleOrDefault First FirstOrDefault
        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync<T>(Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.SelectAsync<T>(GetFullSelectSql(sql), _args);
        }
        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync<T>(long limit, Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.SelectAsync<T>(limit, GetFullSelectSql(sql), _args);
        }
        /// <summary>
        /// 查询 返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync<T>(long skip, long take, Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.SelectAsync<T>(skip, take, GetFullSelectSql(sql), _args);
        }

        /// <summary>
        /// 返回唯一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> SingleAsync<T>(Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.SingleAsync<T>(GetFullSelectSql(sql), _args);
        }
        /// <summary>
        /// 返回唯一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> SingleOrDefaultAsync<T>(Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.SingleOrDefaultAsync<T>(GetFullSelectSql(sql), _args);
        }
        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> FirstAsync<T>(Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.FirstAsync<T>(GetFullSelectSql(sql), _args);
        }
        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync<T>(Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.FirstOrDefaultAsync<T>(GetFullSelectSql(sql), _args);
        }

        /// <summary>
        /// 查询 返回Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, Expression<Func<T1, T>> columns)
        {
            _sqlExpression.GetColumns(columns, out string sql);
            return await _sqlhelper.PageAsync<T>(page, itemsPerPage, GetFullSelectSql(sql), _args);
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
        public async Task<int> SelectCountAsync(string selectSql = null, bool distinct = false)
        {
            return await this._sqlhelper.getDatabase().ExecuteAsync(this.GetCountSql(selectSql, distinct), this._args.ToArray());
        }
        /// <summary>
        /// 执行返回DataTable
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(string selectSql = null)
        {
            return await this._sqlhelper.ExecuteDataTableAsync(this.GetFullSelectSql(selectSql), this._args.ToArray());
        }

        /// <summary>
        /// 执行返回集合
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync<T>(string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return await this._sqlhelper.SelectAsync<T>(this.GetFullSelectSql(sql), this._args.ToArray());
        }
        /// <summary>
        /// 执行返回集合
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync<T>(long limit, string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return await this._sqlhelper.SelectAsync<T>(limit, 0, this.GetFullSelectSql(sql), this._args.ToArray());
        }

        /// <summary>
        /// 执行返回集合
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync<T>(long skip, long take, string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return await this._sqlhelper.SelectAsync<T>(skip, take, this.GetFullSelectSql(sql), this._args.ToArray());
        }

        /// <summary>
        /// 返回唯一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<T> SingleAsync<T>(string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return await this._sqlhelper.SingleAsync<T>(this.GetFullSelectSql(sql), this._args.ToArray());
        }
        /// <summary>
        /// 返回唯一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<T> SingleOrDefaultAsync<T>(string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return await this._sqlhelper.SingleOrDefaultAsync<T>(this.GetFullSelectSql(sql), this._args.ToArray());
        }

        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<T> FirstAsync<T>(string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return await this._sqlhelper.FirstAsync<T>(this.GetFullSelectSql(sql), this._args.ToArray());
        }
        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync<T>(string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return await this._sqlhelper.FirstOrDefaultAsync<T>(this.GetFullSelectSql(sql), this._args.ToArray());
        }
        /// <summary>
        /// 返回页，page类
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public async Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string selectSql = null)
        {
            var sql = getSelect<T>(selectSql);
            return await this._sqlhelper.PageAsync<T>(page, itemsPerPage, this.GetFullSelectSql(sql), this._args.ToArray());
        }


        #endregion

    }
}
