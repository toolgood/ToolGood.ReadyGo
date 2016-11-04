using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using ToolGood.ReadyGo.Internals;
using ToolGood.ReadyGo.Providers;
using ToolGood.ReadyGo.SqlBuilding;

namespace ToolGood.ReadyGo.Poco
{
    public class Database : IDisposable
    {
        #region IDisposable

        /// <summary>
        ///     Automatically close one open shared connection
        ///
        /// </summary>
        public void Dispose()
        {
            while (_transactionDepth > 0) {
                AbortTransaction();
            }
            while (_sharedConnectionDepth > 0) {
                CloseSharedConnection();
            }
        }
        ~Database()
        {
            Dispose();
        }

        #endregion IDisposable

        #region 事件 Transactioning Transactioned ConnectionOpened ConnectionClosing ExecutingCommand ExecutedCommand Exceptioned

        internal event Action Transactioning;

        internal event Action Transactioned;

        internal event Action<IDbConnection> ConnectionOpened;

        internal event Action<IDbConnection> ConnectionClosing;

        internal event Action<IDbCommand> ExecutingCommand;

        internal event Action<IDbCommand> ExecutedCommand;

        internal event Action<Exception> Exceptioned;

        bool OnException(Exception x)
        {
            if (Exceptioned != null) {
                Exceptioned(x);
            }
            return true;
        }

        IDbConnection OnConnectionOpened(IDbConnection conn)
        {
            if (ConnectionOpened != null) {
                ConnectionOpened(conn);
            }
            return conn;
        }

        void OnConnectionClosing(IDbConnection conn)
        {
            if (ConnectionClosing != null) {
                ConnectionClosing(conn);
            }
        }

        void OnExecutingCommand(IDbCommand cmd)
        {
            if (ExecutingCommand != null) {
                ExecutingCommand(cmd);
            }
        }

        void OnExecutedCommand(IDbCommand cmd)
        {
            if (ExecutedCommand != null) {
                ExecutedCommand(cmd);
            }
        }

        void OnBeginTransaction()
        {
            if (Transactioning != null) {
                Transactioning();
            }
        }

        void OnEndTransaction()
        {
            if (Transactioned != null) {
                Transactioned();
            }
        }

        #endregion 事件 Transactioning Transactioned ConnectionOpened ConnectionClosing ExecutingCommand ExecutedCommand Exceptioned

        #region 私有变量

        private SqlType _sqlType;
        private string _connectionString;
        private IDbConnection _sharedConnection;
        private IDbTransaction _transaction;
        private int _sharedConnectionDepth;
        private int _transactionDepth;
        private bool _transactionCancelled;
        private DbProviderFactory _factory;
        private DatabaseProvider _provider;
        internal CommandType commandType = CommandType.Text;
        private string _paramPrefix;

        #endregion 私有变量

        #region 公共属性

        public int CommandTimeout { get; set; }
        public int OneTimeCommandTimeout { get; set; }

        #endregion 公共属性

        #region 构造函数

        public Database(string connectionString, DbProviderFactory factory, SqlType type)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string must not be null or empty", "connectionString");

            if (factory == null)
                throw new ArgumentNullException("factory");
            _connectionString = connectionString;
            _sqlType = type;
            _provider = DatabaseProvider.Resolve(_sqlType);
            _paramPrefix = _provider.GetParameterPrefix(_connectionString);

            _transactionDepth = 0;
            _factory = factory;
        }

        #endregion 构造函数

        #region CreateParams AddParam

        List<IDbDataParameter> CreateParams(object[] args)
        {
            //var _provider = DatabaseProvider.Resolve(_sqlType);
            //var _paramPrefix = _provider.GetParameterPrefix(_connectionString);

            List<IDbDataParameter> list = new List<IDbDataParameter>();
            for (int i = 0; i < args.Length; i++) {
                var value = args[i];
                var idbParam = value as IDbDataParameter;
                if (idbParam != null) {
                    idbParam.ParameterName = string.Format("{0}{1}", _paramPrefix, list.Count);
                    list.Add(idbParam);
                    continue;
                }
                var p = _factory.CreateParameter();
                p.ParameterName = string.Format("{0}{1}", _paramPrefix, list.Count);

                // Assign the parmeter value
                if (value == null) {
                    p.Value = DBNull.Value;
                } else {
                    convertToDBtype(value, p, _provider);
                }
                list.Add(p);
            }
            return list;
        }

