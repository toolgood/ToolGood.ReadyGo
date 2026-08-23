/* ToolGood.ReadyGo.NPoco 5.0 - A Tiny ORMish thing for your POCO's.
 * Copyright 2011-2020. All Rights Reserved.
 *
 * Apache License 2.0 - http://www.apache.org/licenses/LICENSE-2.0
 *
 * Originally created by Brad Robinson (@toptensoftware)
 */

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToolGood.ReadyGo.NPoco.Expressions;
using ToolGood.ReadyGo.NPoco.Extensions;
using ToolGood.ReadyGo.NPoco.Linq;
using ToolGood.ReadyGo.NPoco.Internal;
using System.Threading;
using System.Runtime.CompilerServices;
using ToolGood.ReadyGo;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 表示数据库上下文，提供对数据库的连接管理、事务、查询与增删改操作的统一入口。
    /// </summary>
    public partial class Database : IDatabase, IDatabaseHelpers
    {
        /// <summary>
        /// 默认是否启用自动补全 SELECT 子句。
        /// </summary>
        public const bool DefaultEnableAutoSelect = true;

        /// <summary>
        /// 使用现有连接初始化数据库实例。
        /// </summary>
        /// <param name="connection">要使用的数据库连接。</param>
        public Database(DbConnection connection)
            : this(connection, null, null, DefaultEnableAutoSelect)
        { }

        /// <summary>
        /// 使用现有连接与数据库类型初始化数据库实例。
        /// </summary>
        /// <param name="connection">要使用的数据库连接。</param>
        /// <param name="dbType">数据库类型处理器；为 null 时自动解析。</param>
        public Database(DbConnection connection, DatabaseType? dbType)
            : this(connection, dbType, null, DefaultEnableAutoSelect)
        { }

        /// <summary>
        /// 使用现有连接、数据库类型与事务隔离级别初始化数据库实例。
        /// </summary>
        /// <param name="connection">要使用的数据库连接。</param>
        /// <param name="dbType">数据库类型处理器；为 null 时自动解析。</param>
        /// <param name="isolationLevel">事务隔离级别；为 null 时使用数据库默认级别。</param>
        public Database(DbConnection connection, DatabaseType? dbType, IsolationLevel? isolationLevel)
            : this(connection, dbType, isolationLevel, DefaultEnableAutoSelect)
        { }

        /// <summary>
        /// 使用现有连接、数据库类型、事务隔离级别与自动补全开关初始化数据库实例。
        /// </summary>
        /// <param name="connection">要使用的数据库连接。</param>
        /// <param name="dbType">数据库类型处理器；为 null 时自动解析。</param>
        /// <param name="isolationLevel">事务隔离级别；为 null 时使用数据库默认级别。</param>
        /// <param name="enableAutoSelect">是否启用自动补全 SELECT 子句。</param>
        public Database(DbConnection connection, DatabaseType? dbType, IsolationLevel? isolationLevel, bool enableAutoSelect)
        {
            EnableAutoSelect = enableAutoSelect;
            KeepConnectionAlive = true;

            _connectionPassedIn = true;
            _sharedConnection = connection;
            _connectionString = connection.ConnectionString;
            _dbType = dbType ?? ToolGood.ReadyGo.NPoco.DatabaseType.Resolve(_sharedConnection.GetType().Name, null);
            _providerName = _dbType.GetProviderName();
            _isolationLevel = isolationLevel ?? _dbType.GetDefaultTransactionIsolationLevel();
            _paramPrefix = _dbType.GetParameterPrefix(_connectionString);

            // Cause it is an external connection ensure that the isolation level matches ours
            //using (var cmd = _sharedConnection.CreateCommand())
            //{
            //    cmd.CommandTimeout = CommandTimeout;
            //    cmd.CommandText = _dbType.GetSQLForTransactionLevel(_isolationLevel);
            //    cmd.ExecuteNonQuery();
            //}
        }

        /// <summary>
        /// 使用连接字符串与数据库类型初始化数据库实例。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <param name="databaseType">数据库类型处理器。</param>
        /// <param name="provider">数据库提供程序工厂。</param>
        public Database(string connectionString, DatabaseType databaseType, DbProviderFactory provider)
            : this(connectionString, databaseType, provider, null)
        { }

        /// <summary>
        /// 使用连接字符串、数据库类型、提供程序工厂、事务隔离级别与自动补全开关初始化数据库实例。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <param name="databaseType">数据库类型处理器。</param>
        /// <param name="provider">数据库提供程序工厂。</param>
        /// <param name="isolationLevel">事务隔离级别；为 null 时使用数据库默认级别。</param>
        /// <param name="enableAutoSelect">是否启用自动补全 SELECT 子句。</param>
        public Database(string connectionString, DatabaseType databaseType, DbProviderFactory provider, IsolationLevel? isolationLevel = null, bool enableAutoSelect = DefaultEnableAutoSelect)
        {
            EnableAutoSelect = enableAutoSelect;
            KeepConnectionAlive = false;

            _sharedConnection = default!;
            _connectionString = connectionString;
            _factory = provider;
            _dbType = databaseType ?? ToolGood.ReadyGo.NPoco.DatabaseType.Resolve(_factory.GetType().Name, null);
            _providerName = _dbType.GetProviderName();
            _isolationLevel = isolationLevel ?? _dbType.GetDefaultTransactionIsolationLevel();
            _paramPrefix = _dbType.GetParameterPrefix(_connectionString);
        }

        private readonly IDatabaseType _dbType;

        /// <summary>
        /// 获取数据库类型处理器。
        /// </summary>
        public IDatabaseType DatabaseType => _dbType;

        /// <summary>
        /// 获取事务隔离级别。
        /// </summary>
        public IsolationLevel IsolationLevel => _isolationLevel;

        private IDictionary<string, object>? _data;

        /// <summary>
        /// 获取用于存储自定义数据的键值集合。
        /// </summary>
        public IDictionary<string, object> Data => _data ??= new Dictionary<string, object>();

        // Automatically close connection
        /// <summary>
        /// 释放数据库实例并关闭共享连接。
        /// </summary>
        public void Dispose()
        {
            if (KeepConnectionAlive) return;
            CloseSharedConnection();
        }

        // Set to true to keep the first opened connection alive until this object is disposed
        /// <summary>
        /// 获取或设置是否在实例存续期间保持连接存活。
        /// </summary>
        public bool KeepConnectionAlive { get; set; }

        private bool ShouldCloseConnectionAutomatically { get; set; }

        private OpenConnectionOptions OpenConnectionOptions { get; set; } = new();

        // Open a connection (can be nested)
        /// <summary>
        /// 手动打开共享连接。
        /// </summary>
        /// <param name="options">打开连接的选项。</param>
        /// <returns>当前数据库实例。</returns>
        public IDatabase OpenSharedConnection(OpenConnectionOptions? options = null)
        {
            OpenConnectionOptions = options ?? new();
            OpenSharedConnectionImp(false, true).RunSync();
            return this;
        }

        /// <summary>
        /// 异步手动打开共享连接。
        /// </summary>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>当前数据库实例。</returns>
        public async Task<IAsyncDatabase> OpenSharedConnectionAsync(CancellationToken cancellationToken = default)
        {
            await OpenSharedConnectionImp(false, false, cancellationToken);
            return this;
        }

        private static readonly OpenConnectionOptions defaultOpenConnectionOptions = new();

        /// <summary>
        /// 使用指定选项异步手动打开共享连接。
        /// </summary>
        /// <param name="options">打开连接的选项。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>当前数据库实例。</returns>
        public async Task<IAsyncDatabase> OpenSharedConnectionAsync(OpenConnectionOptions options, CancellationToken cancellationToken = default)
        {
            OpenConnectionOptions = options ?? defaultOpenConnectionOptions;
            await OpenSharedConnectionImp(false, false, cancellationToken);
            return this;
        }

        private void OpenSharedConnectionInternal()
        {
            OpenSharedConnectionImp(true, true).RunSync();
        }

        private Task OpenSharedConnectionInternalAsync(CancellationToken cancellationToken = default)
        {
            return OpenSharedConnectionImp(true, false, cancellationToken);
        }

        private async Task OpenSharedConnectionImp(bool isInternal, bool sync, CancellationToken cancellationToken = default)
        {
            if (_connectionPassedIn && _sharedConnection != null && _sharedConnection.State != ConnectionState.Open)
                throw new Exception("You must explicitly open the connection before executing anything when passing in a DbConnection to Database");

            if (_sharedConnection != null && _sharedConnection.State != ConnectionState.Broken && _sharedConnection.State != ConnectionState.Closed)
                return;

            if (!isInternal && OpenConnectionOptions.Lazy)
                return;

            ShouldCloseConnectionAutomatically = isInternal && !OpenConnectionOptions.Lazy;

            _sharedConnection = _factory?.CreateConnection()!;
            if (_sharedConnection == null) throw new Exception("SQL Connection failed to configure.");

            _sharedConnection.ConnectionString = _connectionString;

            if (_sharedConnection.State == ConnectionState.Broken)
            {
                if (sync)
                    _sharedConnection.Close();
                else { 
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                    await _sharedConnection.CloseAsync();
#else
                    _sharedConnection.Close();
#endif
                }
            }

            if (_sharedConnection.State == ConnectionState.Closed)
            {
                if (sync) 
                    _sharedConnection.Open();
                else 
                    await _sharedConnection.OpenAsync(cancellationToken);

                _sharedConnection = OnConnectionOpenedInternal(_sharedConnection);

                //using (var cmd = _sharedConnection.CreateCommand())
                //{
                //    cmd.CommandTimeout = CommandTimeout;
                //    cmd.CommandText = _dbType.GetSQLForTransactionLevel(_isolationLevel);
                //    cmd.ExecuteNonQuery();
                //}
            }
        }

        private void CloseSharedConnectionInternal()
        {
            if (ShouldCloseConnectionAutomatically && _transaction == null)
                CloseSharedConnection();
        }

        private async Task CloseSharedConnectionInternalAsync()
        {
            if (ShouldCloseConnectionAutomatically && _transaction == null)
                await CloseSharedConnectionAsync();
        }

        /// <summary>
        /// 手动关闭共享连接。
        /// </summary>
        public void CloseSharedConnection()
        {
            CloseSharedConnectionImp(true).RunSync();
        }

        /// <summary>
        /// 异步手动关闭共享连接。
        /// </summary>
        public async Task CloseSharedConnectionAsync()
        {
            await CloseSharedConnectionImp(false);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task CloseSharedConnectionImp(bool sync, CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (KeepConnectionAlive) return;

            if (_transaction != null)
            {
                if (sync)
                {
                    _transaction.Dispose();
                }
                else
                {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                    await _transaction.DisposeAsync();
#else
                    _transaction.Dispose();
#endif
                    _transaction = null;
                }
            }

            if (_sharedConnection == null) return;

            OnConnectionClosingInternal(_sharedConnection);

            if (sync)
            {
                _sharedConnection.Close();
                _sharedConnection.Dispose();
            }
            else
            {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                await _sharedConnection.CloseAsync();
                await _sharedConnection.DisposeAsync();
#else
                _sharedConnection.Close();
                _sharedConnection.Dispose();
#endif
            }
            _sharedConnection = null!;
        }

        /// <summary>
        /// 获取或设置版本冲突时的处理方式。
        /// </summary>
        public VersionExceptionHandling VersionException { get; set; } = VersionExceptionHandling.Exception;

        // Access to our shared connection
        /// <summary>
        /// 获取底层共享数据库连接。
        /// </summary>
        public DbConnection Connection => _sharedConnection;

        /// <summary>
        /// 获取当前活动的事务。
        /// </summary>
        public DbTransaction? Transaction => _transaction;

        /// <summary>
        /// 创建适用于当前数据库提供程序的参数对象。
        /// </summary>
        /// <returns>新建的数据库参数。</returns>
        public DbParameter CreateParameter()
        {
            using (var conn = _sharedConnection ?? _factory?.CreateConnection())
            {
                if (conn == null) throw new Exception("DB Connection no longer active and failed to reset.");
                using (var comm = conn.CreateCommand())
                {
                    return comm.CreateParameter();
                }
            }
        }

        // Helper to create a transaction scope
        /// <summary>
        /// 以默认隔离级别创建事务。
        /// </summary>
        /// <returns>可用于 using 语句的事务对象。</returns>
        public ITransaction GetTransaction()
        {
            return GetTransaction(_isolationLevel);
        }

        /// <summary>
        /// 以指定隔离级别创建事务。
        /// </summary>
        /// <param name="isolationLevel">事务隔离级别。</param>
        /// <returns>可用于 using 语句的事务对象。</returns>
        public ITransaction GetTransaction(IsolationLevel isolationLevel)
        {
            return new Transaction(this, isolationLevel);
        }

        /// <summary>
        /// 设置当前事务为已存在的事务对象。
        /// </summary>
        /// <param name="tran">要设置的事务。</param>
        public void SetTransaction(DbTransaction tran)
        {
            _transaction = tran;
        }

        private void OnBeginTransactionInternal()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Created new transaction using isolation level of " + _transaction?.IsolationLevel + ".");
#endif
            OnBeginTransaction();
            foreach (var interceptor in Interceptors.OfType<ITransactionInterceptor>())
            {
                interceptor.OnBeginTransaction(this);
            }
        }

        /// <summary>
        /// 事务开启时调用的钩子方法，子类可重写。
        /// </summary>
        protected virtual void OnBeginTransaction()
        {
        }

        private void OnAbortTransactionInternal()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Rolled back a transaction");
#endif
            OnAbortTransaction();
            foreach (var interceptor in Interceptors.OfType<ITransactionInterceptor>())
            {
                interceptor.OnAbortTransaction(this);
            }
        }

        /// <summary>
        /// 事务回滚时调用的钩子方法，子类可重写。
        /// </summary>
        protected virtual void OnAbortTransaction()
        {
        }

        private void OnCompleteTransactionInternal()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Committed the transaction");
