﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.Exceptions;
using ToolGood.ReadyGo3.PetaPoco.Core;
using ToolGood.ReadyGo3.PetaPoco.Internal;
using System.Data.Common;
using ToolGood.ReadyGo3.Internals;
using System.Threading;

#if !NET40
using SqlCommand = System.Data.Common.DbCommand;
using SqlDataReader = System.Data.Common.DbDataReader;
#endif

#if !NET40

namespace ToolGood.ReadyGo3.PetaPoco
{
    partial class Database
    {
        /// <summary>
        /// 
        /// </summary>
        public CancellationToken _Token = CancellationToken.None;


        /// <summary>
        /// Open a connection that will be used for all subsequent queries.
        /// </summary>
        /// <remarks>
        /// Calls to Open/CloseSharedConnection are reference counted and should be balanced
        /// </remarks>
        public async Task OpenSharedConnectionAsync()
        {
            if (_sharedConnectionDepth == 0) {
                _sharedConnection = _factory.CreateConnection();
                _sharedConnection.ConnectionString = _sqlHelper._connectionString;

                if (_sharedConnection.State == ConnectionState.Broken)
                    _sharedConnection.Close();

                if (_sharedConnection.State == ConnectionState.Closed) {
                    var con = _sharedConnection as DbConnection;
                    if (con != null)
                        await con.OpenAsync(_Token).ConfigureAwait(false);
                    else
                        _sharedConnection.Open();
                }

                if (KeepConnectionAlive)
                    _sharedConnectionDepth++; // Make sure you call Dispose
            } else {
                if (_sharedConnection.State == ConnectionState.Broken)
                    _sharedConnection.Close();

                if (_sharedConnection.State == ConnectionState.Closed) {
                    var con = _sharedConnection as DbConnection;
                    if (con != null)
                        await con.OpenAsync(_Token).ConfigureAwait(false);
                    else
                        _sharedConnection.Open();
                }
            }
            _sharedConnectionDepth++;
        }

        internal async Task ExecuteNonQueryHelperAsync(SqlCommand cmd)
        {
            DoPreExecute(cmd);
            await cmd.ExecuteNonQueryAsync().ConfigureAwait(false); ;
            OnExecutedCommand(cmd);
        }


        internal async Task<object> ExecuteScalarHelperAsync(SqlCommand cmd)
        {
            DoPreExecute(cmd);
            object r = await cmd.ExecuteScalarAsync(_Token).ConfigureAwait(false);
            OnExecutedCommand(cmd);
            return r;
        }