        internal static void convertToDBtype(object value, IDbDataParameter p, DatabaseProvider _provider)
        {
            //var _provider = DatabaseProvider.ResolveDatabaseProvider(_sqlType);
            // Give the database type first crack at converting to DB required type
            value = _provider.MapParameterValue(value);

            var t = value.GetType();
            if (t.IsEnum) // PostgreSQL .NET driver wont cast enum to int
            {
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
            } else if (value.GetType().Name == "SqlGeography") //SqlGeography is a CLR Type
            {
                p.GetType().GetProperty("UdtTypeName").SetValue(p, "geography", null); //geography is the equivalent SQL Server Type
                p.Value = value;
            } else if (value.GetType().Name == "SqlGeometry") //SqlGeometry is a CLR Type
            {
                p.GetType().GetProperty("UdtTypeName").SetValue(p, "geometry", null); //geography is the equivalent SQL Server Type
                p.Value = value;
            } else {
                p.Value = value;
            }
        }

        private void AddParam(IDbCommand cmd, object value, PropertyInfo pi)
        {
            // Support passed in parameters
            var idbParam = value as IDbDataParameter;
            if (idbParam != null) {
                idbParam.ParameterName = _paramPrefix + cmd.Parameters.Count.ToString();
                //string.Format("{0}{1}", _paramPrefix, cmd.Parameters.Count);
                cmd.Parameters.Add(idbParam);
                return;
            }

            // Create the parameter
            var p = cmd.CreateParameter();
            p.ParameterName = _paramPrefix + cmd.Parameters.Count.ToString();
            //string.Format("{0}{1}", _paramPrefix, cmd.Parameters.Count);

            // Assign the parmeter value
            if (value == null) {
                p.Value = DBNull.Value;
                if (pi != null && pi.PropertyType.Name == "Byte[]") {
                    p.DbType = DbType.Binary;
                }
            } else {
                convertToDBtype(value, p, _provider);
            }

            // Add to the collection
            cmd.Parameters.Add(p);
        }

        #endregion CreateParams AddParam

        #region CreateCommand

        private IDbCommand CreateCommand(IDbConnection connection, string sql, List<IDbDataParameter> args, CommandType commandType)
        {
            // Create the command and add parameters
            IDbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = sql;
            cmd.Transaction = _transaction;
            cmd.CommandType = commandType;
            foreach (var item in args) {
                cmd.Parameters.Add(item);
            }

            // Call logging
            if (!String.IsNullOrEmpty(sql))
                DoPreExecute(cmd);

            return cmd;
        }

        #endregion CreateCommand

        #region Connection Management

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
                _sharedConnection.ConnectionString = _connectionString;

                if (_sharedConnection.State == ConnectionState.Broken)
                    _sharedConnection.Close();

                if (_sharedConnection.State == ConnectionState.Closed)
                    _sharedConnection.Open();

