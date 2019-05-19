﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ToolGood.ReadyGo3.Gadget.Caches;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3.StoredProcedure
{
    /// <summary>
    /// 存储过程
    /// </summary>
    public abstract class SqlProcess : IDisposable
    {
        private readonly bool _singleSqlHelper;
        private readonly SqlHelper _sqlhelper;
        private readonly Dictionary<string, IDbDataParameter> _parameters = new Dictionary<string, IDbDataParameter>();
        private readonly List<IDbDataParameter> _outs = new List<IDbDataParameter>();
        /// <summary>
        /// 存储过程名称
        /// </summary>
        protected abstract string ProcessName { get; set; }

        #region 构造函数
        /// <summary>
        /// SqlProcess构造函数
        /// </summary>
        /// <param name="helper"></param>
        protected SqlProcess(SqlHelper helper)
        {
            _singleSqlHelper = false;
            _sqlhelper = helper;
            Init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Init();
        /// <summary>
        /// 释放内存
        /// </summary>
        public void Dispose()
        {
            if (_singleSqlHelper) {
                _sqlhelper.Dispose();
            }
        }

        #endregion 构造函数

        #region Add _G _S
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="isOutput"></param>
        protected void Add<T>(string name, bool isOutput)
        {
            var _p = _sqlhelper._factory.CreateParameter();
            _p.ParameterName = name;
            if (isOutput) {
                _p.Direction = ParameterDirection.InputOutput;
                _outs.Add(_p);
            }
            _parameters[name] = _p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected T _G<T>(string name)
        {
            return (T)(_parameters[name].Value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void _S<T>(string name, T value)
        {
            var _p = _parameters[name];
            var dp = DatabaseProvider.Resolve(_sqlhelper._sqlType);
            convertToDBtype(value, _p, dp);
        }

        private void convertToDBtype(object value, IDbDataParameter p, DatabaseProvider _provider)
        {
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
            } else if (value.GetType().Name == "SqlGeography") {
                p.GetType().GetProperty("UdtTypeName").SetValue(p, "geography", null); //geography is the equivalent SQL Server Type
                p.Value = value;
            } else if (value.GetType().Name == "SqlGeometry") {
                p.GetType().GetProperty("UdtTypeName").SetValue(p, "geometry", null); //geography is the equivalent SQL Server Type
                p.Value = value;
            } else if (t == typeof(DateTimeOffset)) {
                p.Value = ((DateTimeOffset)value).LocalDateTime;
            } else {
                p.Value = value;
            }
        }
        #endregion Add _G _S

        #region 执行
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行的行数</returns>
        public int Execute()
        {
            var args = _parameters.Select(q => (object)q.Value).ToArray();
            var db = _sqlhelper.getDatabase();
            return db.Execute(ProcessName, args, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ExecuteScalar<T>()
        {
            var args = _parameters.Select(q => (object)q.Value).ToArray();
            var db = _sqlhelper.getDatabase();
            return db.ExecuteScalar<T>(ProcessName, args, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public DataTable ExecuteDataTable()
        {
            var args = _parameters.Select(q => (object)q.Value).ToArray();
            var db = _sqlhelper.getDatabase();
            return db.ExecuteDataTable(ProcessName, args, CommandType.StoredProcedure);
        }
#if !NETSTANDARD2_0
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteDataSet()
        {
            var args = _parameters.Select(q => (object)q.Value).ToArray();
            var db = _sqlhelper.getDatabase();
            return db.ExecuteDataSet(ProcessName, args, CommandType.StoredProcedure);
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> Select<T>() where T : class, new()
        {
            var args = _parameters.Select(q => (object)q.Value).ToArray();
            var db = _sqlhelper.getDatabase();
            return db.Query<T>(ProcessName, args, CommandType.StoredProcedure).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Single<T>()
        {
            var args = _parameters.Select(q => (object)q.Value).ToArray();
            var db = _sqlhelper.getDatabase();
            return db.Query<T>(ProcessName, args, CommandType.StoredProcedure).Single();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SingleOrDefault<T>()
        {
            var args = _parameters.Select(q => (object)q.Value).ToArray();
            var db = _sqlhelper.getDatabase();
            return db.Query<T>(ProcessName, args, CommandType.StoredProcedure).SingleOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T First<T>()
        {
            var args = _parameters.Select(q => (object)q.Value).ToArray();
            var db = _sqlhelper.getDatabase();
            return db.Query<T>(ProcessName, args, CommandType.StoredProcedure).First();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FirstOrDefault<T>()
        {
            var args = _parameters.Select(q => (object)q.Value).ToArray();
            var db = _sqlhelper.getDatabase();
            return db.Query<T>(ProcessName, args, CommandType.StoredProcedure).FirstOrDefault();
        }



        #endregion 执行
    }
}