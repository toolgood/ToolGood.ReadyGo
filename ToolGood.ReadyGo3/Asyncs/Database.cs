using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;
using ToolGood.ReadyGo3.PetaPoco.Core;
using ToolGood.ReadyGo3.PetaPoco.Internal;

#if NETSTANDARD2_0
using SqlCommand = System.Data.Common.DbCommand;
using SqlDataReader = System.Data.Common.DbDataReader;
using SqlConnection = System.Data.Common.DbConnection;
#endif

#if !NET40

namespace ToolGood.ReadyGo3.PetaPoco
{
    partial class Database
    {
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

                if (_sharedConnection.State == ConnectionState.Closed)
                    await ((SqlConnection)_sharedConnection).OpenAsync();

                _sharedConnection = OnConnectionOpened(_sharedConnection);

                if (KeepConnectionAlive)
                    _sharedConnectionDepth++; // Make sure you call Dispose
            } else {
                if (_sharedConnection.State == ConnectionState.Broken)
                    _sharedConnection.Close();

                if (_sharedConnection.State == ConnectionState.Closed)
                    await ((SqlConnection)_sharedConnection).OpenAsync();
            }
            _sharedConnectionDepth++;
        }

        internal async Task ExecuteNonQueryHelperAsync(SqlCommand cmd)
        {
            DoPreExecute(cmd);
            await cmd.ExecuteNonQueryAsync();
            OnExecutedCommand(cmd);
        }

        internal async Task<object> ExecuteScalarHelperAsync(SqlCommand cmd)
        {
            DoPreExecute(cmd);
            object r = await cmd.ExecuteScalarAsync();
            OnExecutedCommand(cmd);
            return r;
        }


        #region ExecuteAsync ExecuteScalarAsync
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(string sql, object[] args, CommandType commandType = CommandType.Text)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args, commandType)) {
                        var retv = await ((SqlCommand)cmd).ExecuteNonQueryAsync();
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarAsync<T>(string sql, object[] args, CommandType commandType = CommandType.Text)
        {
            try {
                await OpenSharedConnectionAsync();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args, commandType)) {
                        object val = await ((SqlCommand)cmd).ExecuteScalarAsync();
                        OnExecutedCommand(cmd);

                        // Handle nullable types
                        Type u = Nullable.GetUnderlyingType(typeof(T));
                        if (u != null && val == null)
                            return default(T);

                        return (T)Convert.ChangeType(val, u == null ? typeof(T) : u);
                    }
                } finally {
                    CloseSharedConnection();
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
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(string sql, object[] args, CommandType commandType = CommandType.Text)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args, commandType)) {
                        var reader = await ((SqlCommand)cmd).ExecuteReaderAsync();
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
                return default(DataTable);
            }
        }

        #endregion

        #region QueryAsync
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> QueryAsync<T>(long skip, long take, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries<T>(0, take, sql, ref args, out sqlCount, out sqlPage);
            return QueryAsync<T>(sqlPage, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object[] args, CommandType commandType = CommandType.Text)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql, _defaultMapper);

            var resultList = new List<T>();
            await OpenSharedConnectionAsync();
            try {
                using (var icmd = CreateCommand(_sharedConnection, sql, args)) {
                    var cmd = icmd as SqlCommand;
                    SqlDataReader r = null;
                    var pd = PocoData.ForType(typeof(T));
                    try {
                        r = await cmd.ExecuteReaderAsync();
                        OnExecutedCommand(cmd);
                    } catch (Exception x) {
                        if (OnException(x))
                            throw;
                    }
                    var factory = pd.GetFactory(cmd.CommandText, _sharedConnection.ConnectionString, 0, r.FieldCount, r, _defaultMapper) as Func<IDataReader, T>;
                    using (r) {
                        while (true) {
                            try {
                                if (!r.Read())
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
        public async Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            BuildPageQueries<T>((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out string sqlCount, out string sqlPage);


            // Save the one-time command time out and use it for both queries
            var saveTimeout = OneTimeCommandTimeout;

            // Setup the paged result
            var result = new Page<T> {
                CurrentPage = page,
                PageSize = itemsPerPage,
                TotalItems = await ExecuteScalarAsync<long>(sqlCount, args)
            };
            OneTimeCommandTimeout = saveTimeout;

            // Get the records
            result.Items = (await QueryAsync<T>(sqlPage, args)).ToList();

            // Done
            return result;
        }

        #endregion

        #region InsertAsync
        /// <summary>
        ///     Performs an SQL Insert
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables</returns>
        /// <remarks>
        ///     The name of the table, it's primary key and whether it's an auto-allocated primary key are retrieved
        ///     from the POCO's attributes
        /// </remarks>
        public Task<object> InsertAsync(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return ExecuteInsertAsync(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, poco);
        }

        private async Task<object> ExecuteInsertAsync(string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new object[0])) {
                        var pd = PocoData.ForObject(poco, primaryKeyName, _defaultMapper);
                        var type = poco.GetType();
                        var sql = insert.Get(Tuple.Create(type, _sqlHelper._sqlType, 1), () => {
                            return CteateInsertSql(pd, 1, tableName, primaryKeyName, autoIncrement);
                        });

                        foreach (var i in pd.Columns) {
                            if (i.Value.ResultColumn) continue;
                            if (autoIncrement && primaryKeyName != null && string.Compare(i.Value.ColumnName, primaryKeyName, true) == 0) {
                                continue;
                            }
                            AddParam(cmd, i.Value.GetValue(poco), i.Value.PropertyInfo);
                        }
                        cmd.CommandText = sql;

                        if (!autoIncrement) {
                            DoPreExecute(cmd);
                            await ((SqlCommand)cmd).ExecuteNonQueryAsync();
                            OnExecutedCommand(cmd);

                            PocoColumn pkColumn;
                            if (primaryKeyName != null && pd.Columns.TryGetValue(primaryKeyName, out pkColumn))
                                return pkColumn.GetValue(poco);
                            else
                                return null;
                        }

                        object id = _provider.ExecuteInsertAsync(this, (SqlCommand)cmd, primaryKeyName);

                        // Assign the ID back to the primary key property
                        if (primaryKeyName != null && !poco.GetType().Name.Contains("AnonymousType")) {
                            PocoColumn pc;
                            if (pd.Columns.TryGetValue(primaryKeyName, out pc)) {
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

        internal async Task<object> ExecuteInsertAsync(string sql, string primaryKeyName)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new object[0])) {
                        cmd.CommandText = sql;
                        return await _provider.ExecuteInsertAsync(this, (SqlCommand)cmd, primaryKeyName);
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
        public async Task InsertAsync<T>(List<T> list)
        {
            if (list == null) throw new ArgumentNullException("poco");
            if (list.Count == 0) return;

            var pd = PocoData.ForType(typeof(T), null);
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
                await ExecuteInsertAsync<T>(tableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, list, index, size);
                index += size;
            }
        }
        private async Task ExecuteInsertAsync<T>(string tableName, string primaryKeyName, bool autoIncrement, List<T> list, int index2, int size)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new object[0], CommandType.Text)) {
                        var type = typeof(T);
                        var pd = PocoData.ForType(type, null);
                        var sql = insert.Get(Tuple.Create(type, _sqlHelper._sqlType, size), () => {
                            return CteateInsertSql(pd, size, tableName, primaryKeyName, autoIncrement);
                        });

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
                        cmd.CommandText = sql;

                        DoPreExecute(cmd);
                        await ((SqlCommand)cmd).ExecuteNonQueryAsync();
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

        #region UpdateAsync

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <returns>The number of affected rows</returns>
        public Task<int> UpdateAsync(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return ExecuteUpdateAsync(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco, null, null);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to update</typeparam>
        /// <param name="sql">The SQL update and condition clause (ie: everything after "UPDATE tablename"</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of affected rows</returns>
        public Task<int> UpdateAsync<T>(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return ExecuteAsync(string.Format("UPDATE {0} {1}", _provider.EscapeTableName(pd.TableInfo.TableName), sql), args);
        }

        private async Task<int> ExecuteUpdateAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new object[0])) {
                        var type = poco.GetType();
                        var pd = PocoData.ForObject(poco, primaryKeyName, _defaultMapper);
                        var sql = update.Get(Tuple.Create(type, _sqlHelper._sqlType), () => {
                            var sb = new StringBuilder();
                            var index = 0;
                            foreach (var i in pd.Columns) {
                                if (String.Compare(i.Value.ColumnName, primaryKeyName, StringComparison.OrdinalIgnoreCase) == 0) continue;
                                if (i.Value.ResultColumn) continue;

                                // Build the sql
                                if (index > 0) sb.Append(", ");
                                sb.AppendFormat("{0} = {1}{2}", _provider.EscapeSqlIdentifier(i.Value.ColumnName), _paramPrefix, index++);
                            }

                            return string.Format("UPDATE {0} SET {1} WHERE {2} = {3}{4}",
                                _provider.EscapeTableName(tableName), sb.ToString(), _provider.EscapeSqlIdentifier(primaryKeyName), _paramPrefix, index++);
                        });

                        cmd.CommandText = sql;

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

                        // Do it
                        var retv = await ((SqlCommand)cmd).ExecuteNonQueryAsync();
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
                var pd = PocoData.ForObject(poco, primaryKeyName, _defaultMapper);
                PocoColumn pc;
                if (pd.Columns.TryGetValue(primaryKeyName, out pc)) {
                    primaryKeyValue = pc.GetValue(poco);
                }
            }

            // Do it
            var sql = string.Format("DELETE FROM {0} WHERE {1}=@0", _provider.EscapeTableName(tableName), _provider.EscapeSqlIdentifier(primaryKeyName));
            return ExecuteAsync(sql, new object[] { primaryKeyValue });
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <param name="poco">The POCO object specifying the table name and primary key value of the row to be deleted</param>
        /// <returns>The number of rows affected</returns>
        public Task<int> DeleteAsync(object poco)
        {
            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return DeleteAsync(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco, null);
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class whose attributes identify the table and primary key to be used in the delete</typeparam>
        /// <param name="pocoOrPrimaryKey">The value of the primary key of the row to delete</param>
        /// <returns></returns>
        public Task<int> DeleteAsync<T>(object pocoOrPrimaryKey)
        {
            if (pocoOrPrimaryKey.GetType() == typeof(T))
                return DeleteAsync(pocoOrPrimaryKey);

            var pd = PocoData.ForType(typeof(T), _defaultMapper);

            if (pocoOrPrimaryKey.GetType().Name.Contains("AnonymousType")) {
                var pi = pocoOrPrimaryKey.GetType().GetProperty(pd.TableInfo.PrimaryKey);

                if (pi == null)
                    throw new InvalidOperationException(string.Format("Anonymous type does not contain an id for PK column `{0}`.", pd.TableInfo.PrimaryKey));

                pocoOrPrimaryKey = pi.GetValue(pocoOrPrimaryKey, new object[0]);
            }

            return DeleteAsync(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, null, pocoOrPrimaryKey);
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to delete from</typeparam>
        /// <param name="sql">The SQL condition clause identifying the row to delete (ie: everything after "DELETE FROM tablename"</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of affected rows</returns>
        public Task<int> DeleteAsync<T>(string sql, params object[] args)
        {

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return ExecuteAsync(string.Format("DELETE FROM {0} {1}", _provider.EscapeTableName(pd.TableInfo.TableName), sql), args);
        }

        #endregion

        #region SaveAsync
        /// <summary>
        /// 
        /// </summary>
        /// <param name="poco"></param>
        /// <returns></returns>
        public Task SaveAsync(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            var tableName = pd.TableInfo.TableName;
            var primaryKeyName = pd.TableInfo.PrimaryKey;

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentException("primaryKeyName");

            if (IsNew(primaryKeyName, pd, poco)) {
                return ExecuteInsertAsync(tableName, primaryKeyName, true, poco);
            } else {
                return ExecuteUpdateAsync(tableName, primaryKeyName, poco, null, null);
            }
        }

        #endregion

    }
}
#endif