                _sharedConnection = OnConnectionOpened(_sharedConnection);
                _sharedConnectionDepth++;
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
                    OnConnectionClosing(_sharedConnection);
                    _sharedConnection.Close();
                    _sharedConnection.Dispose();
                    _sharedConnection = null;
                }
            }
        }

        #endregion Connection Management

        #region Transaction Management

        //public TableTransaction GetTransaction()
        //{
        //    return new TableTransaction(this);
        //}

        /// <summary>
        ///     Starts a transaction scope, see GetTransaction() for recommended usage
        /// </summary>
        public void BeginTransaction(IsolationLevel? _isolationLevel)
        {
            _transactionDepth++;

            if (_transactionDepth == 1) {
                OpenSharedConnection();
                //_transaction = _sharedConnection.BeginTransaction();
                _transaction = !_isolationLevel.HasValue ?
                    _sharedConnection.BeginTransaction() :
                    _sharedConnection.BeginTransaction(_isolationLevel.Value);
                _transactionCancelled = false;
                OnBeginTransaction();
            }
        }

        /// <summary>
        ///     Internal helper to cleanup transaction
        /// </summary>
        public void CleanupTransaction()
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
            _transactionCancelled = true;
            if ((--_transactionDepth) == 0)
                CleanupTransaction();
        }

        /// <summary>
        ///     Marks the current transaction scope as complete.
        /// </summary>
        public void CompleteTransaction()
        {
            if ((--_transactionDepth) == 0)
                CleanupTransaction();
        }

        #endregion Transaction Management

        #region Execute ExecuteScalar Exists

        public int Execute(string sql, object[] args)
        {
            var list = CreateParams(args);
            return Execute(sql, list);
        }

        public int Execute(string sql, List<IDbDataParameter> args)
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
                    throw;
                return -1;
            }
        }

        public T ExecuteScalar<T>(string sql, object[] args)
        {
            var list = CreateParams(args);
            return ExecuteScalar<T>(sql, list);
        }

        public T ExecuteScalar<T>(string sql, List<IDbDataParameter> args)
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

        public bool Exists<T>(string sql, object[] args)
        {
            var list = CreateParams(args);
            return ExecuteScalar<int>(sql, list) != 0;
        }

        public bool Exists<T>(string sql, List<IDbDataParameter> args)
        {
            return ExecuteScalar<int>(sql, args) != 0;
        }

        public DataTable ExecuteDataTable(string sql, object[] args)
        {
            var list = CreateParams(args);
            return GetDataTable(sql, list);
        }

        public DataTable GetDataTable(string sql, List<IDbDataParameter> args)
        {
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
                    return dt;
                }
            } finally {
                CloseSharedConnection();
            }
        }

        public DataSet ExecuteDataSet(string sql, object[] args)
        {
            var list = CreateParams(args);
            return ExecuteDataSet(sql, list);
        }

        public DataSet ExecuteDataSet(string sql, List<IDbDataParameter> args)
        {
            OpenSharedConnection();
            try {
                using (var cmd = CreateCommand(_sharedConnection, sql, args, commandType)) {
                    using (var adapter = _factory.CreateDataAdapter()) {
                        //cmd.ExecuteNonQuery();
                        adapter.SelectCommand = (DbCommand)cmd;
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        return ds;
                    }
                }
            } finally {
                CloseSharedConnection();
            }
        }

        #endregion Execute ExecuteScalar Exists

        #region Query

        public IEnumerable<T> Query<T>(string sql, object[] args)
        {
            var list = CreateParams(args);
            return Query<T>(sql, list);
        }

        public IEnumerable<T> Query<T>(string sql, List<IDbDataParameter> args)
        {
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
                            throw;
                        yield break;
                    }
                    var factory = pd.GetFactory(_sqlType, "t1", r) as Func<IDataReader, string, T>;

                    using (r) {
                        while (true) {
                            T poco;
                            try {
                                if (!r.Read())
                                    yield break;
                                poco = factory(r, "t1");
                            } catch (Exception x) {
                                if (OnException(x))
                                    throw;
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

        public IEnumerable<T> Query<T>(T row, string sql, object[] args) where T : ITableRow
        {
            var list = CreateParams(args);
            return Query(row, sql, list);
        }

        public IEnumerable<T> Query<T>(T row, string sql, List<IDbDataParameter> args) where T : ITableRow
        {
            List<PocoData> pds = new List<PocoData>();
            foreach (var item in row.GetTypes()) {
                pds.Add(PocoData.ForType(item));
            }
            List<Func<IDataReader, string, dynamic>> factorys = new List<Func<IDataReader, string, dynamic>>();

            OpenSharedConnection();
            try {
                using (var cmd = CreateCommand(_sharedConnection, sql, args, commandType)) {
                    IDataReader r;
                    try {
                        r = cmd.ExecuteReader();
                        OnExecutedCommand(cmd);
                    } catch (Exception x) {
                        if (OnException(x))
                            throw;
                        yield break;
                    }
                    for (int i = 0; i < row.GetTableCount; i++) {
                        factorys.Add(pds[i].GetFactory(_sqlType, "t" + (i + 1).ToString(), r) as Func<IDataReader, string, dynamic>);
                    }
                    using (r) {
                        while (true) {
                            T poco = (T)row.Clone();
                            try {
                                if (!r.Read())
                                    yield break;
                                for (int i = 0; i < row.GetTableCount; i++) {
                                    var obj = factorys[i](r, "t" + (i + 1).ToString());
                                    ((ITableRow)poco).SetTable(i, obj);
                                }
                            } catch (Exception x) {
                                if (OnException(x))
                                    throw;
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

        #endregion Query

        #region Page SkipTake

        public Page<T> Page<T>(long page, long itemsPerPage, string sql, object[] args)
        {
            string sqlPage;
            var result = GetPageData<T>(page, itemsPerPage, sql, ref args, out sqlPage);
            result.Items = Query<T>(sqlPage, args).ToList();

            return result;
        }

        public Page<TableRow<T1, T2>> Page<T1, T2>(long page, long itemsPerPage, string sql, object[] args)
        {
            string sqlPage;
            var result = GetPageData<TableRow<T1, T2>>(page, itemsPerPage, sql, ref args, out sqlPage);
            TableRow<T1, T2> row = new TableRow<T1, T2>();

            result.Items = ((IEnumerable<TableRow<T1, T2>>)Query(row, sqlPage, args)).ToList();
            return result;
        }

        public Page<TableRow<T1, T2, T3>> Page<T1, T2, T3>(long page, long itemsPerPage, string sql, object[] args)
        {
            string sqlPage;
            var result = GetPageData<TableRow<T1, T2, T3>>(page, itemsPerPage, sql, ref args, out sqlPage);
            TableRow<T1, T2, T3> row = new TableRow<T1, T2, T3>();

            result.Items = ((IEnumerable<TableRow<T1, T2, T3>>)Query(row, sqlPage, args)).ToList();
            return result;
        }

        public Page<TableRow<T1, T2, T3, T4>> Page<T1, T2, T3, T4>(long page, long itemsPerPage, string sql, object[] args)
        {
            string sqlPage;
            var result = GetPageData<TableRow<T1, T2, T3, T4>>(page, itemsPerPage, sql, ref args, out sqlPage);
            TableRow<T1, T2, T3, T4> row = new TableRow<T1, T2, T3, T4>();

            result.Items = ((IEnumerable<TableRow<T1, T2, T3, T4>>)Query(row, sqlPage, args)).ToList();
            return result;
        }

        public Page<TableRow<T1, T2, T3, T4, T5>> Page<T1, T2, T3, T4, T5>(long page, long itemsPerPage, string sql, object[] args)
        {
            string sqlPage;
            var result = GetPageData<TableRow<T1, T2, T3, T4, T5>>(page, itemsPerPage, sql, ref args, out sqlPage);
            TableRow<T1, T2, T3, T4, T5> row = new TableRow<T1, T2, T3, T4, T5>();

            result.Items = ((IEnumerable<TableRow<T1, T2, T3, T4, T5>>)Query(row, sqlPage, args)).ToList();
            return result;
        }

        public List<T> SkipTake<T>(long skip, long take, string sql, object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries(skip, take, sql, ref args, out sqlCount, out sqlPage);
            return Query<T>(sqlPage, args).ToList();
        }

        public List<TableRow<T1, T2>> SkipTake<T1, T2>(long skip, long take, string sql, object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries(skip, take, sql, ref args, out sqlCount, out sqlPage);
            TableRow<T1, T2> row = new TableRow<T1, T2>();
            return ((IEnumerable<TableRow<T1, T2>>)Query(row, sqlPage, args)).ToList();
        }

        public List<TableRow<T1, T2, T3>> SkipTake<T1, T2, T3>(long skip, long take, string sql, object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries(skip, take, sql, ref args, out sqlCount, out sqlPage);
            TableRow<T1, T2, T3> row = new TableRow<T1, T2, T3>();
            return ((IEnumerable<TableRow<T1, T2, T3>>)Query(row, sqlPage, args)).ToList();
        }

        public List<TableRow<T1, T2, T3, T4>> SkipTake<T1, T2, T3, T4>(long skip, long take, string sql, object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries(skip, take, sql, ref args, out sqlCount, out sqlPage);
            TableRow<T1, T2, T3, T4> row = new TableRow<T1, T2, T3, T4>();
            return ((IEnumerable<TableRow<T1, T2, T3, T4>>)Query(row, sqlPage, args)).ToList();
        }

        public List<TableRow<T1, T2, T3, T4, T5>> SkipTake<T1, T2, T3, T4, T5>(long skip, long take, string sql, object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries(skip, take, sql, ref args, out sqlCount, out sqlPage);
            TableRow<T1, T2, T3, T4, T5> row = new TableRow<T1, T2, T3, T4, T5>();
            return ((IEnumerable<TableRow<T1, T2, T3, T4, T5>>)Query(row, sqlPage, args)).ToList();
        }

        private Page<T> GetPageData<T>(long page, long itemsPerPage, string sql, ref object[] args, out string sqlPage)
        {
            string sqlCount;
            BuildPageQueries((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out sqlCount, out sqlPage);

            // Save the one-time command time out and use it for both queries
            var saveTimeout = OneTimeCommandTimeout;

            // Setup the paged result
            var result = new Page<T> {
                CurrentPage = page,
                ItemsPerPage = itemsPerPage,
                TotalItems = ExecuteScalar<long>(sqlCount, args)
            };
            result.TotalPages = result.TotalItems / itemsPerPage;

            if ((result.TotalItems % itemsPerPage) != 0)
                result.TotalPages++;

            OneTimeCommandTimeout = saveTimeout;
            return result;
        }

        private void BuildPageQueries(long skip, long take, string sql, ref object[] args, out string sqlCount, out string sqlPage)
        {
            // Split the SQL
            SQLParts parts;
            if (!_provider.PagingUtility.SplitSQL(sql, out parts))
                throw new Exception("Unable to parse SQL statement for paged query");

            sqlPage = _provider.BuildPageQuery(skip, take, parts, ref args);
            sqlCount = parts.SqlCount;
        }

        #endregion Page SkipTake

        #region Object Insert Update Delete Save

        #region operation: Insert

        public object Insert(object poco, TableNameManger tableNameFixManger)
        {
            if (poco == null)
                throw new ArgumentNullException("poco is null");
            var pd = PocoData.ForType(poco.GetType());
            var tableName = _provider.GetTableName(pd, tableNameFixManger);

            return ExecuteInsert(tableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, poco);
        }

        private static Cache<Tuple<Type, SqlType, string, int>, string> insert = new Cache<Tuple<Type, SqlType, string, int>, string>();
        private object ExecuteInsert(string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new List<IDbDataParameter>(), CommandType.Text)) {
                        var type = poco.GetType();
                        var pd = PocoData.ForType(type);
                        var sql = insert.Get(Tuple.Create(type, _sqlType, tableName, 1), () => {
                            return CteateInsertSql(pd, 1, tableName, primaryKeyName, autoIncrement);
                        });

                        foreach (var i in pd.Columns) {
                            if (i.ResultColumn) continue;
                            if (autoIncrement && primaryKeyName != null && string.Compare(i.ColumnName, primaryKeyName, true) == 0) {
                                continue;
                            }
                            AddParam(cmd, i.GetValue(poco), i.PropertyInfo);
                        }
                        cmd.CommandText = sql;

                        if (!autoIncrement) {
                            DoPreExecute(cmd);
                            cmd.ExecuteNonQuery();
                            OnExecutedCommand(cmd);

                            PocoColumn pkColumn = pd.Columns.FirstOrDefault(q => q.ColumnName == primaryKeyName);
                            if (primaryKeyName != null && pkColumn != null)
                                return pkColumn.GetValue(poco);
                            else
                                return null;
                        }

                        object id = _provider.ExecuteInsert(this, cmd, primaryKeyName);

                        // Assign the ID back to the primary key property
                        if (primaryKeyName != null && !type.Name.Contains("AnonymousType")) {
                            PocoColumn pc = pd.Columns.FirstOrDefault(q => q.ColumnName == primaryKeyName);
                            if (pc != null) {
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
                    throw;
                return null;
            }
        }

        public void Insert<T>(List<T> list, TableNameManger tableNameFixManger, bool quick)
        {
            if (list == null) throw new ArgumentNullException("poco");
            if (list.Count == 0) return;

            var pd = PocoData.ForType(typeof(T));
            var tableName = _provider.GetTableName(pd, tableNameFixManger);

            var errCount = 0;
            var index = 0;
            while (index < list.Count) {
                var count = list.Count - index;
                int size = 1;
                if (count >= 50) {
                    size = 50;
                } else if (count >= 10) {
                    size = 10;
                }
                Exception e = ExecuteInsert<T>(tableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, list, index, size, quick);
                if (e != null) {
                    errCount++;
                    if (errCount > 3) throw e;
                }
                index += size;
            }
        }
        private Exception ExecuteInsert<T>(string tableName, string primaryKeyName, bool autoIncrement, List<T> list, int index2, int size, bool quick)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new List<IDbDataParameter>(), CommandType.Text)) {
                        var type = typeof(T);
                        var pd = PocoData.ForType(type);
                        var sql = insert.Get(Tuple.Create(type, _sqlType, tableName, size), () => {
                            return CteateInsertSql(pd, size, tableName, primaryKeyName, autoIncrement);
                        });

                        for (int j = 0; j < size; j++) {
                            var poco = list[index2 + j];
                            foreach (var i in pd.Columns) {
                                if (i.ResultColumn) continue;
                                if (autoIncrement && primaryKeyName != null && string.Compare(i.ColumnName, primaryKeyName, true) == 0) {
                                    continue;
                                }
                                AddParam(cmd, i.GetValue(poco), i.PropertyInfo);
                            }
                        }

                        cmd.CommandText = sql;

                        if (!autoIncrement || quick) {
                            DoPreExecute(cmd);
                            cmd.ExecuteNonQuery();
                            OnExecutedCommand(cmd);
                            return null;
                        }

                        object id = _provider.ExecuteInsert(this, cmd, primaryKeyName);

                        // Assign the ID back to the primary key property
                        if (primaryKeyName != null && !type.Name.Contains("AnonymousType")) {
                            PocoColumn pc = pd.Columns.FirstOrDefault(q => q.ColumnName == primaryKeyName);
                            if (pc != null && ColumnType.IsNumericType(pc.PropertyInfo.PropertyType)) {
                                for (int i = 0; i < size; i++) {
                                    var poco = list[index2 + i];
                                    if (id.GetType() == typeof(int)) {
                                        pc.SetValue(poco, pc.ChangeType(((int)id) + i));
                                    } else if (id.GetType() == typeof(uint)) {
                                        pc.SetValue(poco, pc.ChangeType(((uint)id) + i));
                                    } else if (id.GetType() == typeof(long)) {
                                        pc.SetValue(poco, pc.ChangeType(((long)id) + (long)i));
                                    } else if (id.GetType() == typeof(ulong)) {
                                        pc.SetValue(poco, pc.ChangeType(((ulong)id) + (ulong)i));
                                    } else if (id.GetType() == typeof(short)) {
                                        pc.SetValue(poco, pc.ChangeType(((short)id) + (short)i));
                                    } else if (id.GetType() == typeof(ushort)) {
                                        pc.SetValue(poco, pc.ChangeType((ushort)id + (ushort)i));
                                    }
                                }
                            }
                        }

                    }
                } finally {
                    CloseSharedConnection();
                }
            } catch (Exception x) {
                OnException(x);
            }
            return null;
        }


        private string CteateInsertSql(PocoData pd, int size, string tableName, string primaryKeyName, bool autoIncrement)
        {
            var names = new List<string>();
            var values = new List<string>();
            var _index = 0;
            foreach (var i in pd.Columns) {
                if (i.ResultColumn) continue;

                // Don't insert the primary key (except under oracle where we need bring in the next sequence value)
                if (autoIncrement && primaryKeyName != null && string.Compare(i.ColumnName, primaryKeyName, true) == 0) {
                    // Setup auto increment expression
                    string autoIncExpression = _provider.GetAutoIncrementExpression(pd.TableInfo);
                    if (autoIncExpression != null) {
                        names.Add(i.ColumnName);
                        values.Add(autoIncExpression);
                    }
                    continue;
                }

                names.Add(_provider.EscapeSqlIdentifier(i.ColumnName));
                values.Add(_paramPrefix + _index.ToString());
                _index++;
            }

            string outputClause = String.Empty;
            if (autoIncrement) {
                outputClause = _provider.GetInsertOutputClause(primaryKeyName);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO {0} ({1}){2} VALUES ({3})",
                _provider.EscapeTableName(tableName),
                string.Join(",", names.ToArray()),
                outputClause,
                string.Join(",", values.ToArray())
                );

            var k = _index;
            for (int i = 1; i < size; i++) {
                sb.Append(",(");
                for (int j = 0; j < k; j++) {
                    if (j > 0) { sb.Append(","); }
                    sb.Append(_paramPrefix);
                    sb.Append(_index.ToString());
                    _index++;
                }
                sb.Append(")");
            }
            return sb.ToString();
        }

        #endregion operation: Insert

        #region operation: Update

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <returns>The number of affected rows</returns>
        public int Update(object poco, TableNameManger tableNameFixManger)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType());
            //var dp = DatabaseProvider.Resolve(_sqlType);
            var tableName = _provider.GetTableName(pd, tableNameFixManger);

            return ExecuteUpdate(tableName, pd.TableInfo.PrimaryKey, poco, null);
        }

        private static Cache<Tuple<Type, SqlType, string>, string> update = new Cache<Tuple<Type, SqlType, string>, string>();
        private int ExecuteUpdate(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            try {
                OpenSharedConnection();
                try {
                    using (var cmd = CreateCommand(_sharedConnection, "", new List<IDbDataParameter>(), CommandType.Text)) {
                        var type = poco.GetType();
                        var pd = PocoData.ForType(poco.GetType());

                        var sql = update.Get(Tuple.Create(type, _sqlType, tableName), () => {
                            var sb = new StringBuilder();
                            var index = 0;
                            foreach (var i in pd.Columns) {
                                if (string.Compare(i.ColumnName, primaryKeyName, true) == 0) continue;
                                if (i.ResultColumn) continue;

                                // Build the sql
                                if (index > 0) sb.Append(", ");
                                sb.AppendFormat("{0} = {1}{2}", _provider.EscapeSqlIdentifier(i.ColumnName), _paramPrefix, index++);

                                // Store the parameter in the command
                                AddParam(cmd, i.GetValue(poco), i.PropertyInfo);
                            }

                            return string.Format("UPDATE {0} SET {1} WHERE {2} = {3}{4}",
                                _provider.EscapeTableName(tableName), sb.ToString(), _provider.EscapeSqlIdentifier(primaryKeyName), _paramPrefix, index++);
                        });

                        cmd.CommandText = sql;

                        foreach (var i in pd.Columns) {
                            if (i.ResultColumn) continue;
                            if (string.Compare(i.ColumnName, primaryKeyName, true) == 0) {
                                if (primaryKeyValue == null) primaryKeyValue = i.GetValue(poco);
                                continue;
                            }
                            AddParam(cmd, i.GetValue(poco), i.PropertyInfo);
                        }

                        // Find the property info for the primary key
                        PropertyInfo pkpi = null;
                        if (primaryKeyName != null) {
                            PocoColumn col = pd.Columns.FirstOrDefault(q => q.ColumnName == primaryKeyName);
                            if (col != null)
                                pkpi = col.PropertyInfo;
                            else
                                pkpi = new { Id = primaryKeyValue }.GetType().GetProperties()[0];
                        }
                        AddParam(cmd, primaryKeyValue, pkpi);

                        DoPreExecute(cmd);
                        var retv = cmd.ExecuteNonQuery();
                        OnExecutedCommand(cmd);
                        return retv;
                    }
                } finally {
                    CloseSharedConnection();
                }
            } catch (Exception x) {
                if (OnException(x))
                    throw;
                return -1;
            }
        }

        #endregion operation: Update

        #region operation: Delete

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <param name="poco">The POCO object specifying the table name and primary key value of the row to be deleted</param>
        /// <returns>The number of rows affected</returns>
        public int Delete(object poco, TableNameManger tableNameFixManger)
        {
            var pd = PocoData.ForType(poco.GetType());
            //var _provider = DatabaseProvider.Resolve(_sqlType);
            var tableName = _provider.GetTableName(pd, tableNameFixManger);
            //var _paramPrefix = _provider.GetParameterPrefix(_connectionString);

            if (pd.TableInfo.PrimaryKey == null) {
                List<object> objs = new List<object>();
                List<string> cols = new List<string>();

                foreach (var item in pd.Columns.Where(q => q.ResultColumn == false)) {
                    cols.Add(string.Format("{0}={1}{2}",
                         _provider.EscapeSqlIdentifier(item.ColumnName),
                         _provider.GetParameterPrefix(_connectionString),
                         objs.Count));
                    objs.Add(item.GetValue(poco));
                }
                var sql = string.Format("DELETE FROM {0} WHERE {1}",
                    tableName, string.Join(" AND ", cols));
                return Execute(sql, objs.ToArray());
            }

            var primaryKeyName = pd.TableInfo.PrimaryKey;
            object primaryKeyValue = null;
            PocoColumn pc = pd.Columns.FirstOrDefault(q => q.ColumnName == primaryKeyName);
            if (pc != null) {
                primaryKeyValue = pc.GetValue(poco);
            }

            var sql2 = string.Format("DELETE FROM {0} WHERE {1}=@0", _provider.EscapeTableName(tableName), _provider.EscapeSqlIdentifier(primaryKeyName));
            return Execute(sql2, new object[] { primaryKeyValue });
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class whose attributes identify the table and primary key to be used in the delete</typeparam>
        /// <param name="pocoOrPrimaryKey">The value of the primary key of the row to delete</param>
        /// <returns></returns>
        public int Delete<T>(object pocoOrPrimaryKey, TableNameManger tableNameFixManger)
        {
            if (pocoOrPrimaryKey.GetType() == typeof(T))
                return Delete(pocoOrPrimaryKey, tableNameFixManger);

            var pd = PocoData.ForType(typeof(T));

            if (pocoOrPrimaryKey.GetType().Name.Contains("AnonymousType")) {
                var pi = pocoOrPrimaryKey.GetType().GetProperty(pd.TableInfo.PrimaryKey);

                if (pi == null)
                    throw new InvalidOperationException(string.Format("Anonymous type does not contain an id for PK column `{0}`.", pd.TableInfo.PrimaryKey));

                pocoOrPrimaryKey = pi.GetValue(pocoOrPrimaryKey, new object[0]);
            }

            var tableName = _provider.GetTableName(pd, tableNameFixManger);

            var sql2 = string.Format("DELETE FROM {0} WHERE {1}=@0", _provider.EscapeTableName(tableName), _provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey));
            return Execute(sql2, new object[] { pocoOrPrimaryKey });
        }

        #endregion operation: Delete

        #region operation: IsNew

        private bool IsNew(string primaryKeyName, PocoData pd, object poco)
        {
            if (string.IsNullOrEmpty(primaryKeyName) || poco is ExpandoObject)
                throw new InvalidOperationException("IsNew() and Save() are only supported on tables with identity (inc auto-increment) primary key columns");

            object pk;
            PocoColumn pc = pd.Columns.FirstOrDefault(q => q.ColumnName == primaryKeyName);
            PropertyInfo pi;
            if (pc != null) {
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
                return (long)pk == default(long);
            if (type == typeof(int))
                return (int)pk == default(int);
            if (type == typeof(Guid))
                return (Guid)pk == default(Guid);
            if (type == typeof(ulong))
                return (ulong)pk == default(ulong);
            if (type == typeof(uint))
                return (uint)pk == default(uint);
            if (type == typeof(short))
                return (short)pk == default(short);
            if (type == typeof(ushort))
                return (ushort)pk == default(ushort);

            // Create a default instance and compare
            return pk == Activator.CreateInstance(pk.GetType());
        }

        /// <summary>
        ///     Check if a poco represents a new row
        /// </summary>
        /// <param name="poco">The object instance whose "newness" is to be tested</param>
        /// <returns>True if the POCO represents a record already in the database</returns>
        /// <remarks>This method simply tests if the POCO's primary key column property has been set to something non-zero.</remarks>
        public bool IsNew(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException("poco");

            var pd = PocoData.ForType(poco.GetType());
            return IsNew(pd.TableInfo.PrimaryKey, pd, poco);
        }

        #endregion operation: IsNew

        #region operation: Save

        /// <summary>
        ///     Saves a POCO by either performing either an SQL Insert or SQL Update
        /// </summary>
        /// <param name="poco">The POCO object to be saved</param>
        public void Save(object poco, TableNameManger tableNameFixManger)
        {
            var pd = PocoData.ForType(poco.GetType());
            //var _provider = DatabaseProvider.Resolve(_sqlType);
            var tableName = _provider.GetTableName(pd, tableNameFixManger);
            //var _paramPrefix = _provider.GetParameterPrefix(_connectionString);

            if (IsNew(pd.TableInfo.PrimaryKey, pd, poco)) {
                ExecuteInsert(tableName, pd.TableInfo.PrimaryKey, true, poco);
            } else {
                ExecuteUpdate(tableName, pd.TableInfo.PrimaryKey, poco, null);
            }
        }

        #endregion operation: Save

        #endregion Object Insert Update Delete Save

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

        #endregion Internal operations
    }
}