﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.ReadyGo3.Exceptions;
using ToolGood.ReadyGo3.Gadget;
using ToolGood.ReadyGo3.PetaPoco.Core;
using ToolGood.ReadyGo3.PetaPoco.Internal;
using ToolGood.ReadyGo3.PetaPoco.Utilities;

namespace ToolGood.ReadyGo3.PetaPoco
{
    /// <summary>
    /// PetaPoco数据库链接库
    /// </summary>
    public partial class Database : IDisposable
    {
        #region IDisposable

        /// <summary>
        ///     Automatically close one open shared connection
        /// </summary>
        public void Dispose()
        {
            if (_isDisposable) return;
            if (_transactionDepth >= 1) {
                _transactionDepth = 1;
                AbortTransaction();
            }
            if (_sharedConnectionDepth >= 1) {
                _sharedConnectionDepth = 1;
                CloseSharedConnection();
            }
            _isDisposable = true;
        }

        #endregion

        #region Constructors
        /// <summary>
        /// PetaPoco数据库链接库
        /// </summary>
        /// <param name="sqlHelper"></param>
        public Database(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
            _factory = sqlHelper._factory;
            _provider = DatabaseProvider.Resolve(sqlHelper._sqlType);
            _paramPrefix = _provider.GetParameterPrefix(sqlHelper._connectionString);


            _transactionDepth = 0;
            EnableAutoSelect = true;
            KeepConnectionAlive = true;
        }

        #endregion

        #region Connection Management

        /// <summary>
        ///     When set to true the first opened connection is kept alive until this object is disposed
        /// </summary>
        public bool KeepConnectionAlive;

        /// <summary>
        ///     Open a connection that will be used for all subsequent queries.
        /// </summary>
        /// <remarks>
        ///     Calls to Open/CloseSharedConnection are reference counted and should be balanced
        /// </remarks>
        public void OpenSharedConnection()
        {
            if (_sharedConnectionDepth == 0) {
                _sharedConnection = _factory.CreateConnection();
                _sharedConnection.ConnectionString = _sqlHelper._connectionString;

                if (_sharedConnection.State == ConnectionState.Broken)
                    _sharedConnection.Close();

                if (_sharedConnection.State == ConnectionState.Closed)
                    _sharedConnection.Open();

                _sharedConnection = OnConnectionOpened(_sharedConnection);

                if (KeepConnectionAlive)
                    _sharedConnectionDepth++; // Make sure you call Dispose
            } else {
                if (_sharedConnection.State == ConnectionState.Broken)
                    _sharedConnection.Close();

                if (_sharedConnection.State == ConnectionState.Closed)
                    _sharedConnection.Open();
            }
            _sharedConnectionDepth++;
        }

        /// <summary>
        ///     Releases the shared connection
        /// </summary>
        public void CloseSharedConnection()
        {
            if (_sharedConnectionDepth > 0) {
                _sharedConnectionDepth--;
                if (_sharedConnectionDepth == 0) {
                    OnConnectionClosing();
                    _sharedConnection.Dispose();
                    _sharedConnection = null;
                }
            }
        }

        #endregion

        #region Transaction Management

        /// <summary>
        ///     Called when a transaction starts.  Overridden by the T4 template generated database
        ///     classes to ensure the same DB instance is used throughout the transaction.
        /// </summary>
        internal void OnBeginTransaction()
        {
            _sqlHelper._sqlMonitor.Transactioning();
        }

        /// <summary>
        ///     Called when a transaction ends.
        /// </summary>
        public void OnEndTransaction()
        {
            _sqlHelper._sqlMonitor.Transactioned();
        }

        /// <summary>
        ///     Starts a transaction scope, see GetTransaction() for recommended usage
        /// </summary>
        public void BeginTransaction()
        {
            _transactionDepth++;

            if (_transactionDepth == 1) {
                OpenSharedConnection();
                _transaction = !_sqlHelper._isolationLevel.HasValue ?
                    _sharedConnection.BeginTransaction() :
                    _sharedConnection.BeginTransaction(_sqlHelper._isolationLevel.Value);
                _transactionCancelled = false;
                OnBeginTransaction();
            }
        }

        /// <summary>
        ///     Internal helper to cleanup transaction
        /// </summary>
        private void CleanupTransaction()
        {
            OnEndTransaction();

            if (_transactionCancelled)
                _transaction.Rollback();
            else
                _transaction.Commit();

            _transaction.Dispose();
            _transaction = null;

            CloseSharedConnection();
        }

