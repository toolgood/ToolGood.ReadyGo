using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ToolGood.ReadyGo.Caches;
using ToolGood.ReadyGo.Poco;
using ToolGood.ReadyGo.Providers;

namespace ToolGood.ReadyGo
{
    public abstract class SqlProcess : IDisposable, IUseCache
    {
        private bool _singleSqlHelper;
        internal SqlHelper _sqlhelper;
        private Dictionary<string, IDbDataParameter> _parameters = new Dictionary<string, IDbDataParameter>();
        private List<IDbDataParameter> _Outs = new List<IDbDataParameter>();
        //private Dictionary<string, object> _args = new Dictionary<string, object>();
        protected abstract string ProcessName { get;  }

        #region 构造函数

        public SqlProcess(string connectionStringName)
        {
            _singleSqlHelper = true;
            _sqlhelper = new SqlHelper(connectionStringName);
            OnInit();
        }

        public SqlProcess(string connectionString, string providerName)
        {
            _singleSqlHelper = true;
            _sqlhelper = new SqlHelper(connectionString, providerName);
            OnInit();
        }

        public SqlProcess(SqlHelper helper)
        {
            _singleSqlHelper = false;
            _sqlhelper = helper;
            OnInit();
        }

        protected abstract void OnInit();

        public void Dispose()
        {
            if (_singleSqlHelper) {
                _sqlhelper.Dispose();
            }
        }

        #endregion 构造函数

        #region Add _G _S

        public void Add<T>(string name, bool isOutput)
        {
            var _p = _sqlhelper._factory.CreateParameter();
            _p.ParameterName = name;
            if (isOutput) {
                _p.Direction = ParameterDirection.InputOutput;
                _Outs.Add(_p);
            }
            _parameters[name] = _p;
        }

        protected T _G<T>(string name)
        {
            return (T)(_parameters[name].Value);
        }

        protected void _S<T>(string name, T value)
        {
            var _p = _parameters[name];
            var dp = DatabaseProvider.Resolve(_sqlhelper._sqlType);
            Database.convertToDBtype(value, _p, dp);
        }

        #endregion Add _G _S

        #region 05 缓存设置

        private ICacheService _cacheService;
        private bool _usedCacheService;
        private int _cacheTime;
        private string _cacheTag;

        void IUseCache.useCache(int second, string cacheTag, ICacheService cacheService)
        {
            _usedCacheService = true;
            _cacheTime = second;
            _cacheTag = cacheTag;
            if (cacheService!=null) {
                _cacheService = cacheService;
            }
        }

        //protected internal void useCache(int second, string cacheTag = null, ICacheService cacheService = null)
        //{
        //    _usedCacheService = true;
        //    _cacheTime = second;
        //    _cacheTag = cacheTag;
        //    _cacheService = cacheService;
        //}

        protected internal void setCache()
        {
            if (_usedCacheService) {
                _sqlhelper.useCache(_cacheTime, _cacheTag, _cacheService);

                //if (_cacheService != null) {
                //} else if (string.IsNullOrEmpty(_cacheTag) == false) {
                //    _sqlhelper.UseCache(_cacheTime, _cacheTag);
                //} else {
                //    _sqlhelper.UseCache(_cacheTime);
                //}
            }
        }

        #endregion 05 缓存设置

        #region 执行

        public int Execute()
        {
            setCache();
            var args = _parameters.Select(q =>q.Value.Value).ToArray();
            var run = _sqlhelper.Run(ProcessName, args, () => {
                var db = _sqlhelper.getDatabase(ConnectionType.Write);
                db.commandType = CommandType.StoredProcedure;
                var dt = db.Execute(ProcessName, _parameters.Select(q => q.Value).ToList());
                var objs = _Outs.Select(q => q.Value).ToArray();
                return Tuple.Create(dt, objs);
            }, "SqlProcess.Execute");
            for (int i = 0; i < _Outs.Count; i++) {
                _Outs[i].Value = run.Item2[i];
            }
            return run.Item1;
        }

        public T ExecuteScalar<T>()
        {
            setCache();
            var args = _parameters.Select(q => q.Value.Value).ToArray();

            var run = _sqlhelper.Run(ProcessName, args, () => {
                var db = _sqlhelper.getDatabase(ConnectionType.Default);
                db.commandType = CommandType.StoredProcedure;
                var dt = db.ExecuteScalar<T>(ProcessName, _parameters.Select(q => q.Value).ToList());
                var objs = _Outs.Select(q => q.Value).ToArray();
                return Tuple.Create(dt, objs);
            }, "SqlProcess.ExecuteScalar");
            for (int i = 0; i < _Outs.Count; i++) {
                _Outs[i].Value = run.Item2[i];
            }
            return run.Item1;
        }