#endif
            OnCompleteTransaction();
            foreach (var interceptor in Interceptors.OfType<ITransactionInterceptor>())
            {
                interceptor.OnCompleteTransaction(this);
            }
        }

        /// <summary>
        /// 事务提交时调用的钩子方法，子类可重写。
        /// </summary>
        protected virtual void OnCompleteTransaction()
        {
        }

        /// <summary>
        /// 以默认隔离级别手动开启事务。
        /// </summary>
        public void BeginTransaction()
        {
            BeginTransaction(_isolationLevel);
        }

        // Start a new transaction, can be nested, every call must be
        //	matched by a call to AbortTransaction or CompleteTransaction
        // Use `using (var scope=db.Transaction) { scope.Complete(); }` to ensure correct semantics
        /// <summary>
        /// 以指定隔离级别手动开启事务，可嵌套。
        /// </summary>
        /// <param name="isolationLevel">事务隔离级别。</param>
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (_transaction == null)
            {
                TransactionCount = 0;
                OpenSharedConnectionInternal();
                _transaction = _sharedConnection.BeginTransaction(isolationLevel);
                OnBeginTransactionInternal();
            }

            if (_transaction != null)
            {
                TransactionCount++;
            }
        }

        /// <summary>
        /// 以默认隔离级别异步创建事务。
        /// </summary>
        /// <returns>异步事务实例。</returns>
        public async Task<IAsyncTransaction> GetTransactionAsync()
        {
            return await AsyncTransaction.Init(this, _isolationLevel);
        }

        /// <summary>
        /// 以指定隔离级别异步创建事务。
        /// </summary>
        /// <param name="isolationLevel">事务隔离级别。</param>
        /// <returns>异步事务实例。</returns>
        public async Task<IAsyncTransaction> GetTransactionAsync(IsolationLevel isolationLevel)
        {
            return await AsyncTransaction.Init(this, isolationLevel);
        }

        /// <summary>
        /// 以默认隔离级别异步手动开启事务。
        /// </summary>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return BeginTransactionAsync(_isolationLevel, cancellationToken);
        }

        /// <summary>
        /// 以指定隔离级别异步手动开启事务。
        /// </summary>
        /// <param name="isolationLevel">事务隔离级别。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        public async Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                TransactionCount = 0;
                await OpenSharedConnectionInternalAsync(cancellationToken);
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                _transaction = await _sharedConnection.BeginTransactionAsync(isolationLevel, cancellationToken);
#else
                _transaction = _sharedConnection.BeginTransaction(isolationLevel);
#endif
                await OnBeginTransactionInternalAsync(cancellationToken);
            }

            if (_transaction != null)
            {
                TransactionCount++;
            }
        }

        private async Task OnBeginTransactionInternalAsync(CancellationToken cancellationToken = default)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Created new transaction using isolation level of " + _transaction?.IsolationLevel + ".");
#endif
            await OnBeginTransactionAsync(cancellationToken);
            foreach (var interceptor in Interceptors.OfType<ITransactionInterceptor>())
            {
                interceptor.OnBeginTransaction(this);
            }
        }

        /// <summary>
        /// 异步事务开启时调用的钩子方法，子类可重写。
        /// </summary>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        protected virtual Task OnBeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步手动回滚事务。
        /// </summary>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        public Task AbortTransactionAsync(CancellationToken cancellationToken = default)
        {
            return AbortTransaction(false, false, cancellationToken);
        }

        private async Task OnAbortTransactionInternalAsync(CancellationToken cancellationToken = default)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Rolled back a transaction");
#endif
            await OnAbortTransactionAsync(cancellationToken);
            foreach (var interceptor in Interceptors.OfType<ITransactionInterceptor>())
            {
                interceptor.OnAbortTransaction(this);
            }
        }

        /// <summary>
        /// 异步事务回滚时调用的钩子方法，子类可重写。
        /// </summary>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        protected virtual Task OnAbortTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 异步手动提交事务。
        /// </summary>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        public Task CompleteTransactionAsync(CancellationToken cancellationToken = default)
        {
            return CompleteTransactionImp(false, cancellationToken);
        }

        private async Task OnCompleteTransactionInternalAsync(CancellationToken cancellationToken = default)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Committed the transaction");
#endif
            await OnCompleteTransactionAsync(cancellationToken);
            foreach (var interceptor in Interceptors.OfType<ITransactionInterceptor>())
            {
                interceptor.OnCompleteTransaction(this);
            }
        }

        /// <summary>
        /// 异步事务提交时调用的钩子方法，子类可重写。
        /// </summary>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        protected virtual Task OnCompleteTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        // Abort the entire outer most transaction scope
        /// <summary>
        /// 回滚最外层事务。
        /// </summary>
        public void AbortTransaction()
        {
            TransactionIsAborted = true;
            AbortTransaction(false, true).RunSync();
        }

        private async Task AbortTransaction(bool fromComplete, bool sync, CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                TransactionIsAborted = false;
                return;
            }

            if (fromComplete == false)
            {
                TransactionCount--;
                if (TransactionCount >= 1)
                {
                    TransactionIsAborted = true;
                    return;
                }
            }

            if (TransactionIsOk())
            {
                if (sync)
                {
                    _transaction.Rollback();
                }
                else
                {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                    await _transaction.RollbackAsync(cancellationToken);
#else
                    _transaction.Rollback();
#endif
                }
            }

            if (_transaction != null)
            {
                if (sync)
                {
                    _transaction.Dispose();
                }
                else
                {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                    await _transaction.DisposeAsync();
#else
                    _transaction.Dispose();
#endif
                }
            }
            _transaction = null;
            TransactionIsAborted = false;

            // You cannot continue to use a connection after a transaction has been rolled back
            if (_sharedConnection != null)
            {
                if (sync)
                {
                    _sharedConnection.Close();
                    _sharedConnection.Open();
                }
                else
                {
                    if (sync)
                    {
                        _sharedConnection.Close();
                    }
                    else
                    {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                        await _sharedConnection.CloseAsync();
#else
                        _sharedConnection.Close();
#endif
                    }

                    await _sharedConnection.OpenAsync(cancellationToken);
                }
            }

            if (sync)
            {
                OnAbortTransactionInternal();
                CloseSharedConnectionInternal();
            }
            else
            {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                await OnAbortTransactionInternalAsync(cancellationToken);
#else
                OnAbortTransactionInternal();
#endif
                await CloseSharedConnectionInternalAsync();
            }
        }

        /// <summary>
        /// 提交当前事务。
        /// </summary>
        public void CompleteTransaction()
        {
            CompleteTransactionImp(true).RunSync();
        }

        private async Task CompleteTransactionImp(bool sync, CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                return;

            TransactionCount--;
            if (TransactionCount >= 1)
                return;

            if (TransactionIsAborted)
            {
                if (sync)
                {
                    AbortTransaction(true, true).RunSync();
                }
                else
                {
                    await AbortTransaction(true, false, cancellationToken);
                }
                return;
            }

            if (TransactionIsOk())
            {
                if (sync)
                {
                    _transaction.Commit();
                }
                else
                {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                    await _transaction.CommitAsync(cancellationToken);
#else
                    _transaction.Commit();
#endif
                }
            }

            if (sync)
            {
                _transaction?.Dispose();
            }
            else
            {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                if (_transaction != null) await _transaction.DisposeAsync();
#else
                _transaction?.Dispose();
#endif
            }
            
            _transaction = null;

            if (sync)
            {
                OnCompleteTransactionInternal();
                CloseSharedConnectionInternal();
            }
            else
            {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
                await OnCompleteTransactionInternalAsync(cancellationToken);
#else
                OnCompleteTransactionInternal();
#endif
                await CloseSharedConnectionInternalAsync();
            }
        }

        private bool TransactionIsAborted { get; set; }
        private int TransactionCount { get; set; }

        private bool TransactionIsOk()
        {
            return _sharedConnection != null
                && _transaction != null
                && _transaction.Connection != null
                && _transaction.Connection.State == ConnectionState.Open;
        }

        // Add a parameter to a DB command
        /// <summary>
        /// 向数据库命令添加参数。
        /// </summary>
        /// <param name="cmd">要添加参数的命令。</param>
        /// <param name="value">参数值。</param>
        public virtual void AddParameter(DbCommand cmd, object? value)
        {
            // Convert value to from poco type to db type
            if (Mappers != null && value != null)
            {
                var converter = Mappers.Find(x => x.GetParameterConverter(cmd, value.GetType()));
                if (converter != null)
                    value = converter(value);
            }

            // Support passed in parameters
            var idbParam = value as DbParameter;
            if (idbParam != null)
            {
                idbParam.ParameterName = string.Format("{0}{1}", _paramPrefix, cmd.Parameters.Count);
                cmd.Parameters.Add(idbParam);
                return;
            }

            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("{0}{1}", _paramPrefix, cmd.Parameters.Count);

            ParameterHelper.SetParameterValue(_dbType, p, value);

            cmd.Parameters.Add(p);
        }
       
        // Create a command
        private DbCommand CreateCommand(DbConnection connection, string sql, params object[] args)
        {
            return CreateCommand(connection, CommandType.Text, sql, args);
        }

        /// <summary>
        /// 创建数据库命令并绑定参数。
        /// </summary>
        /// <param name="connection">命令使用的连接。</param>
        /// <param name="commandType">命令类型。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数。</param>
        /// <returns>创建好的数据库命令。</returns>
        public virtual DbCommand CreateCommand(DbConnection connection, CommandType commandType, string sql, params object[] args)
        {
            if (commandType == CommandType.StoredProcedure)
            {
                return CreateStoredProcedureCommand(connection, sql, args);
            }

            // Perform parameter prefix replacements
            if (_paramPrefix != "@")
                sql = ParameterHelper.rxParamsPrefix.Replace(sql, m => _paramPrefix + m.Value.Substring(1));
            sql = sql.Replace("@@", "@");		   // <- double @@ escapes a single @

            // Create the command and add parameters
            DbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = sql;
            cmd.Transaction = _transaction;

            foreach (var item in args)
            {
                AddParameter(cmd, item);
            }

            // Notify the DB type
            _dbType.PreExecute(cmd);

            return cmd;
        }

        /// <summary>
        /// 发生异常时调用的钩子方法，子类可重写以记录或捕获异常。
        /// </summary>
        /// <param name="exception">捕获到的异常。</param>
        protected virtual void OnException(Exception exception)
        {
        }

        // Override this to log/capture exceptions
        private void OnExceptionInternal(Exception exception)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("***** EXCEPTION *****" + Environment.NewLine + Environment.NewLine + exception.Message + Environment.NewLine + exception.StackTrace);
            System.Diagnostics.Debug.WriteLine("***** LAST COMMAND *****" + Environment.NewLine + Environment.NewLine + LastCommand);
            System.Diagnostics.Debug.WriteLine("***** CONN INFO *****" + Environment.NewLine + Environment.NewLine + "Provider: " + _providerName + Environment.NewLine + "Connection String: " + _connectionString + Environment.NewLine + "DB Type: " + _dbType);