        #region ExecuteAsync ExecuteScalarAsync
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(string sql, object[] args)
        {
            try {
                await OpenSharedConnectionAsync().ConfigureAwait(false);
                try {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args)) {
                        DoPreExecute(cmd);
                        var retv = await ((SqlCommand)cmd).ExecuteNonQueryAsync(_Token).ConfigureAwait(false);
                        OnExecutedCommand(cmd);
                        return retv;
                    }
                } finally {
                    CloseSharedConnection();
                    _Token = CancellationToken.None;
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                return -1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarAsync<T>(string sql, object[] args)
        {
            try {
                await OpenSharedConnectionAsync().ConfigureAwait(false);
                try {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args)) {
                        DoPreExecute(cmd);
                        object val = await ((SqlCommand)cmd).ExecuteScalarAsync(_Token).ConfigureAwait(false);
                        OnExecutedCommand(cmd);

                        // Handle nullable types
                        Type u = Nullable.GetUnderlyingType(typeof(T));
                        if (u != null && val == null)
                            return default(T);

                        return (T)Convert.ChangeType(val, u ?? typeof(T));
                    }
                } finally {
                    CloseSharedConnection();
                    _Token = CancellationToken.None;
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw;
                return default(T);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(string sql, object[] args)
        {
            try {
                await OpenSharedConnectionAsync().ConfigureAwait(false);
                try {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args)) {
                        DoPreExecute(cmd);
                        var reader = await ((SqlCommand)cmd).ExecuteReaderAsync(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult, _Token).ConfigureAwait(false);
                        OnExecutedCommand(cmd);
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
                        return dt;
                    }
                } finally {
                    CloseSharedConnection();
                    _Token = CancellationToken.None;
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                return default(DataTable);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteDataSetAsync(string sql, object[] args)
        {
            try {
                await OpenSharedConnectionAsync().ConfigureAwait(false);
                try {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args)) {
                        using (var adapter = _factory.CreateDataAdapter()) {
                            DoPreExecute(cmd);
                            adapter.SelectCommand = (DbCommand)cmd;
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            OnExecutedCommand(cmd);
                            return ds;
                        }
                    }
                } finally {
                    CloseSharedConnection();
                    _Token = CancellationToken.None;
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                return default;
            }
        }
        #endregion

        #region QueryAsync
        public Task<IEnumerable<T>> QueryAsync<T>(int skip, int take, string sql, object[] args)
        {
            return QueryAsync<T>(null, skip, take, sql, args);
        }


        public async Task<IEnumerable<T>> QueryAsync<T>(string table, int skip, int take, string sql, object[] args)
        {
            //string sqlCount, sqlPage;

            BuildPageQueries_Table<T>(table, skip, take, sql, ref args, out _, out string sqlPage);

            List<T> list = new List<T>(take);
            await QueryAsync<T>(sqlPage, args, list);
            return list;
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object[] args)
        {
            return QueryAsync<T>(null, sql, args);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string table, string sql, object[] args)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, table, sql);

            var resultList = new List<T>();
            await OpenSharedConnectionAsync().ConfigureAwait(false);
            try {
                using (var cmd = CreateCommand(_sharedConnection, sql, args)) {
                    SqlDataReader r = null;
                    var pd = PocoData.ForType(typeof(T));
                    try {
                        DoPreExecute(cmd);
                        r = await ((SqlCommand)cmd).ExecuteReaderAsync(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult, _Token).ConfigureAwait(false);
                        OnExecutedCommand(cmd);
                    } catch (Exception x) {
                        if (OnException(x))
                            throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                    }
                    var factory = pd.GetFactory(0, r.FieldCount, r/*, _sqlHelper._use_proxyType*/) as Func<IDataReader, T>;
                    using (r) {
                        while (true) {
                            try {
                                if (!await r.ReadAsync(_Token).ConfigureAwait(false))
                                    break;
                                T poco = factory(r);
                                resultList.Add(poco);
                            } catch (Exception x) {
                                if (OnException(x))
                                    throw;
                            }
                        }
                    }
                }
                return resultList;
            } finally {
                CloseSharedConnection();
                _Token = CancellationToken.None;
            }
        }

        public async Task QueryAsync<T>(string sql, object[] args, IList<T> resultList)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, null, sql);