        public DataTable ExecuteDataTable()
        {
            setCache();
            var args = _parameters.Select(q => q.Value.Value).ToArray();

            var run = _sqlhelper.Run(ProcessName, args, () => {
                var db = _sqlhelper.getDatabase(ConnectionType.Write);
                db.commandType = CommandType.StoredProcedure;
                var dt = db.GetDataTable(ProcessName, _parameters.Select(q => q.Value).ToList());
                var objs = _Outs.Select(q => q.Value).ToArray();
                return Tuple.Create(dt, objs);
            }, "SqlProcess.ExecuteDataTable");
            for (int i = 0; i < _Outs.Count; i++) {
                _Outs[i].Value = run.Item2[i];
            }
            return run.Item1;
        }

        public DataSet ExecuteDataSet()
        {
            setCache();
            var args = _parameters.Select(q => q.Value.Value).ToArray();

            var run = _sqlhelper.Run(ProcessName, args, () => {
                var db = _sqlhelper.getDatabase(ConnectionType.Write);
                db.commandType = CommandType.StoredProcedure;
                var dt = db.ExecuteDataSet(ProcessName, _parameters.Select(q => q.Value).ToList());
                var objs = _Outs.Select(q => q.Value).ToArray();
                return Tuple.Create(dt, objs);
            }, "SqlProcess.ExecuteDataSet");
            for (int i = 0; i < _Outs.Count; i++) {
                _Outs[i].Value = run.Item2[i];
            }
            return run.Item1;
        }

        public List<T> Select<T>() where T : class, new()
        {
            setCache();
            var args = _parameters.Select(q => q.Value.Value).ToArray();
            var run = _sqlhelper.Run(ProcessName, args, () => {
                var db = _sqlhelper.getDatabase(ConnectionType.Write);
                db.commandType = CommandType.StoredProcedure;
                var dt = db.Query<T>(ProcessName, _parameters.Select(q => q.Value).ToList()).ToList();
                var objs = _Outs.Select(q => q.Value).ToArray();
                return Tuple.Create(dt, objs);
            }, "SqlProcess.Select");
            for (int i = 0; i < _Outs.Count; i++) {
                _Outs[i].Value = run.Item2[i];
            }
            return run.Item1;
        }

        public T Single<T>()
        {
            setCache();
            var args = _parameters.Select(q => q.Value.Value).ToArray();

            var run = _sqlhelper.Run(ProcessName, args, () => {
                var db = _sqlhelper.getDatabase(ConnectionType.Write);
                db.commandType = CommandType.StoredProcedure;
                var dt = db.Query<T>(ProcessName, _parameters.Select(q => q.Value).ToList()).Single();
                var objs = _Outs.Select(q => q.Value).ToArray();
                return Tuple.Create(dt, objs);
            }, "SqlProcess.Single");
            for (int i = 0; i < _Outs.Count; i++) {
                _Outs[i].Value = run.Item2[i];
            }
            return run.Item1;
        }

        public T SingleOrDefault<T>()
        {
            setCache();
            var args = _parameters.Select(q => q.Value.Value).ToArray();

            var run = _sqlhelper.Run(ProcessName, args, () => {
                var db = _sqlhelper.getDatabase(ConnectionType.Write);
                db.commandType = CommandType.StoredProcedure;
                var dt = db.Query<T>(ProcessName, _parameters.Select(q => q.Value).ToList()).SingleOrDefault();
                var objs = _Outs.Select(q => q.Value).ToArray();
                return Tuple.Create(dt, objs);
            }, "SqlProcess.SingleOrDefault");
            for (int i = 0; i < _Outs.Count; i++) {
                _Outs[i].Value = run.Item2[i];
            }
            return run.Item1;
        }

        public T First<T>()
        {
            setCache();
            var args = _parameters.Select(q => q.Value.Value).ToArray();

            var run = _sqlhelper.Run(ProcessName, args, () => {
                var db = _sqlhelper.getDatabase(ConnectionType.Write);
                db.commandType = CommandType.StoredProcedure;
                var dt = db.Query<T>(ProcessName, _parameters.Select(q => q.Value).ToList()).First();
                var objs = _Outs.Select(q => q.Value).ToArray();
                return Tuple.Create(dt, objs);
            }, "SqlProcess.First");
            for (int i = 0; i < _Outs.Count; i++) {
                _Outs[i].Value = run.Item2[i];
            }
            return run.Item1;
        }

        public T FirstOrDefault<T>()
        {
            setCache();
            var args = _parameters.Select(q => q.Value.Value).ToArray();

            var run = _sqlhelper.Run(ProcessName, args, () => {
                var db = _sqlhelper.getDatabase(ConnectionType.Write);
                db.commandType = CommandType.StoredProcedure;

                var dt = db.Query<T>(ProcessName, _parameters.Select(q => q.Value).ToList()).FirstOrDefault();
                var objs = _Outs.Select(q => q.Value).ToArray();
                return Tuple.Create(dt, objs);
            }, "SqlProcess.FirstOrDefault");
            for (int i = 0; i < _Outs.Count; i++) {
                _Outs[i].Value = run.Item2[i];
            }
            return run.Item1;
        }



        #endregion 执行
    }
}