#endif
            OnException(exception);
            foreach (var interceptor in Interceptors.OfType<IExceptionInterceptor>())
            {
                interceptor.OnException(this, exception);
            }
        }

        /// <summary>
        /// 连接打开后调用的钩子方法，子类可重写。
        /// </summary>
        /// <param name="conn">已打开的连接。</param>
        /// <returns>处理后的连接。</returns>
        protected virtual DbConnection OnConnectionOpened(DbConnection conn)
        {
            return conn;
        }

        private DbConnection OnConnectionOpenedInternal(DbConnection conn)
        {
            var newConnection = OnConnectionOpened(conn);
            foreach (var interceptor in Interceptors.OfType<IConnectionInterceptor>())
            {
                newConnection = interceptor.OnConnectionOpened(this, newConnection);
            }
            return newConnection;
        }

        /// <summary>
        /// 连接关闭前调用的钩子方法，子类可重写。
        /// </summary>
        /// <param name="conn">即将关闭的连接。</param>
        protected virtual void OnConnectionClosing(DbConnection conn)
        {
        }

        private void OnConnectionClosingInternal(DbConnection conn)
        {
            OnConnectionClosing(conn);
            foreach (var interceptor in Interceptors.OfType<IConnectionInterceptor>())
            {
                interceptor.OnConnectionClosing(this, conn);
            }
        }

        /// <summary>
        /// 命令执行前调用的钩子方法，子类可重写。
        /// </summary>
        /// <param name="cmd">即将执行的命令。</param>
        protected virtual void OnExecutingCommand(DbCommand cmd)
        {

        }

        private void OnExecutingCommandInternal(DbCommand cmd)
        {
            OnExecutingCommand(cmd);
            foreach (var interceptor in Interceptors.OfType<IExecutingInterceptor>())
            {
                interceptor.OnExecutingCommand(this, cmd);
            }
        }

        /// <summary>
        /// 命令执行后调用的钩子方法，子类可重写。
        /// </summary>
        /// <param name="cmd">已执行的命令。</param>
        protected virtual void OnExecutedCommand(DbCommand cmd)
        {

        }

        private void OnExecutedCommandInternal(DbCommand cmd)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(LastCommand);
