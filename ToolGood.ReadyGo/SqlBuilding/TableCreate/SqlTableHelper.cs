using System;
using System.Text;
using ToolGood.ReadyGo.Poco;
using ToolGood.ReadyGo.Providers;

namespace ToolGood.ReadyGo.SqlBuilding.TableCreate
{
    public class SqlTableHelper
    {
        private SqlHelper _helper;
        private Database database;
        private DatabaseProvider dbType;
        private CreateTableHelper tableHelper;
        //dbType = database.Provider;

        internal SqlTableHelper(SqlHelper helper)
        {
            _helper = helper;
            database = helper.getDatabase(ConnectionType.Default);
            dbType = DatabaseProvider.Resolve(_helper._sqlType);
            tableHelper = CreateTableHelper.Resolve(_helper._sqlType);
        }
        /// <summary>
        /// 显示换行
        /// </summary>
        public bool ShowLine { get; set; }

        private string Wrap()
        {
            return ShowLine ? "\r\n" : "";
        }

        private Table getTable(Type type)
        {
            return Table.FromType(type);
        }

        /// <summary>
        /// 获取删除数据表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetDeleteTable(Type type)
        {
            Table ti = getTable(type);
            var tableName = tableHelper.GetTableName(ti, _helper._tableNameManger);
            return tableHelper.DeleteTable(tableName);
        }
        /// <summary>
        /// 获取创建数据表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetCreateTable(Type type)
        {
            Table ti = getTable(type);
            var tableName = tableHelper.GetTableName(ti, _helper._tableNameManger);
            return tableHelper.CreateTable(tableName, ti, ti.Columns, Wrap());
        }
        /// <summary>
        /// 获取创建普通索引
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetCreateIndex(Type type)
        {
            Table ti = getTable(type);
            var tableName = tableHelper.GetTableName(ti, _helper._tableNameManger);

            StringBuilder sb = new StringBuilder();
            foreach (var item in ti.Indexs) {
                sb.Append(tableHelper.CreateIndex(tableName, item));
                sb.Append(Wrap());
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取创建唯一索引
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetCreateUnique(Type type)
        {
            Table ti = getTable(type);
            var tableName = tableHelper.GetTableName(ti, _helper._tableNameManger);

            StringBuilder sb = new StringBuilder();
            foreach (var item in ti.Uniques) {
                sb.Append(tableHelper.CreateUnique(tableName, item));
                sb.Append(Wrap());
            }
            return sb.ToString();
        }

        /// <summary>
        /// 创建数据表 包括普通索引及唯一索引
        /// </summary>
        /// <param name="type"></param>
        public void CreateTable(Type type)
        {
            var database = _helper.getDatabase(ConnectionType.Write);
            database.Execute(GetCreateTable(type), new object[0]);
            var sql = GetCreateIndex(type);
            if (sql!="") {
                database.Execute(sql, new object[0]);
            }
            sql = GetCreateUnique(type);
            if (sql != "") {
                database.Execute(sql, new object[0]);
            }
        }

        /// <summary>
        /// 尝试创建数据表 包括普通索引及唯一索引
        /// </summary>
        /// <param name="type"></param>
        public void TryCreateTable(Type type)
        {
            try {
                CreateTable(type);
            } catch (Exception) { }
        }

        /// <summary>
        /// 删除数据表
        /// </summary>
        /// <param name="type"></param>
        public void DeleteTable(Type type)
        {
            var database = _helper.getDatabase(ConnectionType.Write);

            database.Execute(GetDeleteTable(type), new object[0]);
        }

        /// <summary>
        /// 重置数据表
        /// </summary>
        /// <param name="type"></param>
        public void ResetTable(Type type)
        {
            var database = _helper.getDatabase(ConnectionType.Write);
            database.Execute(GetDeleteTable(type), new object[0]);

            CreateTable(type);
            //database.Execute(GetCreateTable(type), new object[0]);
        }
        /// <summary>
        /// 获取创建数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CreateTable<T>()
        {
            CreateTable(typeof(T));
        }

        /// <summary>
        /// 尝试创建数据表 包括普通索引及唯一索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void TryCreateTable<T>()
        {
            TryCreateTable(typeof(T));
        }

        /// <summary>
        /// 重置数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ResetTable<T>()
        {
            ResetTable(typeof(T));
        }
        /// <summary>
        /// 删除数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DeleteTable<T>()
        {
            DeleteTable(typeof(T));
        }
    }
}