            await OpenSharedConnectionAsync().ConfigureAwait(false);
            try {
                using (var cmd = CreateCommand(_sharedConnection, sql, args)) {
                    SqlDataReader r = null;
                    var pd = PocoData.ForType(typeof(T));
                    try {
                        DoPreExecute(cmd);
                        r = await ((SqlCommand)cmd).ExecuteReaderAsync(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult, _Token).ConfigureAwait(false);
                        OnExecutedCommand(cmd);
                    } catch (Exception x) {
                        if (OnException(x))
                            throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                    }
                    var factory = pd.GetFactory(0, r.FieldCount, r/*, _sqlHelper._use_proxyType*/) as Func<IDataReader, T>;
                    using (r) {
                        while (true) {
                            try {
                                if (!await r.ReadAsync(_Token).ConfigureAwait(false))
                                    break;
                                T poco = factory(r);
                                resultList.Add(poco);
                            } catch (Exception x) {
                                if (OnException(x))
                                    throw;
                            }
                        }
                    }
                }
            } finally {
                CloseSharedConnection();
                _Token = CancellationToken.None;
            }
        }

        #endregion

        #region PageAsync

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<Page<T>> PageAsync<T>(int page, int itemsPerPage, string sql, object[] args)
        {
            BuildPageQueries<T>((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out string sqlCount, out string sqlPage);

            return PageSqlAsync<T>(page, itemsPerPage, sqlPage, sqlCount, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<Page<T>> PageAsync_Table<T>(string table, int page, int itemsPerPage, string sql, object[] args)
        {
            BuildPageQueries_Table<T>(table, (page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out string sqlCount, out string sqlPage);
            return PageSqlAsync<T>(page, itemsPerPage, sqlPage, sqlCount, args);
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
        public async Task<Page<T>> PageSqlAsync<T>(int page, int itemsPerPage, string selectSql, string countSql, object[] args)
        {
            // Save the one-time command time out and use it for both queries
            var saveTimeout = OneTimeCommandTimeout;
            var tokenTemp = _Token;

            // Setup the paged result
            var result = new Page<T> {
                CurrentPage = page,
                PageSize = itemsPerPage,
                TotalItems = await ExecuteScalarAsync<int>(countSql, args).ConfigureAwait(false)
            };
            OneTimeCommandTimeout = saveTimeout;

            _Token = tokenTemp;

            List<T> list = new List<T>();
            await QueryAsync<T>(selectSql, args, list);
            // Get the records
            result.Items = list;
            // Done
            return result;
        }

        #endregion

        #region InsertAsync
        public Task<object> InsertAsync(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType());
            return ExecuteInsertAsync(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, poco);
        }
        public Task<object> InsertAsync_Table(string table, object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType());
            return ExecuteInsertAsync(table, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, poco);
        }

        public Task<object> InsertAsync(string table, object poco, bool autoIncrement, IEnumerable<string> ignoreFields)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");
            return ExecuteInsertAsync(table, null, autoIncrement, poco, ignoreFields);
        }


        private async Task<object> ExecuteInsertAsync(string tableName, string primaryKeyName, bool autoIncrement, object poco, IEnumerable<string> ignoreFields = null)
        {
            try {
                await OpenSharedConnectionAsync().ConfigureAwait(false);
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new object[0])) {
                        var pd = PocoData.ForObject(poco, primaryKeyName);
                        var type = poco.GetType();
                        cmd.CommandText = CrudCache.GetInsertSql(_provider, _paramPrefix, pd, 1, tableName, primaryKeyName, autoIncrement, ignoreFields);

                        foreach (var i in pd.Columns) {
                            if (i.Value.ResultColumn) continue;
                            if (autoIncrement && primaryKeyName != null && string.Compare(i.Value.ColumnName, primaryKeyName, true) == 0) { continue; }
                            if (ignoreFields != null && ignoreFields.Contains(i.Key, StringComparer.OrdinalIgnoreCase)) continue;
                            AddParam(cmd, i.Value.GetValue(poco), i.Value.PropertyInfo);
                        }

                        if (!autoIncrement) {
                            DoPreExecute(cmd);
                            await ((SqlCommand)cmd).ExecuteNonQueryAsync(_Token).ConfigureAwait(false);
                            OnExecutedCommand(cmd);

                            if (primaryKeyName != null && pd.Columns.TryGetValue(primaryKeyName, out PocoColumn pkColumn))
                                return pkColumn.GetValue(poco);
                            else
                                return null;
                        }

                        object id = await _provider.ExecuteInsertAsync(this, (SqlCommand)cmd, primaryKeyName);

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
                    _Token = CancellationToken.None;
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                return null;
            }
        }

        public async Task InsertAsync_Table<T>(string table, List<T> list)
        {
            if (string.IsNullOrEmpty(table)) throw new ArgumentNullException("table");
            if (list == null) throw new ArgumentNullException("poco");
            if (list.Count == 0) return;

            var pd = PocoData.ForType(typeof(T));

            var index = 0;
            while (index < list.Count) {
                var count = list.Count - index;
                int size;
                if (count >= 50) {
                    size = 50;
                } else if (count >= 10) {
                    size = 10;
                } else {
                    size = count;
                }
                await ExecuteInsertAsync<T>(table, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, list, index, size);
                index += size;
            }
        }

        public async Task InsertAsync<T>(List<T> list)
        {
            if (list == null) throw new ArgumentNullException("poco");
            if (list.Count == 0) return;

            var pd = PocoData.ForType(typeof(T));
            var tableName = pd.TableInfo.TableName;

            var index = 0;
            while (index < list.Count) {
                var count = list.Count - index;
                int size;
                if (count >= 50) {
                    size = 50;
                } else if (count >= 10) {
                    size = 10;
                } else {
                    size = count;
                }
                await ExecuteInsertAsync<T>(tableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, list, index, size);
                index += size;
            }
        }

        private async Task ExecuteInsertAsync<T>(string tableName, string primaryKeyName, bool autoIncrement, List<T> list, int index2, int size)
        {
            try {
                await OpenSharedConnectionAsync().ConfigureAwait(false);
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new object[0])) {
                        var type = typeof(T);
                        var pd = PocoData.ForType(type);
                        cmd.CommandText = CrudCache.GetInsertSql(_provider, _paramPrefix, pd, size, tableName, primaryKeyName, autoIncrement);

                        var cols = pd.Columns.Where(q => q.Value.ResultColumn == false).Select(q => q.Value).ToList();
                        if (autoIncrement && primaryKeyName != null) {
                            cols.RemoveAll(q => string.Compare(q.ColumnName, primaryKeyName, true) == 0);
                        }

                        for (int j = 0; j < size; j++) {
                            var poco = list[index2 + j];
                            foreach (var c in cols) {
                                AddParam(cmd, c.GetValue(poco), c.PropertyInfo);
                            }
                        }

                        DoPreExecute(cmd);
                        await ((SqlCommand)cmd).ExecuteNonQueryAsync(_Token).ConfigureAwait(false);
                        OnExecutedCommand(cmd);
                    }
                } finally {
                    CloseSharedConnection();
                    _Token = CancellationToken.None;
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
            }
        }


        #endregion

        #region UpdateAsync
        public Task<int> UpdateAsync(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType());
            return ExecuteUpdateAsync(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco, null);
        }
        public Task<int> UpdateAsync_Table(string table, object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType());
            return ExecuteUpdateAsync(table, pd.TableInfo.PrimaryKey, poco, null);
        }

        public Task<int> UpdateAsync<T>(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            var pd = PocoData.ForType(typeof(T));
            return ExecuteAsync(string.Format("UPDATE {0} {1}", _provider.GetTableName(pd.TableInfo.TableName), sql), args);
        }
        public Task<int> UpdateAsync_Table<T>(string table, string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            return ExecuteAsync(string.Format("UPDATE {0} {1}", _provider.GetTableName(table), sql), args);
        }


        private async Task<int> ExecuteUpdateAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            try {
                await OpenSharedConnectionAsync().ConfigureAwait(false);
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
                            PocoColumn col = pd.Columns.FirstOrDefault(q => q.Value.ColumnName == primaryKeyName).Value;
                            if (col != null)
                                pkpi = col.PropertyInfo;
                            else
                                pkpi = new { Id = primaryKeyValue }.GetType().GetProperties()[0];
                        }
                        AddParam(cmd, primaryKeyValue, pkpi);

                        DoPreExecute(cmd);
                        var retv = await ((SqlCommand)cmd).ExecuteNonQueryAsync(_Token).ConfigureAwait(false);
                        OnExecutedCommand(cmd);
                        return retv;
                    }
                } finally {
                    CloseSharedConnection();
                    _Token = CancellationToken.None;
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw new SqlExecuteException(x, _sqlHelper._sql.LastCommand);
                return -1;
            }
        }



        #endregion

        #region DeleteAsync
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
        public Task<int> DeleteAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            // If primary key value not specified, pick it up from the object
            if (primaryKeyValue == null) {
                var pd = PocoData.ForObject(poco, primaryKeyName);
                if (pd.Columns.TryGetValue(primaryKeyName, out PocoColumn pc)) {
                    primaryKeyValue = pc.GetValue(poco);
                }
            }

            // Do it
            var sql = string.Format("DELETE FROM {0} WHERE {1}=@0", _provider.GetTableName(tableName), _provider.EscapeSqlIdentifier(primaryKeyName));
            return ExecuteAsync(sql, new object[] { primaryKeyValue });
        }

        public Task<int> DeleteAsync(object poco)
        {
            var pd = PocoData.ForType(poco.GetType());
            return DeleteAsync(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco, null);
        }

        public Task<int> DeleteAsync_Table(string table, object poco)
        {
            var pd = PocoData.ForType(poco.GetType());
            return DeleteAsync(table, pd.TableInfo.PrimaryKey, poco, null);
        }


        public Task<int> DeleteAsync_Table<T>(string table, object pocoOrPrimaryKey)
        {
            if (pocoOrPrimaryKey.GetType() == typeof(T))
                return DeleteAsync(pocoOrPrimaryKey);

            var pd = PocoData.ForType(typeof(T));

            if (pocoOrPrimaryKey.GetType().Name.Contains("AnonymousType")) {
                var pi = pocoOrPrimaryKey.GetType().GetProperty(pd.TableInfo.PrimaryKey);

                if (pi == null)
                    throw new InvalidOperationException(string.Format("Anonymous type does not contain an id for PK column `{0}`.", pd.TableInfo.PrimaryKey));

                pocoOrPrimaryKey = pi.GetValue(pocoOrPrimaryKey, new object[0]);
            }

            return DeleteAsync(table, pd.TableInfo.PrimaryKey, null, pocoOrPrimaryKey);
        }
        public Task<int> DeleteAsync<T>(object pocoOrPrimaryKey)
        {
            if (pocoOrPrimaryKey.GetType() == typeof(T))
                return DeleteAsync(pocoOrPrimaryKey);

            var pd = PocoData.ForType(typeof(T));

            if (pocoOrPrimaryKey.GetType().Name.Contains("AnonymousType")) {
                var pi = pocoOrPrimaryKey.GetType().GetProperty(pd.TableInfo.PrimaryKey);

                if (pi == null)
                    throw new InvalidOperationException(string.Format("Anonymous type does not contain an id for PK column `{0}`.", pd.TableInfo.PrimaryKey));

                pocoOrPrimaryKey = pi.GetValue(pocoOrPrimaryKey, new object[0]);
            }

            return DeleteAsync(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, null, pocoOrPrimaryKey);
        }

        public Task<int> DeleteAsync<T>(string sql, params object[] args)
        {

            var pd = PocoData.ForType(typeof(T));
            return ExecuteAsync(string.Format("DELETE FROM {0} {1}", _provider.GetTableName(pd.TableInfo.TableName), sql), args);
        }
        public Task<int> DeleteAsync_Table<T>(string table, string sql, params object[] args)
        {
            return ExecuteAsync(string.Format("DELETE FROM {0} {1}", _provider.GetTableName(table), sql), args);
        }
        public Task<int> DeleteAsync_Table(string table, string sql, params object[] args)
        {
            return ExecuteAsync(string.Format("DELETE FROM {0} {1}", _provider.GetTableName(table), sql), args);
        }
        #endregion

        #region SaveAsync
        public Task SaveAsync(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType());
            var tableName = pd.TableInfo.TableName;
            var primaryKeyName = pd.TableInfo.PrimaryKey;

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentException("primaryKeyName");

            if (IsNew(primaryKeyName, pd, poco)) {
                return ExecuteInsertAsync(tableName, primaryKeyName, true, poco);
            } else {
                return ExecuteUpdateAsync(tableName, primaryKeyName, poco, null);
            }
        }
        public Task SaveAsync_Table(string table, object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType());
            var primaryKeyName = pd.TableInfo.PrimaryKey;

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentException("primaryKeyName");

            if (IsNew(primaryKeyName, pd, poco)) {
                return ExecuteInsertAsync(table, primaryKeyName, true, poco);
            } else {
                return ExecuteUpdateAsync(table, primaryKeyName, poco, null);
            }
        }


        #endregion

    }
}
#endif