        /// <summary>
        ///     Aborts the entire outer most transaction scope
        /// </summary>
        /// <remarks>
        ///     Called automatically by Transaction.Dispose()
        ///     if the transaction wasn't completed.
        /// </remarks>
        public void AbortTransaction()
        {
            if (_isDisposable == false) {
                _transactionCancelled = true;
                if ((--_transactionDepth) == 0)
                    CleanupTransaction();
            }
        }

        /// <summary>
        ///     Marks the current transaction scope as complete.
        /// </summary>
        public void CompleteTransaction()
        {
            if (_isDisposable == false) {
                if ((--_transactionDepth) == 0)
                    CleanupTransaction();
            }
        }

        #endregion

        #region Command Management

        /// <summary>
        ///     Add a parameter to a DB command
        /// </summary>
        /// <param name="cmd">A reference to the IDbCommand to which the parameter is to be added</param>
        /// <param name="value">The value to assign to the parameter</param>
        /// <param name="pi">Optional, a reference to the property info of the POCO property from which the value is coming.</param>
        private void AddParam(IDbCommand cmd, object value, PropertyInfo pi)
        {
            //// Convert value to from poco type to db type
            //if (pi != null) {
            //    var mapper = Singleton<StandardMapper>.Instance;
            //    var fn = mapper.GetToDbConverter(pi);
            //    if (fn != null)
            //        value = fn(value);
            //}

            var dbParam = value as SqlParameter;
            if (dbParam != null) {
                CreateParam(cmd, dbParam.Value, dbParam.ParameterName, null, dbParam.Size, dbParam.Scale, dbParam.DbType, dbParam.ParameterDirection);
                return;
            }

            // Support passed in parameters
            var idbParam = value as IDbDataParameter;
            if (idbParam != null) {
                idbParam.ParameterName = string.Format("{0}{1}", _paramPrefix, cmd.Parameters.Count);
                cmd.Parameters.Add(idbParam);
                return;
            }
            CreateParam(cmd, value, cmd.Parameters.Count.ToString(), pi, null, null, null, null);
        }
        private void CreateParam(IDbCommand cmd, object value, string name, PropertyInfo pi, int? size, byte? scale, DbType? dbType, ParameterDirection? parameterDirection)
        {
            // Create the parameter
            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("{0}{1}", _paramPrefix, name);


            // Assign the parmeter value
            if (value == null) {
                p.Value = DBNull.Value;

                if (pi != null && pi.PropertyType.Name == "Byte[]") {
                    p.DbType = DbType.Binary;
                }
            } else {
                // Give the database type first crack at converting to DB required type
                value = _provider.MapParameterValue(value);

                var t = value.GetType();
                if (t.IsEnum) {
                    // PostgreSQL .NET driver wont cast enum to int
                    p.Value = Convert.ChangeType(value, ((Enum)value).GetTypeCode());
                } else if (t == typeof(Guid) && !_provider.HasNativeGuidSupport) {
                    p.Value = value.ToString();
                    p.DbType = DbType.String;
                    p.Size = 40;
                } else if (t == typeof(string)) {
                    // out of memory exception occurs if trying to save more than 4000 characters to SQL Server CE NText column. Set before attempting to set Size, or Size will always max out at 4000
                    if ((value as string).Length + 1 > 4000 && p.GetType().Name == "SqlCeParameter")
                        p.GetType().GetProperty("SqlDbType").SetValue(p, SqlDbType.NText, null);

                    p.Size = Math.Max((value as string).Length + 1, 4000); // Help query plan caching by using common size
                    p.Value = value;
                } else if (t == typeof(AnsiString)) {
                    // Thanks @DataChomp for pointing out the SQL Server indexing performance hit of using wrong string type on varchar
                    p.Size = Math.Max((value as AnsiString).Value.Length + 1, 4000);
                    p.Value = (value as AnsiString).Value;
                    p.DbType = DbType.AnsiString;
                } else if (value.GetType().Name == "SqlGeography") {
                    //SqlGeography is a CLR Type
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geography", null); //geography is the equivalent SQL Server Type
                    p.Value = value;
                } else if (value.GetType().Name == "SqlGeometry") {
                    //SqlGeometry is a CLR Type
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geometry", null); //geography is the equivalent SQL Server Type
                    p.Value = value;
                } else if (t == typeof(DateTime) || t == typeof(DateTime?)) {
                    // 在sql server 内会影响查询速度 
                    p.Value = value;
                    p.DbType = DbType.DateTime;
                } else if (t == typeof(TimeSpan) || t == typeof(TimeSpan?)) {
                    p.Value = value;
                    p.DbType = DbType.DateTime;
                } else if (t == typeof(DateTimeOffset) || t == typeof(DateTimeOffset?)) {
                    p.Value = value;
                    p.DbType = DbType.DateTimeOffset;
                } else {
                    p.Value = value;
                }
            }
            if (scale != null) { p.Precision = scale.Value; }
            if (parameterDirection != null) { p.Direction = parameterDirection.Value; }
            if (dbType != null) { p.DbType = dbType.Value; }
            if (size != null) { p.Size = size.Value; }

            // Add to the collection
            cmd.Parameters.Add(p);
        }