#endif
            OnExecutedCommand(cmd);
            foreach (var interceptor in Interceptors.OfType<IExecutingInterceptor>())
            {
                interceptor.OnExecutedCommand(this, cmd);
            }
        }

        /// <summary>
        /// 插入前调用的钩子方法，返回 false 可取消插入。
        /// </summary>
        /// <param name="insertContext">插入上下文。</param>
        /// <returns>是否允许继续插入。</returns>
        protected virtual bool OnInserting(InsertContext insertContext)
        {
            return true;
        }

        private bool OnInsertingInternal(InsertContext insertContext)
        {
            var result = OnInserting(insertContext);
            return result && Interceptors.OfType<IDataInterceptor>().All(x => x.OnInserting(this, insertContext));
        }

        /// <summary>
        /// 更新前调用的钩子方法，返回 false 可取消更新。
        /// </summary>
        /// <param name="updateContext">更新上下文。</param>
        /// <returns>是否允许继续更新。</returns>
        protected virtual bool OnUpdating(UpdateContext updateContext)
        {
            return true;
        }

        private bool OnUpdatingInternal(UpdateContext updateContext)
        {
            var result = OnUpdating(updateContext);
            return result && Interceptors.OfType<IDataInterceptor>().All(x => x.OnUpdating(this, updateContext));
        }

        /// <summary>
        /// 删除前调用的钩子方法，返回 false 可取消删除。
        /// </summary>
        /// <param name="deleteContext">删除上下文。</param>
        /// <returns>是否允许继续删除。</returns>
        protected virtual bool OnDeleting(DeleteContext deleteContext)
        {
            return true;
        }

        private bool OnDeletingInternal(DeleteContext deleteContext)
        {
            var result = OnDeleting(deleteContext);
            return result && Interceptors.OfType<IDataInterceptor>().All(x => x.OnDeleting(this, deleteContext));
        }

        /// <summary>
        /// 创建用于调用存储过程的命令。
        /// </summary>
        /// <param name="connection">命令使用的连接。</param>
        /// <param name="name">存储过程名称。</param>
        /// <param name="args">参数。</param>
        /// <returns>存储过程命令。</returns>
        public DbCommand CreateStoredProcedureCommand(DbConnection connection, string name, params object[] args)
        {
            DbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = name;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = _transaction;

            if (args.Length == 1)
            {
                var arg = args[0] as DbParameter;
                if (arg != null)
                {
                    cmd.Parameters.Add(arg);
                }
                else
                {
                    var props = args[0].GetType().GetProperties().Select(x => new { x.Name, Value = x.GetValue(args[0], null) }).ToList();
                    foreach (var item in props)
                    {
                        DbParameter param = cmd.CreateParameter();
                        param.ParameterName = item.Name;

                        ParameterHelper.SetParameterValue(_dbType, param, item.Value);

                        cmd.Parameters.Add(param);
                    }
                }
            }
            else
            {
                cmd.Parameters.AddRange(args.OfType<DbParameter>().ToArray());
            }

            // Notify the DB type
            _dbType.PreExecute(cmd);

            return cmd;
        }

        // Execute a non-query command
        /// <summary>
        /// 执行非查询 SQL 语句。
        /// </summary>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>受影响的行数。</returns>
        public int Execute(string sql, params object[] args)
        {
            return Execute(new Sql(sql, args));
        }

        /// <summary>
        /// 执行非查询 SQL 语句。
        /// </summary>
        /// <param name="Sql">封装 SQL 与参数的对象。</param>
        /// <returns>受影响的行数。</returns>
        public int Execute(Sql Sql)
        {
            return Execute(Sql.SQL, CommandType.Text, Sql.Arguments);
        }

        /// <summary>
        /// 以指定命令类型执行 SQL 语句。
        /// </summary>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="commandType">命令类型。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>受影响的行数。</returns>
        public int Execute(string sql, CommandType commandType, params object[] args)
        {
            try
            {
                OpenSharedConnectionInternal();
                using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                {
                    var result = ExecuteNonQueryHelper(cmd);
                    return result;
                }
            }
            catch (Exception x)
            {
                OnExceptionInternal(x);
                throw;
            }
            finally
            {
                CloseSharedConnectionInternal();
            }
        }

        // Execute and cast a scalar property
        /// <summary>
        /// 执行标量查询并将结果转换为指定类型。
        /// </summary>
        /// <typeparam name="T">返回类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>查询结果的首行首列值。</returns>
        public T ExecuteScalar<T>(string sql, params object[] args)
        {
            return ExecuteScalar<T>(new Sql(sql, args));
        }

        /// <summary>
        /// 执行标量查询并将结果转换为指定类型。
        /// </summary>
        /// <typeparam name="T">返回类型。</typeparam>
        /// <param name="Sql">封装 SQL 与参数的对象。</param>
        /// <returns>查询结果的首行首列值。</returns>
        public T ExecuteScalar<T>(Sql Sql)
        {
            return ExecuteScalar<T>(Sql.SQL, CommandType.Text, Sql.Arguments);
        }

        /// <summary>
        /// 以指定命令类型执行标量查询并将结果转换为指定类型。
        /// </summary>
        /// <typeparam name="T">返回类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="commandType">命令类型。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>查询结果的首行首列值。</returns>
        public T ExecuteScalar<T>(string sql, CommandType commandType, params object[] args)
        {
            try
            {
                OpenSharedConnectionInternal();
                using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                {
                    object val = ExecuteScalarHelper(cmd);

                    if (val == null || val == DBNull.Value)
                        return default!;

                    Type t = typeof(T);
                    Type? u = Nullable.GetUnderlyingType(t);

                    return (T)Convert.ChangeType(val, u ?? t);
                }
            }
            catch (Exception x)
            {
                OnExceptionInternal(x);
                throw;
            }
            finally
            {
                CloseSharedConnectionInternal();
            }
        }

        /// <summary>
        /// 获取或设置是否在查询时自动补全 SELECT 子句。
        /// </summary>
        public bool EnableAutoSelect { get; set; }

        // Return a typed list of pocos
        /// <summary>
        /// 查询并返回类型 T 的对象列表。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>对象列表。</returns>
        public List<T> Fetch<T>(string sql, params object[] args)
        {
            return Fetch<T>(new Sql(sql, args));
        }

        /// <summary>
        /// 查询并返回类型 T 的对象列表。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>对象列表。</returns>
        public List<T> Fetch<T>(Sql sql)
        {
            return Query<T>(sql).ToList();
        }

        /// <summary>
        /// 查询并返回类型 T 的全部对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>对象列表。</returns>
        public List<T> Fetch<T>()
        {
            return Fetch<T>("");
        }

        /// <summary>
        /// 根据跳过/获取数量拆分 SQL 并生成分页查询与计数查询。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="skip">跳过的行数。</param>
        /// <param name="take">获取的行数。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组（可能被修改）。</param>
        /// <param name="sqlCount">输出的计数 SQL。</param>
        /// <param name="sqlPage">输出的分页 SQL。</param>
        public void BuildPageQueries<T>(long skip, long take, string sql, ref object[] args, out string sqlCount, out string sqlPage)
        {
            // Add auto select clause
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause(this, typeof(T), sql);

            // Split the SQL
            SQLParts parts;
            if (!PagingHelper.SplitSQL(sql, out parts)) throw new Exception("Unable to parse SQL statement for paged query");

            sqlPage = _dbType.BuildPageQuery(skip, take, parts, ref args);
            sqlCount = parts.sqlCount;
        }

        // Fetch a page
        /// <summary>
        /// 分页查询并返回类型 T 的对象与分页元数据。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="itemsPerPage">每页条数。</param>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>分页结果。</returns>
        public Page<T> Page<T>(long page, long itemsPerPage, Sql sql)
        {
            return Page<T>(page, itemsPerPage, sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 分页查询并返回类型 T 的对象列表。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="itemsPerPage">每页条数。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>对象列表。</returns>
        public List<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            return SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);
        }

        /// <summary>
        /// 分页查询并返回类型 T 的对象列表。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="itemsPerPage">每页条数。</param>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>对象列表。</returns>
        public List<T> Fetch<T>(long page, long itemsPerPage, Sql sql)
        {
            return SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 按跳过/获取数量查询并返回类型 T 的对象列表。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="skip">跳过的行数。</param>
        /// <param name="take">获取的行数。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>对象列表。</returns>
        public List<T> SkipTake<T>(long skip, long take, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries<T>(skip, take, sql, ref args, out sqlCount, out sqlPage);
            return Fetch<T>(sqlPage, args);
        }

        /// <summary>
        /// 按跳过/获取数量查询并返回类型 T 的对象列表。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="skip">跳过的行数。</param>
        /// <param name="take">获取的行数。</param>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>对象列表。</returns>
        public List<T> SkipTake<T>(long skip, long take, Sql sql)
        {
            return SkipTake<T>(skip, take, sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 查询两列结果并转换为字典。
        /// </summary>
        /// <typeparam name="TKey">字典键类型。</typeparam>
        /// <typeparam name="TValue">字典值类型。</typeparam>
        /// <param name="Sql">封装 SQL 与参数的对象。</param>
        /// <returns>查询结果字典。</returns>
        public Dictionary<TKey, TValue> Dictionary<TKey, TValue>(Sql Sql) where TKey : notnull
        {
            return Dictionary<TKey, TValue>(Sql.SQL, Sql.Arguments);
        }

        /// <summary>
        /// 查询两列结果并转换为字典。
        /// </summary>
        /// <typeparam name="TKey">字典键类型。</typeparam>
        /// <typeparam name="TValue">字典值类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>查询结果字典。</returns>
        public Dictionary<TKey, TValue> Dictionary<TKey, TValue>(string sql, params object[] args) where TKey : notnull
        {
            var newDict = new Dictionary<TKey, TValue>();
            bool isConverterSet = false;
            Func<object, object> converter1 = x => x, converter2 = x => x;

            foreach (var line in Query<Dictionary<string, object>>(sql, args))
            {
                object key = line.ElementAt(0).Value;
                object? value = line.ElementAt(1).Value;

                if (isConverterSet == false)
                {
                    converter1 = MappingHelper.GetConverter(Mappers, null, typeof(TKey), key.GetType()) ?? (x => x);
                    converter2 = (value != null ? MappingHelper.GetConverter(Mappers, null, typeof(TValue), value.GetType()) : null) ?? (x => x);
                    isConverterSet = true;
                }

                var keyConverted = (TKey)Convert.ChangeType(converter1(key), typeof(TKey));

                var valueType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
                var valConv = converter2(value!);
                var valConverted = valConv != null ? (TValue)Convert.ChangeType(valConv, valueType) : default;

                if (keyConverted != null)
                {
                    newDict.Add(keyConverted, valConverted!);
                }
            }
            return newDict;
        }

        // Return an enumerable collection of pocos
        /// <summary>
        /// 查询并返回类型 T 的对象序列。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>对象序列。</returns>
        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            return Query<T>(new Sql(sql, args));
        }

        /// <summary>
        /// 查询并返回类型 T 的对象序列。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="Sql">封装 SQL 与参数的对象。</param>
        /// <returns>对象序列。</returns>
        public IEnumerable<T> Query<T>(Sql Sql)
        {
            return Query(default(T)!, Sql);
        }

        private async IAsyncEnumerable<T> ReadAsync<T>(object instance, DbDataReader r, PocoData pd, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var factory = new MappingFactory(pd, r);
            while (true)
            {
                T poco;
                try
                {
                    if (!await r.ReadAsync(cancellationToken).ConfigureAwait(false)) yield break;
                    poco = (T)factory.Map(r, instance);
                }
                catch (Exception x)
                {
                    OnExceptionInternal(x);
                    throw;
                }

                yield return poco;
            }
        }

        private async IAsyncEnumerable<T> ReadOneToManyAsync<T>(T instance, DbDataReader r, Expression<Func<T, IList>> listExpression, Func<T, object[]> idFunc, PocoData pocoData, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Func<T, IList>? listFunc = null;
            PocoMember? pocoMember = null;
            PocoMember? foreignMember = null;

            if (listExpression != null)
            {
                idFunc ??= (x => pocoData.GetPrimaryKeyValues(x));
                listFunc = listExpression.Compile();
                var key = PocoColumn.GenerateKey(MemberChainHelper.GetMembers(listExpression));
                pocoMember = pocoData.Members.FirstOrDefault(x => x.Name == key);
                foreignMember = pocoMember?.PocoMemberChildren.FirstOrDefault(x => x.Name == pocoMember.ReferenceMemberName && x.ReferenceType == ReferenceType.Foreign);
            }

            var factory = new MappingFactory(pocoData, r);
            object? prevPoco = null;

            while (true)
            {
                T poco;
                try
                {
                    if (!await r.ReadAsync(cancellationToken).ConfigureAwait(false)) break;
                    poco = (T)factory.Map(r, instance);
                }
                catch (Exception x)
                {
                    OnExceptionInternal(x);
                    throw;
                }

                if (prevPoco != null)
                {
                    if (listFunc != null
                        && pocoMember != null
                        && idFunc(poco).SequenceEqual(idFunc((T)prevPoco)))
                    {
                        OneToManyHelper.SetListValue(listFunc, pocoMember, prevPoco, poco);
                        continue;
                    }

                    OneToManyHelper.SetForeignList(listFunc, foreignMember, prevPoco);
                    yield return (T)prevPoco;
                }

                prevPoco = poco;
            }

            if (prevPoco != null)
            {
                OneToManyHelper.SetForeignList(listFunc, foreignMember, prevPoco);
                yield return (T)prevPoco;
            }
        }

        private IEnumerable<T> Read<T>(object? instance, DbDataReader r, PocoData pd)
        {
            var factory = new MappingFactory(pd, r);
            while (true)
            {
                T poco;
                try
                {
                    if (!r.Read()) yield break;
                    poco = (T)factory.Map(r, instance);
                }
                catch (Exception x)
                {
                    OnExceptionInternal(x);
                    throw;
                }

                yield return poco;
            }
        }

        private IEnumerable<T> ReadOneToMany<T>(T instance, DbDataReader r, Expression<Func<T, IList>> listExpression, Func<T, object[]>? idFunc, PocoData pocoData)
        {
            Func<T, IList>? listFunc = null;
            PocoMember? pocoMember = null;
            PocoMember? foreignMember = null;

            if (listExpression != null)
            {
                idFunc ??= (x => pocoData.GetPrimaryKeyValues(x));
                listFunc = listExpression.Compile();
                var key = PocoColumn.GenerateKey(MemberChainHelper.GetMembers(listExpression));
                pocoMember = pocoData.Members.FirstOrDefault(x => x.Name == key);
                foreignMember = pocoMember?.PocoMemberChildren.FirstOrDefault(x => x.Name == pocoMember.ReferenceMemberName && x.ReferenceType == ReferenceType.Foreign);
            }

            var factory = new MappingFactory(pocoData, r);
            object? prevPoco = null;

            while (true)
            {
                T poco;
                try
                {
                    if (!r.Read()) break;
                    poco = (T)factory.Map(r, instance);
                }
                catch (Exception x)
                {
                    OnExceptionInternal(x);
                    throw;
                }

                if (prevPoco != null)
                {
                    if (idFunc != null
                        && listFunc != null
                        && pocoMember != null
                        && idFunc(poco).SequenceEqual(idFunc((T)prevPoco)))
                    {
                        OneToManyHelper.SetListValue(listFunc, pocoMember, prevPoco, poco);
                        continue;
                    }

                    OneToManyHelper.SetForeignList(listFunc, foreignMember, prevPoco);
                    yield return (T)prevPoco;
                }

                prevPoco = poco;
            }

            if (prevPoco != null)
            {
                OneToManyHelper.SetForeignList(listFunc, foreignMember, prevPoco);
                yield return (T)prevPoco;
            }
        }

        /// <summary>
        /// 获取用于 LINQ 查询的查询提供程序。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>查询提供程序。</returns>
        public IQueryProviderWithIncludes<T> Query<T>()
        {
            return new QueryProvider<T>(this);
        }

        /// <summary>
        /// 组合多个 LINQ 查询并一次性执行，返回各结果集。
        /// </summary>
        /// <returns>由各结果集组成的元组。</returns>
        public (List<T>, List<T1>, List<T2>, List<T3>) QueryMultiple<T, T1, T2, T3>(
            Func<IQueryProviderWithIncludes<T>, IQueryProvider<T>> query1,
            Func<IQueryProviderWithIncludes<T1>, IQueryProvider<T1>> query2,
            Func<IQueryProviderWithIncludes<T2>, IQueryProvider<T2>> query3,
            Func<IQueryProviderWithIncludes<T3>, IQueryProvider<T3>> query4
            )
        {
            var qp1 = new QueryProvider<T>(this);
            var qp2 = new QueryProvider<T1>(this);
            var qp3 = new QueryProvider<T2>(this);
            var qp4 = new QueryProvider<T3>(this);
            query1.Invoke(qp1);
            query2.Invoke(qp2);
            query3.Invoke(qp3);
            query4.Invoke(qp4);
            var sql1 = ((INeedSql)qp1).GetSql();
            var sql2 = ((INeedSql)qp2).GetSql();
            var sql3 = ((INeedSql)qp3).GetSql();
            var sql4 = ((INeedSql)qp4).GetSql();
            return FetchMultiple<T, T1, T2, T3>(sql1.Concat(sql2, ";").Concat(sql3, ";").Concat(sql4, ";"));
        }

        private IEnumerable<T> Query<T>(T instance, Sql Sql)
        {
            return QueryImp(instance, null, null, Sql);
        }

        /// <summary>
        /// 按运行时类型查询并返回对象列表。
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>对象列表。</returns>
        public List<object> Fetch(Type type, string sql, params object[] args)
        {
            return Fetch(type, new Sql(sql, args));
        }

        /// <summary>
        /// 按运行时类型查询并返回对象列表。
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <param name="Sql">封装 SQL 与参数的对象。</param>
        /// <returns>对象列表。</returns>
        public List<object> Fetch(Type type, Sql Sql)
        {
            return Query(type, Sql).ToList();
        }

        /// <summary>
        /// 按运行时类型查询并返回对象序列。
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>对象序列。</returns>
        public IEnumerable<object> Query(Type type, string sql, params object[] args)
        {
            return Query(type, new Sql(sql, args));
        }

        /// <summary>
        /// 按运行时类型查询并返回对象序列。
        /// </summary>
        /// <param name="type">对象类型。</param>
        /// <param name="Sql">封装 SQL 与参数的对象。</param>
        /// <returns>对象序列。</returns>
        public IEnumerable<object> Query(Type type, Sql Sql)
        {
            var sql = Sql.SQL;
            var args = Sql.Arguments;

            if (EnableAutoSelect) sql = AutoSelectHelper.AddSelectClause(this, type, sql);

            try
            {
                OpenSharedConnectionInternal();
                using var cmd = CreateCommand(_sharedConnection, sql, args);
                using var reader = ExecuteDataReader(cmd, true).RunSync();
                var read = Read<object>(null, reader, PocoDataFactory.ForType(type));
                foreach (var item in read)
                {
                    yield return item;
                }
            }
            finally
            {
                CloseSharedConnectionInternal();
            }
        }

        internal IEnumerable<T> QueryImp<T>(T instance, Expression<Func<T, IList>>? listExpression, Func<T, object[]>? idFunc, Sql Sql, PocoData? pocoData = null)
        {
            pocoData ??= PocoDataFactory.ForType(typeof(T));

            var sql = Sql.SQL;
            var args = Sql.Arguments;

            if (EnableAutoSelect) sql = AutoSelectHelper.AddSelectClause(this, typeof(T), sql);

            try
            {
                OpenSharedConnectionInternal();
                using var cmd = CreateCommand(_sharedConnection, sql, args);
                using var reader = ExecuteDataReader(cmd, true).RunSync();
                var read = listExpression != null ? ReadOneToMany(instance, reader, listExpression, idFunc, pocoData) : Read<T>(instance, reader, pocoData);
                foreach (var item in read)
                {
                    yield return item;
                }
            }
            finally
            {
                CloseSharedConnectionInternal();
            }
        }

        private async Task<DbDataReader> ExecuteDataReader(DbCommand cmd, bool sync, CancellationToken cancellationToken = default)
        {
            DbDataReader r;
            try
            {
                r = sync ? ExecuteReaderHelper(cmd) : await ExecuteReaderHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception x)
            {
                OnExceptionInternal(x);
                throw;
            }
            return r;
        }

        /// <summary>
        /// 查询一对多数据，将子记录填充到指定的列表属性中。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="many">指向列表属性的表达式。</param>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>对象列表。</returns>
        public List<T> FetchOneToMany<T>(Expression<Func<T, IList>> many, Sql sql)
        {
            return QueryImp(default!, many, null, sql).ToList();
        }

        /// <summary>
        /// 查询一对多数据，将子记录填充到指定的列表属性中。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="many">指向列表属性的表达式。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>对象列表。</returns>
        public List<T> FetchOneToMany<T>(Expression<Func<T, IList>> many, string sql, params object[] args)
        {
            return FetchOneToMany(many, new Sql(sql, args));
        }

        /// <summary>
        /// 查询一对多数据，使用自定义主键提取函数将子记录填充到列表属性中。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="many">指向列表属性的表达式。</param>
        /// <param name="idFunc">提取主键的函数。</param>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>对象列表。</returns>
        public List<T> FetchOneToMany<T>(Expression<Func<T, IList>> many, Func<T, object> idFunc, Sql sql)
        {
            return QueryImp(default!, many, x => new[] { idFunc(x) }, sql).ToList();
        }

        /// <summary>
        /// 查询一对多数据，使用自定义主键提取函数将子记录填充到列表属性中。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="many">指向列表属性的表达式。</param>
        /// <param name="idFunc">提取主键的函数。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>对象列表。</returns>
        public List<T> FetchOneToMany<T>(Expression<Func<T, IList>> many, Func<T, object> idFunc, string sql, params object[] args)
        {
            return FetchOneToMany(many, idFunc, new Sql(sql, args));
        }

        /// <summary>
        /// 分页查询并返回类型 T 的对象与分页元数据。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="itemsPerPage">每页条数。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>分页结果。</returns>
        public Page<T> Page<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            return PageImpAsync<T>(page, itemsPerPage, sql, args, true).RunSync();
        }

        // Actual implementation of the multi-poco paging
        private async Task<Page<T>> PageImpAsync<T>(long page, long itemsPerPage, string sql, object[] args, bool sync, CancellationToken cancellationToken = default)
        {
            if (page <= 0 || itemsPerPage <= 0)
            {
                throw new ArgumentException("Parameter page and itemsPerPage must be greater then zero.");
            }

            string sqlCount, sqlPage;

            long offset = (page - 1) * itemsPerPage;

            BuildPageQueries<T>(offset, itemsPerPage, sql, ref args, out sqlCount, out sqlPage);

            // Save the one-time command time out and use it for both queries
            int saveTimeout = OneTimeCommandTimeout;

            // Setup the paged result
            var result = new Page<T>();
            result.CurrentPage = page;
            result.PageSize = itemsPerPage;
            result.TotalItems = sync ? ExecuteScalar<long>(sqlCount, args) : await ExecuteScalarAsync<long>(sqlCount, args, cancellationToken).ConfigureAwait(false);
            result.TotalPages = result.TotalItems / itemsPerPage;
            if ((result.TotalItems % itemsPerPage) != 0)
                result.TotalPages++;

            OneTimeCommandTimeout = saveTimeout;

            // Get the records
            result.Items = sync
                ? Fetch<T>(new Sql(sqlPage, args))
                : await FetchAsync<T>(new Sql(sqlPage, args), cancellationToken).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// 获取多个结果集，并由回调合并为一个对象。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">返回类型。</typeparam>
        /// <param name="cb">合并各结果集的回调。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>合并后的对象。</returns>
        public TRet FetchMultiple<T1, T2, TRet>(Func<List<T1>, List<T2>, TRet> cb, string sql, params object[] args) { return FetchMultipleImp<T1, T2, DontMap, DontMap, TRet>(new[] { typeof(T1), typeof(T2) }, cb, new Sql(sql, args), true).RunSync(); }

        /// <summary>
        /// 获取多个结果集，并由回调合并为一个对象。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">返回类型。</typeparam>
        /// <param name="cb">合并各结果集的回调。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>合并后的对象。</returns>
        public TRet FetchMultiple<T1, T2, T3, TRet>(Func<List<T1>, List<T2>, List<T3>, TRet> cb, string sql, params object[] args) { return FetchMultipleImp<T1, T2, T3, DontMap, TRet>(new[] { typeof(T1), typeof(T2), typeof(T3) }, cb, new Sql(sql, args), true).RunSync(); }

        /// <summary>
        /// 获取多个结果集，并由回调合并为一个对象。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">返回类型。</typeparam>
        /// <param name="cb">合并各结果集的回调。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>合并后的对象。</returns>
        public TRet FetchMultiple<T1, T2, T3, T4, TRet>(Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> cb, string sql, params object[] args) { return FetchMultipleImp<T1, T2, T3, T4, TRet>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb, new Sql(sql, args), true).RunSync(); }

        /// <summary>
        /// 获取多个结果集，并由回调合并为一个对象。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">返回类型。</typeparam>
        /// <param name="cb">合并各结果集的回调。</param>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>合并后的对象。</returns>
        public TRet FetchMultiple<T1, T2, TRet>(Func<List<T1>, List<T2>, TRet> cb, Sql sql) { return FetchMultipleImp<T1, T2, DontMap, DontMap, TRet>(new[] { typeof(T1), typeof(T2) }, cb, sql, true).RunSync(); }

        /// <summary>
        /// 获取多个结果集，并由回调合并为一个对象。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">返回类型。</typeparam>
        /// <param name="cb">合并各结果集的回调。</param>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>合并后的对象。</returns>
        public TRet FetchMultiple<T1, T2, T3, TRet>(Func<List<T1>, List<T2>, List<T3>, TRet> cb, Sql sql) { return FetchMultipleImp<T1, T2, T3, DontMap, TRet>(new[] { typeof(T1), typeof(T2), typeof(T3) }, cb, sql, true).RunSync(); }

        /// <summary>
        /// 获取多个结果集，并由回调合并为一个对象。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">返回类型。</typeparam>
        /// <param name="cb">合并各结果集的回调。</param>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>合并后的对象。</returns>
        public TRet FetchMultiple<T1, T2, T3, T4, TRet>(Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> cb, Sql sql) { return FetchMultipleImp<T1, T2, T3, T4, TRet>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb, sql, true).RunSync(); }

        /// <summary>
        /// 获取多个结果集并合并为元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>由各结果集组成的元组。</returns>
        public (List<T1>, List<T2>) FetchMultiple<T1, T2>(string sql, params object[] args) { return FetchMultipleImp<T1, T2, DontMap, DontMap, (List<T1>, List<T2>)>(new[] { typeof(T1), typeof(T2) }, new Func<List<T1>, List<T2>, (List<T1>, List<T2>)>((y, z) => (y, z)), new Sql(sql, args), true).RunSync(); }

        /// <summary>
        /// 获取多个结果集并合并为元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>由各结果集组成的元组。</returns>
        public (List<T1>, List<T2>, List<T3>) FetchMultiple<T1, T2, T3>(string sql, params object[] args) { return FetchMultipleImp<T1, T2, T3, DontMap, (List<T1>, List<T2>, List<T3>)>(new[] { typeof(T1), typeof(T2), typeof(T3) }, new Func<List<T1>, List<T2>, List<T3>, (List<T1>, List<T2>, List<T3>)>((x, y, z) => (x, y, z)), new Sql(sql, args), true).RunSync(); }

        /// <summary>
        /// 获取多个结果集并合并为元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>由各结果集组成的元组。</returns>
        public (List<T1>, List<T2>, List<T3>, List<T4>) FetchMultiple<T1, T2, T3, T4>(string sql, params object[] args) { return FetchMultipleImp<T1, T2, T3, T4, (List<T1>, List<T2>, List<T3>, List<T4>)>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, new Func<List<T1>, List<T2>, List<T3>, List<T4>, (List<T1>, List<T2>, List<T3>, List<T4>)>((w, x, y, z) => (w, x, y, z)), new Sql(sql, args), true).RunSync(); }

        /// <summary>
        /// 获取多个结果集并合并为元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>由各结果集组成的元组。</returns>
        public (List<T1>, List<T2>) FetchMultiple<T1, T2>(Sql sql) { return FetchMultipleImp<T1, T2, DontMap, DontMap, (List<T1>, List<T2>)>(new[] { typeof(T1), typeof(T2) }, new Func<List<T1>, List<T2>, (List<T1>, List<T2>)>((y, z) => (y, z)), sql, true).RunSync(); }

        /// <summary>
        /// 获取多个结果集并合并为元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>由各结果集组成的元组。</returns>
        public (List<T1>, List<T2>, List<T3>) FetchMultiple<T1, T2, T3>(Sql sql) { return FetchMultipleImp<T1, T2, T3, DontMap, (List<T1>, List<T2>, List<T3>)>(new[] { typeof(T1), typeof(T2), typeof(T3) }, new Func<List<T1>, List<T2>, List<T3>, (List<T1>, List<T2>, List<T3>)>((x, y, z) => (x, y, z)), sql, true).RunSync(); }

        /// <summary>
        /// 获取多个结果集并合并为元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>由各结果集组成的元组。</returns>
        public (List<T1>, List<T2>, List<T3>, List<T4>) FetchMultiple<T1, T2, T3, T4>(Sql sql) { return FetchMultipleImp<T1, T2, T3, T4, (List<T1>, List<T2>, List<T3>, List<T4>)>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, new Func<List<T1>, List<T2>, List<T3>, List<T4>, (List<T1>, List<T2>, List<T3>, List<T4>)>((w, x, y, z) => (w, x, y, z)), sql, true).RunSync(); }

        /// <summary>
        /// 内部占位类型，表示不映射的结果集。
        /// </summary>
        public class DontMap { }

        // Actual implementation of the multi query
        private async Task<TRet> FetchMultipleImp<T1, T2, T3, T4, TRet>(Type[] types, object cb, Sql Sql, bool sync, CancellationToken cancellationToken = default)
        {
            var sql = Sql.SQL;
            var args = Sql.Arguments;

            try
            {
                if (sync) OpenSharedConnectionInternal();
                else await OpenSharedConnectionInternalAsync(cancellationToken);

                using var cmd = CreateCommand(_sharedConnection, sql, args);
                using var r = sync ? ExecuteDataReader(cmd, true).RunSync() : await ExecuteDataReader(cmd, false, cancellationToken).ConfigureAwait(false);

                var typeIndex = 1;
                var list1 = new List<T1>();
                var list2 = types.Length > 1 ? new List<T2>() : null;
                var list3 = types.Length > 2 ? new List<T3>() : null;
                var list4 = types.Length > 3 ? new List<T4>() : null;
                do
                {
                    if (typeIndex > types.Length)
                        break;

                    var pd = PocoDataFactory.ForType(types[typeIndex - 1]);
                    var factory = new MappingFactory(pd, r);

                    while (true)
                    {
                        try
                        {
                            if (sync ? !r.Read() : !await r.ReadAsync(cancellationToken).ConfigureAwait(false))
                                break;

                            switch (typeIndex)
                            {
                                case 1:
                                    list1.Add((T1)factory.Map(r, default(T1)));
                                    break;
                                case 2:
                                    list2!.Add((T2)factory.Map(r, default(T2)));
                                    break;
                                case 3:
                                    list3!.Add((T3)factory.Map(r, default(T3)));
                                    break;
                                case 4:
                                    list4!.Add((T4)factory.Map(r, default(T4)));
                                    break;
                            }
                        }
                        catch (Exception x)
                        {
                            OnExceptionInternal(x);
                            throw;
                        }
                    }

                    typeIndex++;
                } while (sync ? r.NextResult() : await r.NextResultAsync(cancellationToken).ConfigureAwait(false));

                switch (types.Length)
                {
                    case 2:
                        return ((Func<List<T1>, List<T2>, TRet>)cb)(list1, list2!);
                    case 3:
                        return ((Func<List<T1>, List<T2>, List<T3>, TRet>)cb)(list1, list2!, list3!);
                    case 4:
                        return ((Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet>)cb)(list1, list2!, list3!, list4!);
                }

                return default(TRet)!;
            }
            finally
            {
                if (sync) CloseSharedConnectionInternal();
                else await CloseSharedConnectionInternalAsync();
            }
        }

        /// <summary>
        /// 判断主键对应的记录是否存在。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="primaryKey">主键值。</param>
        /// <returns>若存在返回 true，否则返回 false。</returns>
        public bool Exists<T>(object primaryKey)
        {
            return ExistsAsync<T>(primaryKey, true).RunSync();
        }

        /// <summary>
        /// 按主键获取类型 T 的唯一对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="primaryKey">主键值。</param>
        /// <returns>匹配的对象。</returns>
        public T SingleById<T>(object primaryKey)
        {
            var sql = GenerateSingleByIdSql<T>(primaryKey);
            return Single<T>(sql);
        }

        /// <summary>
        /// 按主键获取类型 T 的唯一对象，若无结果返回默认值。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="primaryKey">主键值。</param>
        /// <returns>匹配的对象或默认值。</returns>
        public T? SingleOrDefaultById<T>(object primaryKey)
        {
            var sql = GenerateSingleByIdSql<T>(primaryKey);
            return SingleOrDefault<T>(sql);
        }

        private Sql GenerateSingleByIdSql<T>(object primaryKey)
        {
            var index = 0;
            var pd = PocoDataFactory.ForType(typeof(T));
            var primaryKeyValuePairs = GetPrimaryKeyValues(this, pd, pd.TableInfo.PrimaryKey, primaryKey, primaryKey is T);
            var sql = AutoSelectHelper.AddSelectClause(this, typeof(T), string.Format("WHERE {0}", BuildPrimaryKeySql(this, primaryKeyValuePairs, ref index)));
            var args = primaryKeyValuePairs.Select(x => x.Value).ToArray();
            return new Sql(true, sql, args);
        }

        /// <summary>
        /// 查询类型 T 的唯一一行。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>唯一结果。</returns>
        public T Single<T>(string sql, params object[] args)
        {
            return Query<T>(sql, args).Single();
        }

        /// <summary>
        /// 查询类型 T 的唯一一行并映射到现有实例。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">接收映射结果的实例。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>唯一结果。</returns>
        public T SingleInto<T>(T instance, string sql, params object[] args)
        {
            return Query(instance, new Sql(sql, args)).Single();
        }

        /// <summary>
        /// 查询类型 T 的唯一一行，若无结果返回默认值。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>唯一结果或默认值。</returns>
        public T? SingleOrDefault<T>(string sql, params object[] args)
        {
            return Query<T>(sql, args).SingleOrDefault();
        }

        /// <summary>
        /// 查询类型 T 的唯一一行并映射到现有实例，若无结果返回默认值。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">接收映射结果的实例。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>唯一结果或默认值。</returns>
        public T? SingleOrDefaultInto<T>(T instance, string sql, params object[] args)
        {
            return Query(instance, new Sql(sql, args)).SingleOrDefault();
        }

        /// <summary>
        /// 查询类型 T 的第一行。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>第一行结果。</returns>
        public T First<T>(string sql, params object[] args)
        {
            return Query<T>(sql, args).First();
        }

        /// <summary>
        /// 查询类型 T 的第一行并映射到现有实例。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">接收映射结果的实例。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>第一行结果。</returns>
        public T FirstInto<T>(T instance, string sql, params object[] args)
        {
            return Query(instance, new Sql(sql, args)).First();
        }

        /// <summary>
        /// 查询类型 T 的第一行，若无结果返回默认值。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>第一行结果或默认值。</returns>
        public T? FirstOrDefault<T>(string sql, params object[] args)
        {
            return Query<T>(sql, args).FirstOrDefault();
        }

        /// <summary>
        /// 查询类型 T 的第一行并映射到现有实例，若无结果返回默认值。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">接收映射结果的实例。</param>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>第一行结果或默认值。</returns>
        public T? FirstOrDefaultInto<T>(T instance, string sql, params object[] args)
        {
            return Query(instance, new Sql(sql, args)).FirstOrDefault();
        }

        /// <summary>
        /// 查询类型 T 的唯一一行。
        /// </summary>
        public T Single<T>(Sql sql)
        {
            return Query<T>(sql).Single();
        }

        /// <summary>
        /// 查询类型 T 的唯一一行并映射到现有实例。
        /// </summary>
        public T SingleInto<T>(T instance, Sql sql)
        {
            return Query(instance, sql).Single();
        }

        /// <summary>
        /// 查询类型 T 的唯一一行，若无结果返回默认值。
        /// </summary>
        public T? SingleOrDefault<T>(Sql sql)
        {
            return Query<T>(sql).SingleOrDefault();
        }

        /// <summary>
        /// 查询类型 T 的唯一一行并映射到现有实例，若无结果返回默认值。
        /// </summary>
        public T? SingleOrDefaultInto<T>(T instance, Sql sql)
        {
            return Query(instance, sql).SingleOrDefault();
        }

        /// <summary>
        /// 查询类型 T 的第一行。
        /// </summary>
        public T First<T>(Sql sql)
        {
            return Query<T>(sql).First();
        }

        /// <summary>
        /// 查询类型 T 的第一行并映射到现有实例。
        /// </summary>
        public T FirstInto<T>(T instance, Sql sql)
        {
            return Query(instance, sql).First();
        }

        /// <summary>
        /// 查询类型 T 的第一行，若无结果返回默认值。
        /// </summary>
        public T? FirstOrDefault<T>(Sql sql)
        {
            return Query<T>(sql).FirstOrDefault();
        }

        /// <summary>
        /// 查询类型 T 的第一行并映射到现有实例，若无结果返回默认值。
        /// </summary>
        public T? FirstOrDefaultInto<T>(T instance, Sql sql)
        {
            return Query(instance, sql).FirstOrDefault();
        }

        // Insert an annotated poco object
        /// <summary>
        /// 插入 POCO 对象，表名、主键等信息由类型特性或约定确定。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="poco">要插入的对象。</param>
        /// <returns>新记录的主键值。</returns>
        public object Insert<T>(T poco)
        {
            if (poco == null) throw new ArgumentNullException(nameof(poco));
            var tableInfo = PocoDataFactory.TableInfoForType(poco.GetType());
            return Insert(tableInfo.TableName, tableInfo.PrimaryKey, tableInfo.AutoIncrement, poco);
        }

        /// <summary>
        /// 将 POCO 对象插入到指定表。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">要插入的对象。</param>
        /// <returns>新记录的主键值。</returns>
        public object Insert<T>(string tableName, string primaryKeyName, T poco)
        {
            return Insert(tableName, primaryKeyName, true, poco);
        }

        // Insert a poco into a table.  If the poco has a property with the same name
        // as the primary key the id of the new record is assigned to it.  Either way,
        // the new id is returned.
        /// <summary>
        /// 将 POCO 对象插入到指定表，并指定主键是否自增。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="autoIncrement">主键是否由数据库自动生成。</param>
        /// <param name="poco">要插入的对象。</param>
        /// <returns>新记录的主键值。</returns>
        public virtual object Insert<T>(string tableName, string primaryKeyName, bool autoIncrement, T poco)
        {
            var pd = PocoDataFactory.ForObject(poco, primaryKeyName, autoIncrement);
            return InsertAsyncImp(pd, tableName, primaryKeyName, autoIncrement, poco, true).RunSync();
        }

        /// <summary>
        /// 批量插入 POCO 集合。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="pocos">要插入的对象集合。</param>
        /// <param name="options">批量插入选项。</param>
        /// <returns>受影响的行数。</returns>
        public int InsertBatch<T>(IEnumerable<T> pocos, BatchOptions? options = null)
        {
            return InsertBatchAsyncImp(pocos, options, true).RunSync();
        }

        /// <summary>
        /// 批量插入 POCO 集合。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="pocos">要插入的对象集合。</param>
        /// <param name="options">批量插入选项。</param>
        public void InsertBulk<T>(IEnumerable<T> pocos, InsertBulkOptions? options = null)
        {
            try
            {
                OpenSharedConnectionInternal();
                _dbType.InsertBulk(this, pocos, options);
            }
            catch (Exception x)
            {
                OnExceptionInternal(x);
                throw;
            }
            finally
            {
                CloseSharedConnectionInternal();
            }
        }

        /// <summary>
        /// 更新指定表中的记录。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">包含更新值的对象。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <returns>受影响的行数。</returns>
        public int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            return Update(tableName, primaryKeyName, poco, primaryKeyValue, null);
        }

        /// <summary>
        /// 更新指定表中的记录，可指定要更新的列。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">包含更新值的对象。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <param name="columns">要更新的列集合。</param>
        /// <returns>受影响的行数。</returns>
        public virtual int Update(string tableName, string primaryKeyName, object poco, object? primaryKeyValue, IEnumerable<string>? columns)
        {
            return UpdateImpAsync(tableName, primaryKeyName, poco, primaryKeyValue, columns, true).RunSync();
        }

        /// <summary>
        /// 批量更新 POCO 集合。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="pocos">要更新的对象集合。</param>
        /// <param name="options">批量更新选项。</param>
        /// <returns>受影响的行数。</returns>
        public int UpdateBatch<T>(IEnumerable<UpdateBatch<T>> pocos, BatchOptions? options = null)
        {
            return UpdateBatchAsyncImp(pocos, options, true).RunSync();
        }

        // Update a record with values from a poco.  primary key value can be either supplied or read from the poco
        private async Task<int> UpdateImpAsync(string tableName, string primaryKeyName, object poco, object? primaryKeyValue, IEnumerable<string>? columns, bool sync, CancellationToken cancellationToken = default)
        {
            if (!OnUpdatingInternal(new UpdateContext(poco, tableName, primaryKeyName, primaryKeyValue, columns)))
                return 0;

            if (columns != null && !columns.Any())
                return 0;

            var pd = PocoDataFactory.ForObject(poco, primaryKeyName, true);
            var preparedStatement = UpdateStatements.PrepareUpdate(this, pd, tableName, primaryKeyName, poco, primaryKeyValue, columns);
            if (preparedStatement.Sql == null)
                return 0;

            var result = sync
                ? Execute(preparedStatement.Sql, preparedStatement.Rawvalues.ToArray())
                : await ExecuteAsync(preparedStatement.Sql, preparedStatement.Rawvalues.ToArray(), cancellationToken).ConfigureAwait(false);

            if (result == 0 && !string.IsNullOrEmpty(preparedStatement.VersionName) && VersionException == VersionExceptionHandling.Exception)
            {
                var exception = new DBConcurrencyException(string.Format("A Concurrency update occurred in table '{0}' for primary key value(s) = '{1}' and version = '{2}'", tableName,
                    string.Join(",", preparedStatement.PrimaryKeyValuePairs.Values.Select(x => x.ToString()).ToArray()), preparedStatement.VersionValue));

                OnExceptionInternal(exception);
                throw exception;
            }

            // Set Version
            if (!string.IsNullOrEmpty(preparedStatement.VersionName) && preparedStatement.VersionColumnType == VersionColumnType.Number)
            {
                PocoColumn? pc;
                if (preparedStatement.PocoData.Columns.TryGetValue(preparedStatement.VersionName, out pc))
                {
                    pc.SetValue(poco, Convert.ChangeType(Convert.ToInt64(preparedStatement.VersionValue) + 1, pc.MemberInfoData.MemberType));
                }
            }

            return result;
        }

        internal static string BuildPrimaryKeySql(Database database, Dictionary<string, object> primaryKeyValuePair, ref int index)
        {
            var tempIndex = index;
            index += primaryKeyValuePair.Count;
            return string.Join(" AND ", primaryKeyValuePair.Select((x, i) => x.Value == null || x.Value == DBNull.Value ? string.Format("{0} IS NULL", database.DatabaseType.EscapeSqlIdentifier(x.Key)) : string.Format("{0} = @{1}", database.DatabaseType.EscapeSqlIdentifier(x.Key), tempIndex + i)).ToArray());
        }

        internal static Dictionary<string, object> GetPrimaryKeyValues(Database database, PocoData? pocoData, string primaryKeyName, object primaryKeyValueOrPoco, bool isPoco)
        {
            Dictionary<string, object> primaryKeyValues;

            var multiplePrimaryKeysNames = primaryKeyName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            if (isPoco == false)
            {
                if (multiplePrimaryKeysNames.Length == 1)
                {
                    primaryKeyValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { primaryKeyName, primaryKeyValueOrPoco } };
                }
                else
                {
                    var dict = primaryKeyValueOrPoco as Dictionary<string, object>;
                    primaryKeyValues = dict ?? multiplePrimaryKeysNames.ToDictionary(x => x, x => primaryKeyValueOrPoco.GetType().GetProperties().Single(y => string.Equals(x, y.Name, StringComparison.OrdinalIgnoreCase)).GetValue(primaryKeyValueOrPoco, null)!, StringComparer.OrdinalIgnoreCase);
                }
            }
            else
            {
                primaryKeyValues = ProcessMapper(database, pocoData!, multiplePrimaryKeysNames.ToDictionary(x => x, x => pocoData!.Columns[x].GetValue(primaryKeyValueOrPoco), StringComparer.OrdinalIgnoreCase));
            }

            return primaryKeyValues;
        }

        internal static Dictionary<string, object> ProcessMapper(Database database, PocoData pd, Dictionary<string, object> primaryKeyValuePairs)
        {
            var keys = primaryKeyValuePairs.Keys.ToArray();
            foreach (var primaryKeyValuePair in keys)
            {
                var col = pd.Columns[primaryKeyValuePair];
                primaryKeyValuePairs[primaryKeyValuePair] = database.ProcessMapper(col, primaryKeyValuePairs[primaryKeyValuePair]);
            }
            return primaryKeyValuePairs;
        }

        /// <summary>
        /// 获取用于构建 UPDATE 语句的流式更新提供程序。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>更新查询提供程序。</returns>
        public IUpdateQueryProvider<T> UpdateMany<T>()
        {
            return new UpdateQueryProvider<T>(this);
        }

        /// <summary>
        /// 更新指定表中的记录。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">包含更新值的对象。</param>
        /// <returns>受影响的行数。</returns>
        public int Update(string tableName, string primaryKeyName, object poco)
        {
            return Update(tableName, primaryKeyName, poco, null);
        }

        /// <summary>
        /// 更新指定表中的记录，可指定要更新的列。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">包含更新值的对象。</param>
        /// <param name="columns">要更新的列集合。</param>
        /// <returns>受影响的行数。</returns>
        public int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string>? columns)
        {
            return Update(tableName, primaryKeyName, poco, null, columns);
        }

        /// <summary>
        /// 更新对象，仅更新指定的列。
        /// </summary>
        /// <param name="poco">包含更新值的对象。</param>
        /// <param name="columns">要更新的列集合。</param>
        /// <returns>受影响的行数。</returns>
        public int Update(object poco, IEnumerable<string> columns)
        {
            return Update(poco, null, columns);
        }

        /// <summary>
        /// 更新对象，仅更新表达式指定的字段。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="poco">包含更新值的对象。</param>
        /// <param name="fields">指定要更新字段的表达式。</param>
        /// <returns>受影响的行数。</returns>
        public int Update<T>(T poco, Expression<Func<T, object>> fields)
        {
            if (poco == null) throw new ArgumentNullException(nameof(poco));
            var expression = DatabaseType.ExpressionVisitor<T>(this, PocoDataFactory.ForType(typeof(T)));
            expression = expression.Select(fields);
            var columnNames = ((ISqlExpression)expression).SelectMembers.Select(x => x.PocoColumn.ColumnName);
            var otherNames = ((ISqlExpression)expression).GeneralMembers.Select(x => x.PocoColumn.ColumnName);
            return Update(poco, columnNames.Union(otherNames));
        }

        /// <summary>
        /// 按约定或配置更新对象。
        /// </summary>
        /// <param name="poco">包含更新值的对象。</param>
        /// <returns>受影响的行数。</returns>
        public int Update(object poco)
        {
            return Update(poco, null, null);
        }

        /// <summary>
        /// 使用指定主键值更新对象。
        /// </summary>
        /// <param name="poco">包含更新值的对象。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <returns>受影响的行数。</returns>
        public int Update(object poco, object primaryKeyValue)
        {
            return Update(poco, primaryKeyValue, null);
        }

        /// <summary>
        /// 使用指定主键值与列集合更新对象。
        /// </summary>
        /// <param name="poco">包含更新值的对象。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <param name="columns">要更新的列集合。</param>
        /// <returns>受影响的行数。</returns>
        public int Update(object poco, object? primaryKeyValue, IEnumerable<string>? columns)
        {
            var tableInfo = PocoDataFactory.TableInfoForType(poco.GetType());
            return Update(tableInfo.TableName, tableInfo.PrimaryKey, poco, primaryKeyValue, columns);
        }

        /// <summary>
        /// 根据 SQL 片段更新类型 T 对应表中的记录。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">UPDATE 语句的 WHERE/SET 等片段。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>受影响的行数。</returns>
        public int Update<T>(string sql, params object[] args)
        {
            var tableInfo = PocoDataFactory.TableInfoForType(typeof(T));
            return Execute($"UPDATE {_dbType.EscapeTableName(tableInfo.TableName)} {sql}", args);
        }

        /// <summary>
        /// 根据 SQL 片段更新类型 T 对应表中的记录。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>受影响的行数。</returns>
        public int Update<T>(Sql sql)
        {
            var tableInfo = PocoDataFactory.TableInfoForType(typeof(T));
            return Execute(new Sql($"UPDATE {_dbType.EscapeTableName(tableInfo.TableName)}").Append(sql));
        }

        /// <summary>
        /// 获取用于构建 DELETE 语句的流式删除提供程序。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>删除查询提供程序。</returns>
        public IDeleteQueryProvider<T> DeleteMany<T>()
        {
            return new DeleteQueryProvider<T>(this);
        }

        /// <summary>
        /// 删除指定表中的记录。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">要删除的对象。</param>
        /// <returns>受影响的行数。</returns>
        public int Delete(string tableName, string primaryKeyName, object poco)
        {
            return Delete(tableName, primaryKeyName, poco, null);
        }

        /// <summary>
        /// 删除指定表中的记录。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">要删除的对象。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <returns>受影响的行数。</returns>
        public virtual int Delete(string tableName, string primaryKeyName, object? poco, object? primaryKeyValue)
        {
            return DeleteImpAsync(tableName, primaryKeyName, poco, primaryKeyValue, true).RunSync();
        }

        private async Task<int> DeleteImpAsync(string tableName, string primaryKeyName, object? poco, object? primaryKeyValue, bool sync, CancellationToken cancellationToken = default)
        {
            if (!OnDeletingInternal(new DeleteContext(poco, tableName, primaryKeyName, primaryKeyValue)))
                return 0;

            var pd = poco != null ? PocoDataFactory.ForObject(poco, primaryKeyName, true) : null;
            var primaryKeyValuePairs = GetPrimaryKeyValues(this, pd, primaryKeyName, primaryKeyValue ?? poco!, primaryKeyValue == null);

            // Do it
            var index = 0;
            var sql = $"DELETE FROM {_dbType.EscapeTableName(tableName)} WHERE {BuildPrimaryKeySql(this, primaryKeyValuePairs, ref index)}";
            var rawValues = primaryKeyValuePairs.Select(x => x.Value).ToList();

            var versionColumn = pd?.Columns.Where(x => x.Value.VersionColumn).Select(x => x.Value).SingleOrDefault();
            string? versionName = null;
            object? versionValue = null;
            if (versionColumn != null)
            {
                versionName = versionColumn.ColumnName;
                versionValue = versionColumn.GetColumnValue(pd, poco, this.ProcessMapper);

                if (!string.IsNullOrEmpty(versionName))
                {
                    sql += $" AND {DatabaseType.EscapeSqlIdentifier(versionName)} = @{index++}";
                    rawValues.Add(versionValue);
                }
            }

            var result = sync
                ? Execute(sql, rawValues.ToArray())
                : await ExecuteAsync(sql, rawValues.ToArray(), cancellationToken).ConfigureAwait(false);

            if (result == 0 && !string.IsNullOrEmpty(versionName) && VersionException == VersionExceptionHandling.Exception)
            {
                var exception = new DBConcurrencyException(string.Format("A Concurrency update occurred in table '{0}' for primary key value(s) = '{1}' and version = '{2}'", tableName,
                    string.Join(",", primaryKeyValuePairs.Values.Select(x => x?.ToString()).ToArray()), versionValue));

                OnExceptionInternal(exception);
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// 删除对象对应的记录。
        /// </summary>
        /// <param name="poco">要删除的对象。</param>
        /// <returns>受影响的行数。</returns>
        public int Delete(object poco)
        {
            var tableInfo = PocoDataFactory.TableInfoForType(poco.GetType());
            return Delete(tableInfo.TableName, tableInfo.PrimaryKey, poco);
        }

        /// <summary>
        /// 按类型删除记录，参数可以是对象或主键值。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="pocoOrPrimaryKey">要删除的对象或主键值。</param>
        /// <returns>受影响的行数。</returns>
        public int Delete<T>(object pocoOrPrimaryKey)
        {
            if (pocoOrPrimaryKey.GetType() == typeof(T))
                return Delete(pocoOrPrimaryKey);
            var tableInfo = PocoDataFactory.TableInfoForType(typeof(T));
            return Delete(tableInfo.TableName, tableInfo.PrimaryKey, null, pocoOrPrimaryKey);
        }

        /// <summary>
        /// 根据 SQL 片段删除类型 T 对应表中的记录。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">DELETE 语句的 WHERE 等片段。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>受影响的行数。</returns>
        public int Delete<T>(string sql, params object[] args)
        {
            var tableInfo = PocoDataFactory.TableInfoForType(typeof(T));
            return Execute($"DELETE FROM {_dbType.EscapeTableName(tableInfo.TableName)} {sql}", args);
        }

        /// <summary>
        /// 根据 SQL 片段删除类型 T 对应表中的记录。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">封装 SQL 与参数的对象。</param>
        /// <returns>受影响的行数。</returns>
        public int Delete<T>(Sql sql)
        {
            var tableInfo = PocoDataFactory.TableInfoForType(typeof(T));
            return Execute(new Sql($"DELETE FROM {_dbType.EscapeTableName(tableInfo.TableName)}").Append(sql));
        }

        /// <summary>Checks if a poco represents a new record.</summary>
        public bool IsNew<T>(T poco)
        {
            return IsNewAsync(poco, true).RunSync();
        }

        private async Task<bool> IsNewAsync<T>(T poco, bool sync, CancellationToken cancellationToken = default)
        {
            if (poco == null) throw new ArgumentNullException(nameof(poco));
            if (poco is System.Dynamic.ExpandoObject || poco is PocoExpando)
            {
                return true;
            }

            var pd = PocoDataFactory.ForType(poco.GetType());
            object pk;
            PocoColumn? pc;

            if (pd.Columns.TryGetValue(pd.TableInfo.PrimaryKey, out pc))
            {
                pk = pc.GetValue(poco);
            }
            else if (pd.TableInfo.PrimaryKey.Contains(","))
            {
                return sync
                    ? !PocoExistsAsync(poco, true).RunSync()
                    : !await PocoExistsAsync(poco, false, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var pi = poco.GetType().GetProperty(pd.TableInfo.PrimaryKey);
                if (pi == null) throw new ArgumentException(string.Format("The object doesn't have a property matching the primary key column name '{0}'", pd.TableInfo.PrimaryKey));
                pk = pi.GetValue(poco, null)!;
            }

            if (pk == null)
                return true;

            if (!pd.TableInfo.AutoIncrement)
            {
                return sync
                    ? !ExistsAsync<T>(pk, true).RunSync()
                    : !await ExistsAsync<T>(pk, false).ConfigureAwait(false);
            }

            var type = pk.GetType();

            if (type.GetTypeInfo().IsValueType)
            {
                // Common primary key types
                if (type == typeof(long)) return (long)pk == default(long);
                if (type == typeof(ulong)) return (ulong)pk == default(ulong);
                if (type == typeof(int)) return (int)pk == default(int);
                if (type == typeof(uint)) return (uint)pk == default(uint);
                if (type == typeof(Guid)) return (Guid)pk == default(Guid);

                // Create a default instance and compare
                return pk == Activator.CreateInstance(pk.GetType());
            }

            return false;
        }

        /// <summary>
        /// 保存实体：若该对象为新记录则执行插入，否则执行更新。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="poco">要保存的实体对象。</param>
        // Insert new record or Update existing record
        public void Save<T>(T poco)
        {
            if (poco == null) throw new ArgumentNullException(nameof(poco));
            var tableInfo = PocoDataFactory.TableInfoForType(poco.GetType());
            if (IsNew(poco))
            {
                Insert(tableInfo.TableName, tableInfo.PrimaryKey, tableInfo.AutoIncrement, poco);
            }
            else
            {
                Update(tableInfo.TableName, tableInfo.PrimaryKey, poco);
            }
        }

        /// <summary>
        /// 设置该数据库实例所有命令的超时时间（秒）；为 0 时使用提供程序的默认值。
        /// </summary>
        public int CommandTimeout { get; set; }
        /// <summary>
        /// 仅为下一条命令设置超时时间（秒），执行一次后自动还原。
        /// </summary>
        public int OneTimeCommandTimeout { get; set; }

        void DoPreExecute(DbCommand cmd)
        {
            // Setup command timeout
            if (OneTimeCommandTimeout != 0)
            {
                cmd.CommandTimeout = OneTimeCommandTimeout;
                OneTimeCommandTimeout = 0;
            }
            else if (CommandTimeout != 0)
            {
                cmd.CommandTimeout = CommandTimeout;
            }

            // Call hook
            OnExecutingCommandInternal(cmd);

            // Save it
            _lastSql = cmd.CommandText;
            _lastParams = cmd.Parameters;
        }

        /// <summary>
        /// 最近一次执行命令的 SQL 文本。
        /// </summary>
        public string? LastSQL => _lastSql;
        /// <summary>
        /// 最近一次执行命令所使用的参数值数组。
        /// </summary>
        public object[]? LastArgs => _lastParams?.Cast<DbParameter>().Select(x => x.Value!).ToArray();

        /// <summary>
        /// 最近一次执行命令的完整 SQL（已替换参数占位符）。
        /// </summary>
        public string LastCommand => FormatCommand(_lastSql, _lastParams?.Cast<object>().ToArray() ?? []);

        /// <summary>
        /// 将命令格式化为可读的 SQL 字符串。
        /// </summary>
        /// <param name="cmd">要格式化的数据库命令。</param>
        /// <returns>格式化后的 SQL 字符串。</returns>
        public virtual string FormatCommand(DbCommand cmd)
        {
            return _dbType.FormatCommand(cmd);
        }

        /// <summary>
        /// 将 SQL 与参数格式化为可读的 SQL 字符串。
        /// </summary>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="args">参数数组。</param>
        /// <returns>格式化后的 SQL 字符串。</returns>
        public string FormatCommand(string? sql, object[]? args)
        {
            return _dbType.FormatCommand(sql, args);
        }

        private List<IInterceptor>? _interceptors;
        /// <summary>
        /// 获取当前数据库实例的拦截器集合，用于在命令执行前后插入自定义逻辑。
        /// </summary>
        public List<IInterceptor> Interceptors => _interceptors ??= new List<IInterceptor>();

        private IMapperCollection? _mappers;
        /// <summary>
        /// 获取或设置列映射器集合，用于配置实体与数据库列之间的映射。
        /// </summary>
        public IMapperCollection Mappers
        {
            get => _mappers ??= new MapperCollection();
            set => _mappers = value;
        }

        private IPocoDataFactory? _pocoDataFactory;
        /// <summary>
        /// 获取或设置 POCO 数据工厂，用于生成实体类型对应的表信息。
        /// </summary>
        public IPocoDataFactory PocoDataFactory
        {
            get => _pocoDataFactory ??= new PocoDataFactory(Mappers);
            set => _pocoDataFactory = value;
        }

        /// <summary>
        /// 当前数据库实例使用的连接字符串。
        /// </summary>
        public string ConnectionString => _connectionString;

        // Member variables
        private readonly string _connectionString;
        private readonly string _providerName;
        private DbProviderFactory? _factory;
        private DbConnection _sharedConnection;
        private DbTransaction? _transaction;
        private IsolationLevel _isolationLevel;
        private string? _lastSql;
        private DbParameterCollection? _lastParams;
        private string _paramPrefix = "@";
        private readonly bool _connectionPassedIn;

        internal int ExecuteNonQueryHelper(DbCommand cmd)
        {
            DoPreExecute(cmd);
            var result = ExecutionHook(() => cmd.ExecuteNonQuery());
            OnExecutedCommandInternal(cmd);
            return result;
        }

        internal object ExecuteScalarHelper(DbCommand cmd)
        {
            DoPreExecute(cmd);
            var result = ExecutionHook(() => cmd.ExecuteScalar());
            OnExecutedCommandInternal(cmd);
            return result!;
        }

        internal DbDataReader ExecuteReaderHelper(DbCommand cmd)
        {
            DoPreExecute(cmd);
            var result = ExecutionHook(() => cmd.ExecuteReader());
            OnExecutedCommandInternal(cmd);
            return result!;
        }

        /// <summary>
        /// 执行钩子，允许派生类在命令执行前后包装执行逻辑。
        /// </summary>
        /// <typeparam name="T">返回值类型。</typeparam>
        /// <param name="action">要执行的委托。</param>
        /// <returns>委托的返回值。</returns>
        protected virtual T ExecutionHook<T>(Func<T> action)
        {
            return action();
        }

        /// <summary>
        /// 异步执行钩子，允许派生类在异步命令执行前后包装执行逻辑。
        /// </summary>
        /// <typeparam name="T">返回值类型。</typeparam>
        /// <param name="action">要异步执行的委托。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>委托的返回值。</returns>
        protected virtual async Task<T> ExecutionHookAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
        {
            return await action(cancellationToken).ConfigureAwait(false);
        }

        int IDatabaseHelpers.ExecuteNonQueryHelper(DbCommand cmd) => ExecuteNonQueryHelper(cmd);

        object IDatabaseHelpers.ExecuteScalarHelper(DbCommand cmd) => ExecuteScalarHelper(cmd);

        DbDataReader IDatabaseHelpers.ExecuteReaderHelper(DbCommand cmd) => ExecuteReaderHelper(cmd);

        Task<int> IDatabaseHelpers.ExecuteNonQueryHelperAsync(DbCommand cmd, CancellationToken cancellationToken) => ExecuteNonQueryHelperAsync(cmd, cancellationToken);

        Task<object> IDatabaseHelpers.ExecuteScalarHelperAsync(DbCommand cmd, CancellationToken cancellationToken) => ExecuteScalarHelperAsync(cmd, cancellationToken);

        Task<DbDataReader> IDatabaseHelpers.ExecuteReaderHelperAsync(DbCommand cmd, CancellationToken cancellationToken) => ExecuteReaderHelperAsync(cmd, cancellationToken);

        /// <summary>
        /// 判断成员是否为枚举类型（包括可空枚举）。
        /// </summary>
        /// <param name="memberInfo">成员信息。</param>
        /// <returns>若为枚举类型返回 true，否则返回 false。</returns>
        public static bool IsEnum(MemberInfoData memberInfo)
        {
            var underlyingType = Nullable.GetUnderlyingType(memberInfo.MemberType);
            return memberInfo.MemberType.GetTypeInfo().IsEnum || (underlyingType != null && underlyingType.GetTypeInfo().IsEnum);
        }
    }
}