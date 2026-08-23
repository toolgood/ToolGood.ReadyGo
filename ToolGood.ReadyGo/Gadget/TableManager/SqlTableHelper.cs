using System;
using ToolGood.ReadyGo.Gadget.TableManager.Providers;

namespace ToolGood.ReadyGo.Gadget.TableManager
{
    /// <summary>
    /// 表结构管理助手（生成并执行建表、删表、清空表等 SQL）
    /// </summary>
    public class SqlTableHelper
    {
        private readonly SqlHelper _sqlHelper;

        /// <summary>
        /// 初始化表结构管理助手
        /// </summary>
        /// <param name="sqlhelper">所属 SqlHelper</param>
        public SqlTableHelper(SqlHelper sqlhelper)
        {
            _sqlHelper = sqlhelper;
        }

        /// <summary>
        /// 获取“表不存在时创建”的建表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="withIndex">是否同时生成索引</param>
        /// <returns>建表 SQL</returns>
        public string GetTryCreateTable(Type type, bool withIndex = true)
        {
            var dp = DatabaseProvider.Resolve(_sqlHelper._sqlType);
            return dp.GetTryCreateTable(type, withIndex);
        }

        /// <summary>
        /// 获取建表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="withIndex">是否同时生成索引</param>
        /// <returns>建表 SQL</returns>
        public string GetCreateTable(Type type, bool withIndex = true)
        {
            var dp = DatabaseProvider.Resolve(_sqlHelper._sqlType);
            return dp.GetCreateTable(type, withIndex);
        }

        /// <summary>
        /// 获取创建索引 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>创建索引 SQL</returns>
        public string GetCreateTableIndex(Type type)
        {
            var dp = DatabaseProvider.Resolve(_sqlHelper._sqlType);
            return dp.GetCreateIndex(type);
        }

        /// <summary>
        /// 获取删除表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>删除表 SQL</returns>
        public string GetDropTable(Type type)
        {
            var dp = DatabaseProvider.Resolve(_sqlHelper._sqlType);
            return dp.GetDropTable(type);
        }

        /// <summary>
        /// 获取删除表 SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>删除表 SQL</returns>
        public string GetDropTable(string tableName)
        {
            var dp = DatabaseProvider.Resolve(_sqlHelper._sqlType);
            return dp.GetDropTable(tableName);
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>清空表 SQL</returns>
        public string GetTruncateTable(Type type)
        {
            var dp = DatabaseProvider.Resolve(_sqlHelper._sqlType);
            return dp.GetTruncateTable(type);
        }

        /// <summary>
        /// 获取清空表 SQL
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>清空表 SQL</returns>
        public string GetTruncateTable(string tableName)
        {
            var dp = DatabaseProvider.Resolve(_sqlHelper._sqlType);
            return dp.GetTruncateTable(tableName);
        }

        /// <summary>
        /// 执行建表（表不存在时才创建）
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="withIndex">是否同时创建索引</param>
        public void TryCreateTable(Type type, bool withIndex = true)
        {
            var sql = GetTryCreateTable(type, withIndex);
            _sqlHelper.Execute(sql);
        }

        /// <summary>
        /// 执行建表
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="withIndex">是否同时创建索引</param>
        public void CreateTable(Type type, bool withIndex = true)
        {
            var sql = GetCreateTable(type, withIndex);
            _sqlHelper.Execute(sql);
        }

        /// <summary>
        /// 执行创建索引
        /// </summary>
        /// <param name="type">实体类型</param>
        public void CreateTableIndex(Type type)
        {
            var sql = GetCreateTableIndex(type);
            if (string.IsNullOrEmpty(sql)) { return; }
            _sqlHelper.Execute(sql);
        }

        /// <summary>
        /// 执行删除表
        /// </summary>
        /// <param name="type">实体类型</param>
        public void DropTable(Type type)
        {
            var sql = GetDropTable(type);
            _sqlHelper.Execute(sql);
        }

        /// <summary>
        /// 执行删除表
        /// </summary>
        /// <param name="tableName">表名</param>
        public void DropTable(string tableName)
        {
            var sql = GetDropTable(tableName);
            _sqlHelper.Execute(sql);
        }

        /// <summary>
        /// 执行清空表
        /// </summary>
        /// <param name="type">实体类型</param>
        public void TruncateTable(Type type)
        {
            var sql = GetTruncateTable(type);
            _sqlHelper.Execute(sql);
        }

        /// <summary>
        /// 执行清空表
        /// </summary>
        /// <param name="tableName">表名</param>
        public void TruncateTable(string tableName)
        {
            var sql = GetTruncateTable(tableName);
            _sqlHelper.Execute(sql);
        }

    }
}