        // Create a command
        private static readonly Regex rxParamsPrefix = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public IDbCommand CreateCommand(IDbConnection connection, string sql, object[] args, CommandType commandType = CommandType.Text)
        {
            // Perform parameter prefix replacements
            if (_paramPrefix != "@")
                sql = rxParamsPrefix.Replace(sql, m => _paramPrefix + m.Value.Substring(1));

            // Create the command and add parameters
            IDbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = sql;
            cmd.Transaction = _transaction;
            cmd.CommandType = commandType;
            if (args != null) {
                if (commandType == CommandType.StoredProcedure) {
                    foreach (var item in args) {
                        var idbParam = item as IDbDataParameter;
                        cmd.Parameters.Add(idbParam);
                    }
                } else {
                    foreach (var item in args) {
                        var items = item as IList;
                        if (items!=null) {
                            foreach (var obj in items) {
                                AddParam(cmd, obj, null);
                            }
                        } else {
                            AddParam(cmd, item, null);

                        }
                    }
                }
            }


            // Notify the DB type
            _provider.PreExecute(cmd);

            // Call logging
            if (!String.IsNullOrEmpty(sql))
                DoPreExecute(cmd);

            return cmd;
        }

        #endregion

        #region Exception Reporting and Logging

        /// <summary>
        ///     Called if an exception occurs during processing of a DB operation.  Override to provide custom logging/handling.
        /// </summary>
        /// <param name="x">The exception instance</param>
        /// <returns>True to re-throw the exception, false to suppress it</returns>
        public bool OnException(Exception x)
        {
            System.Diagnostics.Debug.WriteLine(x.ToString());
            System.Diagnostics.Debug.WriteLine(_sqlHelper._sql.LastCommand);

            _sqlHelper._sqlMonitor.Exception(x.Message);
            _sqlHelper._sql.LastErrorMessage = x.Message;

            return true;
        }

        /// <summary>
        ///     Called when DB connection opened
        /// </summary>
        /// <param name="conn">The newly opened IDbConnection</param>
        /// <returns>The same or a replacement IDbConnection</returns>
        /// <remarks>
        ///     Override this method to provide custom logging of opening connection, or
        ///     to provide a proxy IDbConnection.
        /// </remarks>
        public IDbConnection OnConnectionOpened(IDbConnection conn)
        {
            _sqlHelper._sqlMonitor.ConnectionOpened();

            return conn;
        }

        /// <summary>
        ///     Called when DB connection closed
        /// </summary>
        /// <param name="conn">The soon to be closed IDBConnection</param>
        public void OnConnectionClosing()
        {
            _sqlHelper._sqlMonitor.ConnectionClosing();
        }

