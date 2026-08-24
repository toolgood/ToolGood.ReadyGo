using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using ToolGood.ReadyGo.Exceptions;

namespace ToolGood.ReadyGo
{
    /// <summary>
    /// 数据库（基于 NPoco 内核）
    /// </summary>
    public class Database : ToolGood.ReadyGo.NPoco.Database
    {
        /// <summary>
        /// 所属 SqlHelper（用于事件转发）
        /// </summary>
        internal SqlHelper _sqlHelper;

        /// <summary>
        /// 数据库
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="databaseType">数据库类型</param>
        /// <param name="provider">提供程序工厂</param>
        /// <param name="isolationLevel">事务隔离级别</param>
        public Database(string connectionString, ToolGood.ReadyGo.NPoco.DatabaseType databaseType, DbProviderFactory provider, IsolationLevel? isolationLevel = null)
            : base(connectionString, databaseType, provider, isolationLevel)
        {
        }

        #region 事件转发

        /// <summary>
        /// 命令执行前
        /// </summary>
        /// <param name="cmd">即将执行的数据库命令</param>
        protected override void OnExecutingCommand(DbCommand cmd)
        {
            base.OnExecutingCommand(cmd);
            if (_sqlHelper != null && _sqlHelper._sql != null) {
                _sqlHelper._sql.LastSQL = cmd.CommandText;
                _sqlHelper._sql.LastArgs = GetParameterValues(cmd);
            }
        }

        /// <summary>
        /// 命令执行出错
        /// </summary>
        /// <param name="exception">执行出错时抛出的异常</param>
        protected override void OnException(Exception exception)
        {
            base.OnException(exception);
            if (_sqlHelper != null && _sqlHelper._sql != null) {
                _sqlHelper._sql.LastErrorMessage = exception.Message;
            }
        }

        private static object[] GetParameterValues(DbCommand cmd)
        {
            var objs = new object[cmd.Parameters.Count];
            for (int i = 0; i < cmd.Parameters.Count; i++) {
                objs[i] = ((IDataParameter)cmd.Parameters[i]).Value;
            }
            return objs;
        }

        #endregion 事件转发

        /// <summary>
        /// 执行查询，返回 DataTable
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>查询结果 DataTable</returns>
        public DataTable ExecuteDataTable(string sql, object[] args)
        {
            OpenSharedConnection();
            try {
                using (var cmd = CreateCommand(Connection, CommandType.Text, sql, args)) {
                    OnExecutingCommand(cmd);
                    using (var reader = cmd.ExecuteReader()) {
                        OnExecutedCommand(cmd);
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        return dt;
                    }
                }
            } catch (Exception x) {
                OnException(x);
                throw new SqlExecuteException(x, LastCommand);
            } finally {
                CloseSharedConnection();
            }
        }

        /// <summary>
        /// 执行查询，返回 DataSet（多结果集）
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <returns>查询结果 DataSet</returns>
        public DataSet ExecuteDataSet(string sql, object[] args)
        {
            OpenSharedConnection();
            try {
                using (var cmd = CreateCommand(Connection, CommandType.Text, sql, args)) {
                    OnExecutingCommand(cmd);
                    using (var reader = cmd.ExecuteReader()) {
                        OnExecutedCommand(cmd);
                        DataSet ds = new DataSet();
                        // DataTable.Load 内部会自动推进到下一个结果集，无更多结果集时会关闭 reader
                        while (!reader.IsClosed) {
                            DataTable dt = new DataTable($"Table{ds.Tables.Count + 1}");
                            dt.Load(reader);
                            if (dt.Columns.Count == 0 && dt.Rows.Count == 0) break;
                            ds.Tables.Add(dt);
                        }
                        return ds;
                    }
                }
            } catch (Exception x) {
                OnException(x);
                throw new SqlExecuteException(x, LastCommand);
            } finally {
                CloseSharedConnection();
            }
        }

        /// <summary>
        /// 执行查询，返回 DataTable
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>查询结果 DataTable</returns>
        public async Task<DataTable> ExecuteDataTable_Async(string sql, object[] args, CancellationToken cancellationToken = default)
        {
            await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
            try {
                using (var cmd = CreateCommand(Connection, CommandType.Text, sql, args)) {
                    OnExecutingCommand(cmd);
                    using (var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false)) {
                        OnExecutedCommand(cmd);
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        return dt;
                    }
                }
            } catch (Exception x) {
                OnException(x);
                throw new SqlExecuteException(x, LastCommand);
            } finally {
                await CloseSharedConnectionAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 执行查询，返回 DataSet（多结果集）
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="args">SQL 参数</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>查询结果 DataSet</returns>
        public async Task<DataSet> ExecuteDataSet_Async(string sql, object[] args, CancellationToken cancellationToken = default)
        {
            await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
            try {
                using (var cmd = CreateCommand(Connection, CommandType.Text, sql, args)) {
                    OnExecutingCommand(cmd);
                    using (var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false)) {
                        OnExecutedCommand(cmd);
                        DataSet ds = new DataSet();
                        // DataTable.Load 内部会自动推进到下一个结果集，无更多结果集时会关闭 reader
                        while (!reader.IsClosed) {
                            DataTable dt = new DataTable($"Table{ds.Tables.Count + 1}");
                            dt.Load(reader);
                            if (dt.Columns.Count == 0 && dt.Rows.Count == 0) break;
                            ds.Tables.Add(dt);
                        }
                        return ds;
                    }
                }
            } catch (Exception x) {
                OnException(x);
                throw new SqlExecuteException(x, LastCommand);
            } finally {
                await CloseSharedConnectionAsync().ConfigureAwait(false);
            }
        }
    }
}