        /// <summary>
        ///     Called just before an DB command is executed
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        /// <remarks>
        ///     Override this method to provide custom logging of commands and/or
        ///     modification of the IDbCommand before it's executed
        /// </remarks>
        public void OnExecutingCommand(IDbCommand cmd)
        {
            var objs = (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray();
            _sqlHelper._sql.LastSQL = cmd.CommandText;
            _sqlHelper._sql.LastArgs = objs;
            _sqlHelper._events.OnExecutingCommand(cmd.CommandText, objs);

            var objs2 = (from IDataParameter parameter in cmd.Parameters select parameter).ToArray();
            _sqlHelper._sqlMonitor.ExecutingCommand(cmd.CommandText, objs2);
        }

        /// <summary>
        ///     Called on completion of command execution
        /// </summary>
        /// <param name="cmd">The IDbCommand that finished executing</param>
        public void OnExecutedCommand(IDbCommand cmd)
        {
            var objs = (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray();
            _sqlHelper._sql.LastSQL = cmd.CommandText;
            _sqlHelper._sql.LastArgs = objs;
            _sqlHelper._sqlMonitor.ExecutedCommand(cmd.CommandText, objs);

            _sqlHelper._events.OnExecutedCommand(cmd.CommandText, objs);
        }

        #endregion

        #region operation: Execute  ExecuteScalar ExecuteDataTable ExecuteDataSet

        /// <summary>
        ///     Executes a non-query command
        /// </summary>
        /// <param name="sql">The SQL statement to execute</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <param name="commandType"></param>
        /// <returns>The number of rows affected</returns>
        public int Execute(string sql, object[] args, CommandType commandType = CommandType.Text)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args, commandType)) {
                        var retv = cmd.ExecuteNonQuery();
                        OnExecutedCommand(cmd);
                        return retv;
                    }
                } finally {
                    CloseSharedConnection();
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                return -1;
            }
        }

        /// <summary>
        ///     Executes a query and return the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to</typeparam>
        /// <param name="sql">The SQL query to execute</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <param name="commandType"></param>
        /// <returns>The scalar value cast to T</returns>
        public T ExecuteScalar<T>(string sql, object[] args, CommandType commandType = CommandType.Text)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args, commandType)) {
                        object val = cmd.ExecuteScalar();
                        OnExecutedCommand(cmd);

                        // Handle nullable types
                        Type u = Nullable.GetUnderlyingType(typeof(T));
                        if (u != null && (val == null || val == DBNull.Value))
                            return default;

                        return (T)Convert.ChangeType(val, u ?? typeof(T));
                    }
                } finally {
                    CloseSharedConnection();
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                return default;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql, object[] args, CommandType commandType = CommandType.Text)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args, commandType)) {
                        var reader = cmd.ExecuteReader();
                        DataTable dt = new DataTable();
                        bool init = false;
                        dt.BeginLoadData();
                        object[] vals = new object[0];
                        while (reader.Read()) {
                            if (!init) {
                                init = true;
                                int fieldCount = reader.FieldCount;
                                for (int i = 0; i < fieldCount; i++) {
                                    dt.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                                }
                                vals = new object[fieldCount];
                            }
                            reader.GetValues(vals);
                            dt.LoadDataRow(vals, true);
                        }
                        reader.Close();
                        dt.EndLoadData();
                        OnExecutedCommand(cmd);
                        return dt;
                    }
                } finally {
                    CloseSharedConnection();
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                return default;
            }
        }
        //#if !NETSTANDARD2_0
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string sql, object[] args, CommandType commandType = CommandType.Text)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args, commandType)) {
                        using (var adapter = _factory.CreateDataAdapter()) {
                            adapter.SelectCommand = (DbCommand)cmd;
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            OnExecutedCommand(cmd);
                            return ds;
                        }
                    }
                } finally {
                    CloseSharedConnection();
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                return default;
            }
        }
        //#endif

        #endregion

        #region operation: Page

        /// <summary>
        ///     Starting with a regular SELECT statement, derives the SQL statements required to query a
        ///     DB for a page of records and the total number of records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="skip">The number of rows to skip before the start of the page</param>
        /// <param name="take">The number of rows in the page</param>
        /// <param name="sql">The original SQL select statement</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <param name="sqlCount">Outputs the SQL statement to query for the total number of matching rows</param>
        /// <param name="sqlPage">Outputs the SQL statement to retrieve a single page of matching rows</param>
        private void BuildPageQueries<T>(long skip, long take, string sql, ref object[] args, out string sqlCount, out string sqlPage)
        {
            // Add auto select clause
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql);

            // Split the SQL
            if (!_provider.PagingUtility.SplitSQL(sql, out SQLParts parts))
                throw new Exception("Unable to parse SQL statement for paged query");

            sqlPage = _provider.BuildPageQuery(skip, take, parts, ref args);
            sqlCount = parts.SqlCount;
        }

        /// <summary>
        ///     Retrieves a page of records	and the total number of available records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="page">The 1 based page number to retrieve</param>
        /// <param name="itemsPerPage">The number of records per page</param>
        /// <param name="sql">The base SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>A Page of results</returns>
        /// <remarks>
        ///     ToolGood.ReadyGo3.PetaPoco will automatically modify the supplied SELECT statement to only retrieve the
        ///     records for the specified page.  It will also execute a second query to retrieve the
        ///     total number of records in the result set.
        /// </remarks>
        public Page<T> Page<T>(long page, long itemsPerPage, string sql, object[] args)
        {
            BuildPageQueries<T>((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out string sqlCount, out string sqlPage);
            return PageSql<T>(page, itemsPerPage, sqlPage, sqlCount, args);
        }

        /// <summary>
        /// Retrieves a page of records	and the total number of available records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="page">The 1 based page number to retrieve</param>
        /// <param name="itemsPerPage">The number of records per page</param>
        /// <param name="selectSql"></param>
        /// <param name="countSql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Page<T> PageSql<T>(long page, long itemsPerPage, string selectSql, string countSql, object[] args)
        {
            // Save the one-time command time out and use it for both queries
            var saveTimeout = OneTimeCommandTimeout;

            // Setup the paged result
            var result = new Page<T> {
                CurrentPage = page,
                PageSize = itemsPerPage,
                TotalItems = ExecuteScalar<long>(countSql, args)
            };
            OneTimeCommandTimeout = saveTimeout;

            // Get the records
            result.Items = Query<T>(selectSql, args).ToList();
            // Done
            return result;
        }




        #endregion

        #region operation: Query
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(long skip, long take, string sql, object[] args)
        {
            BuildPageQueries<T>(skip, take, sql, ref args, out string sqlCount, out string sqlPage);
            return Query<T>(sqlPage, args);
        }

        /// <summary>
        ///     Runs an SQL query, returning the results as an IEnumerable collection
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="sql">The SQL query</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <param name="commandType"></param>
        /// <returns>An enumerable collection of result records</returns>
        /// <remarks>
        ///     For some DB providers, care should be taken to not start a new Query before finishing with
        ///     and disposing the previous one. In cases where this is an issue, consider using Fetch which
        ///     returns the results as a List rather than an IEnumerable.
        /// </remarks>
        public IEnumerable<T> Query<T>(string sql, object[] args, CommandType commandType = CommandType.Text)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql);

            OpenSharedConnection();
            try {
                using (var cmd = CreateCommand(_sharedConnection, sql, args, commandType)) {
                    IDataReader r;
                    var pd = PocoData.ForType(typeof(T));
                    try {
                        r = cmd.ExecuteReader();
                        OnExecutedCommand(cmd);
                    } catch (Exception x) {
                        if (OnException(x))
                            throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                        yield break;
                    }
                    var factory = pd.GetFactory(0, r.FieldCount, r) as Func<IDataReader, T>;
                    using (r) {
                        while (true) {
                            T poco;
                            try {
                                if (!r.Read())
                                    yield break;
                                poco = factory(r);
                            } catch (Exception x) {
                                if (OnException(x))
                                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                                yield break;
                            }

                            yield return poco;
                        }
                    }
                }
            } finally {
                CloseSharedConnection();
            }
        }

        #endregion

        #region operation: Exists

        /// <summary>
        ///     Checks for the existence of a row matching the specified condition
        /// </summary>
        /// <typeparam name="T">The Type representing the table being queried</typeparam>
        /// <param name="sqlCondition">The SQL expression to be tested for (ie: the WHERE expression)</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement</param>
        /// <returns>True if a record matching the condition is found.</returns>
        public bool Exists<T>(string sqlCondition, params object[] args)
        {
            var poco = PocoData.ForType(typeof(T)).TableInfo;

            if (sqlCondition.TrimStart().StartsWith("where", StringComparison.OrdinalIgnoreCase))
                sqlCondition = sqlCondition.TrimStart().Substring(5);

            return ExecuteScalar<int>(string.Format(_provider.GetExistsSql(), _provider.EscapeTableName(poco.TableName), sqlCondition), args) != 0;
        }

        /// <summary>
        ///     Checks for the existence of a row with the specified primary key value.
        /// </summary>
        /// <typeparam name="T">The Type representing the table being queried</typeparam>
        /// <param name="primaryKey">The primary key value to look for</param>
        /// <returns>True if a record with the specified primary key value exists.</returns>
        public bool Exists<T>(object primaryKey)
        {
            return Exists<T>(string.Format("{0}=@0", _provider.EscapeSqlIdentifier(PocoData.ForType(typeof(T)).TableInfo.PrimaryKey)), primaryKey);
        }

        #endregion

        #region operation: Insert

        /// <summary>
        ///     Performs an SQL Insert
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables</returns>
        /// <remarks>
        ///     The name of the table, it's primary key and whether it's an auto-allocated primary key are retrieved
        ///     from the POCO's attributes
        /// </remarks>
        public object Insert(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType());
            return ExecuteInsert(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, poco);
        }

        private object ExecuteInsert(string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new object[0])) {
                        var pd = PocoData.ForObject(poco, primaryKeyName);
                        var type = poco.GetType();
                        cmd.CommandText = CrudCache.GetInsertSql(_provider, _paramPrefix, pd, 1, tableName, primaryKeyName, autoIncrement);

                        foreach (var i in pd.Columns) {
                            if (i.Value.ResultColumn) continue;
                            if (autoIncrement && primaryKeyName != null && string.Compare(i.Value.ColumnName, primaryKeyName, true) == 0) {
                                continue;
                            }
                            AddParam(cmd, i.Value.GetValue(poco), i.Value.PropertyInfo);
                        }
 
                        if (!autoIncrement) {
                            DoPreExecute(cmd);
                            cmd.ExecuteNonQuery();
                            OnExecutedCommand(cmd);

                            if (primaryKeyName != null && pd.Columns.TryGetValue(primaryKeyName, out PocoColumn pkColumn))
                                return pkColumn.GetValue(poco);
                            else
                                return null;
                        }

                        object id = _provider.ExecuteInsert(this, cmd, primaryKeyName);

                        // Assign the ID back to the primary key property
                        if (primaryKeyName != null && !poco.GetType().Name.Contains("AnonymousType")) {
                            if (pd.Columns.TryGetValue(primaryKeyName, out PocoColumn pc)) {
                                pc.SetValue(poco, pc.ChangeType(id));
                            }
                        }

                        return id;
                    }
                } finally {
                    CloseSharedConnection();
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                return null;
            }
        }

        internal object ExecuteInsert(string sql, string primaryKeyName)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new object[0])) {
                        cmd.CommandText = sql;
                        return _provider.ExecuteInsert(this, cmd, primaryKeyName);
                    }
                } finally {
                    CloseSharedConnection();
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                return null;
            }
        }


        /// <summary>
        /// 插入列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void Insert<T>(List<T> list)
        {
            if (list == null) throw new ArgumentNullException("poco");
            if (list.Count == 0) return;

            var pd = PocoData.ForType(typeof(T));
            var tableName = pd.TableInfo.TableName;

            var index = 0;
            while (index < list.Count) {
                var count = list.Count - index;
                int size = 1;
                if (count >= 50) {
                    size = 50;
                } else if (count >= 10) {
                    size = 10;
                }
                ExecuteInsert<T>(tableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, list, index, size);
                index += size;
            }
        }
        private void ExecuteInsert<T>(string tableName, string primaryKeyName, bool autoIncrement, List<T> list, int index2, int size)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new object[0], CommandType.Text)) {
                        var type = typeof(T);
                        var pd = PocoData.ForType(type);
                        cmd.CommandText = CrudCache.GetInsertSql(_provider, _paramPrefix, pd, size, tableName, primaryKeyName, autoIncrement);

                        for (int j = 0; j < size; j++) {
                            var poco = list[index2 + j];
                            foreach (var i in pd.Columns) {
                                if (i.Value.ResultColumn) continue;
                                if (autoIncrement && primaryKeyName != null && string.Compare(i.Value.ColumnName, primaryKeyName, true) == 0) {
                                    continue;
                                }
                                AddParam(cmd, i.Value.GetValue(poco), i.Value.PropertyInfo);
                            }
                        }

                        DoPreExecute(cmd);
                        cmd.ExecuteNonQuery();
                        OnExecutedCommand(cmd);
                    }
                } finally {
                    CloseSharedConnection();
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
            }
        }


        #endregion

        #region operation: Update

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <returns>The number of affected rows</returns>
        public int Update(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType());
            return ExecuteUpdate(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco, null);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to update</typeparam>
        /// <param name="sql">The SQL update and condition clause (ie: everything after "UPDATE tablename"</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of affected rows</returns>
        public int Update<T>(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");
            if (sql.StartsWith("UPDATE ", StringComparison.CurrentCultureIgnoreCase)) {
                return Execute(sql, args);
            }
            var pd = PocoData.ForType(typeof(T));
            return Execute(string.Format("UPDATE {0} {1}", _provider.EscapeTableName(pd.TableInfo.TableName), sql), args);
        }

        private int ExecuteUpdate(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new object[0])) {
                        var type = poco.GetType();
                        var pd = PocoData.ForObject(poco, primaryKeyName);
                        cmd.CommandText = CrudCache.GetUpdateSql(_provider, _paramPrefix, pd, tableName, primaryKeyName);

                        foreach (var i in pd.Columns) {
                            if (i.Value.ResultColumn) continue;
                            if (string.Compare(i.Value.ColumnName, primaryKeyName, true) == 0) {
                                if (primaryKeyValue == null) primaryKeyValue = i.Value.GetValue(poco);
                                continue;
                            }
                            AddParam(cmd, i.Value.GetValue(poco), i.Value.PropertyInfo);
                        }

                        // Find the property info for the primary key
                        PropertyInfo pkpi = null;
                        if (primaryKeyName != null) {
                            PocoColumn col = pd.Columns.FirstOrDefault(q => q.Key == primaryKeyName).Value;
                            if (col != null)
                                pkpi = col.PropertyInfo;
                            else
                                pkpi = new { Id = primaryKeyValue }.GetType().GetProperties()[0];
                        }
                        AddParam(cmd, primaryKeyValue, pkpi);

                        DoPreExecute(cmd);

                        // Do it
                        var retv = cmd.ExecuteNonQuery();
                        OnExecutedCommand(cmd);
                        return retv;
                    }
                } finally {
                    CloseSharedConnection();
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                return -1;
            }
        }

        #endregion

        #region operation: Delete

        /// <summary>
        ///     Performs and SQL Delete
        /// </summary>
        /// <param name="tableName">The name of the table to delete from</param>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">
        ///     The POCO object whose primary key value will be used to delete the row (or null to use the supplied
        ///     primary key value)
        /// </param>
        /// <param name="primaryKeyValue">
        ///     The value of the primary key identifing the record to be deleted (or null, or get this
        ///     value from the POCO instance)
        /// </param>
        /// <returns>The number of rows affected</returns>
        public int Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            // If primary key value not specified, pick it up from the object
            if (primaryKeyValue == null) {
                var pd = PocoData.ForObject(poco, primaryKeyName);
                if (pd.Columns.TryGetValue(primaryKeyName, out PocoColumn pc)) {
                    primaryKeyValue = pc.GetValue(poco);
                }
            }

            // Do it
            var sql = string.Format("DELETE FROM {0} WHERE {1}=@0", _provider.EscapeTableName(tableName), _provider.EscapeSqlIdentifier(primaryKeyName));
            return Execute(sql, new object[] { primaryKeyValue });
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <param name="poco">The POCO object specifying the table name and primary key value of the row to be deleted</param>
        /// <returns>The number of rows affected</returns>
        public int Delete(object poco)
        {
            var pd = PocoData.ForType(poco.GetType());
            return Delete(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco, null);
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class whose attributes identify the table and primary key to be used in the delete</typeparam>
        /// <param name="pocoOrPrimaryKey">The value of the primary key of the row to delete</param>
        /// <returns></returns>
        public int Delete<T>(object pocoOrPrimaryKey)
        {
            if (pocoOrPrimaryKey.GetType() == typeof(T))
                return Delete(pocoOrPrimaryKey);

            var pd = PocoData.ForType(typeof(T));

            if (pocoOrPrimaryKey.GetType().Name.Contains("AnonymousType")) {
                var pi = pocoOrPrimaryKey.GetType().GetProperty(pd.TableInfo.PrimaryKey);

                if (pi == null)
                    throw new InvalidOperationException(string.Format("Anonymous type does not contain an id for PK column `{0}`.", pd.TableInfo.PrimaryKey));

                pocoOrPrimaryKey = pi.GetValue(pocoOrPrimaryKey, new object[0]);
            }

            return Delete(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, null, pocoOrPrimaryKey);
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to delete from</typeparam>
        /// <param name="sql">The SQL condition clause identifying the row to delete (ie: everything after "DELETE FROM tablename"</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of affected rows</returns>
        public int Delete<T>(string sql, params object[] args)
        {
            var pd = PocoData.ForType(typeof(T));
            return Execute(string.Format("DELETE FROM {0} {1}", _provider.EscapeTableName(pd.TableInfo.TableName), sql), args);
        }

        #endregion

        #region operation: Save
        /// <summary>
        ///     Saves a POCO by either performing either an SQL Insert or SQL Update
        /// </summary>
        /// <param name="poco">The POCO object to be saved</param>
        public void Save(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType());
            var tableName = pd.TableInfo.TableName;
            var primaryKeyName = pd.TableInfo.PrimaryKey;

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentException("primaryKeyName");

            if (IsNew(primaryKeyName, pd, poco)) {
                ExecuteInsert(tableName, primaryKeyName, true, poco);
            } else {
                ExecuteUpdate(tableName, primaryKeyName, poco, null);
            }
        }

        private bool IsNew(string primaryKeyName, PocoData pd, object poco)
        {
            if (string.IsNullOrEmpty(primaryKeyName) || poco is ExpandoObject)
                throw new InvalidOperationException("IsNew() and Save() are only supported on tables with identity (inc auto-increment) primary key columns");

            object pk;
            PropertyInfo pi;
            if (pd.Columns.TryGetValue(primaryKeyName, out PocoColumn pc)) {
                pk = pc.GetValue(poco);
                pi = pc.PropertyInfo;
            } else {
                pi = poco.GetType().GetProperty(primaryKeyName);
                if (pi == null)
                    throw new ArgumentException(string.Format("The object doesn't have a property matching the primary key column name '{0}'", primaryKeyName));
                pk = pi.GetValue(poco, null);
            }

            var type = pk != null ? pk.GetType() : pi.PropertyType;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) || !type.IsValueType)
                return pk == null;

            if (type == typeof(string))
                return string.IsNullOrEmpty((string)pk);
            if (!pi.PropertyType.IsValueType)
                return pk == null;
            if (type == typeof(long))
                return (long)pk == default;
            if (type == typeof(int))
                return (int)pk == default;
            if (type == typeof(Guid))
                return (Guid)pk == default;
            if (type == typeof(ulong))
                return (ulong)pk == default;
            if (type == typeof(uint))
                return (uint)pk == default;
            if (type == typeof(short))
                return (short)pk == default(short);
            if (type == typeof(ushort))
                return (ushort)pk == default(ushort);

            // Create a default instance and compare
            return pk == Activator.CreateInstance(pk.GetType());
        }


        #endregion

        #region FormatCommand

        /// <summary>
        ///     Formats the contents of a DB command for display
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string FormatCommand(IDbCommand cmd)
        {
            return FormatCommand(cmd.CommandText, (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray());
        }

        /// <summary>
        ///     Formats an SQL query and it's arguments for display
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string FormatCommand(string sql, object[] args)
        {
            var sb = new StringBuilder();
            if (sql == null)
                return "";
            sb.Append(sql);
            if (args != null && args.Length > 0) {
                sb.Append("\n");
                for (int i = 0; i < args.Length; i++) {
                    sb.AppendFormat("\t -> {0}{1} [{2}] = \"{3}\"\n", _paramPrefix, i, args[i].GetType().Name, args[i]);
                }
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     When set to true, ToolGood.ReadyGo3.PetaPoco will automatically create the "SELECT columns" part of any query that looks like it
        ///     needs it
        /// </summary>
        public bool EnableAutoSelect;

        /// <summary>
        ///     Sets the timeout value for all SQL statements.
        /// </summary>
        public int CommandTimeout;

        /// <summary>
        ///     Sets the timeout value for the next (and only next) SQL statement
        /// </summary>
        public int OneTimeCommandTimeout;

        #endregion

        #region Member Fields

        // Member variables
        private readonly SqlHelper _sqlHelper;
        private readonly DatabaseProvider _provider;
        private IDbConnection _sharedConnection;
        private IDbTransaction _transaction;
        private int _sharedConnectionDepth;
        private int _transactionDepth;
        private bool _transactionCancelled;
        private readonly string _paramPrefix;
        private readonly DbProviderFactory _factory;
        private bool _isDisposable;

        #endregion

        #region Internal operations

        internal void ExecuteNonQueryHelper(IDbCommand cmd)
        {
            DoPreExecute(cmd);
            cmd.ExecuteNonQuery();
            OnExecutedCommand(cmd);
        }

        internal object ExecuteScalarHelper(IDbCommand cmd)
        {
            DoPreExecute(cmd);
            object r = cmd.ExecuteScalar();
            OnExecutedCommand(cmd);
            return r;
        }

        internal void DoPreExecute(IDbCommand cmd)
        {
            // Setup command timeout
            if (OneTimeCommandTimeout != 0) {
                cmd.CommandTimeout = OneTimeCommandTimeout;
                OneTimeCommandTimeout = 0;
            } else if (CommandTimeout != 0) {
                cmd.CommandTimeout = CommandTimeout;
            }

            // Call hook
            OnExecutingCommand(cmd);
        }

        #endregion
    }
}
