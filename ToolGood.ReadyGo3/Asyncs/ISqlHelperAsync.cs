using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.Gadget.Caches;
using ToolGood.ReadyGo3.Gadget.Events;
using ToolGood.ReadyGo3.Gadget.Internals;
using ToolGood.ReadyGo3.Gadget.TableManager;
using ToolGood.ReadyGo3.PetaPoco;

#if !NET40

namespace ToolGood.ReadyGo3
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISqlHelperAsync
    {
        /// <summary>
        /// 是否释放
        /// </summary>
        bool _IsDisposed { get; }
        /// <summary>
        /// SQL设置
        /// </summary>
        SqlRecord _Sql { get; }
        /// <summary>
        /// 数据库配置
        /// </summary>
        SqlConfig _Config { get; }
        /// <summary>
        /// SQL操作事件
        /// </summary>
        SqlEvents _Events { get; }
        /// <summary>
        /// 
        /// </summary>
        SqlTableHelper _TableHelper { get; }
        /// <summary>
        /// 表名设置
        /// </summary>
        TableNameManager _TableNameManager { get; }



        //
        // 摘要:
        //     执行SQL 查询,返回数量
        //
        // 参数:
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        Task<int> CountAsync<T>(string sql = "", params object[] args);
        //
        // 摘要:
        //     执行SQL 查询,返回数量
        //
        // 参数:
        //   sql:
        //
        //   args:
        Task<int> CountAsync(string sql, params object[] args);


        //
        // 摘要:
        //     删除
        //
        // 参数:
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        Task<int> DeleteAsync<T>(string sql, params object[] args);
        //
        // 摘要:
        //     删除
        //
        // 参数:
        //   poco:
        //     对象
        Task<int> DeleteAsync<T>(T poco) where T : class;

        //
        // 摘要:
        //     删除
        //
        // 参数:
        //   primaryKey:
        //     主键
        //
        // 类型参数:
        //   T:
        Task<int> DeleteByIdAsync<T>(object primaryKey);
        //
        // 摘要:
        //     释放
        void Dispose();

        //
        // 摘要:
        //     执行 SQL 语句，并返回受影响的行数
        //
        // 参数:
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 返回结果:
        //     返回受影响的行数
        Task<int> ExecuteAsync(string sql, params object[] args);

        //
        // 摘要:
        //     执行SQL 查询,返回 DataTable
        //
        // 参数:
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 返回结果:
        //     返回 DataTable
        Task<DataTable> ExecuteDataTableAsync(string sql, params object[] args);

        //
        // 摘要:
        //     执行SQL 查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        //
        // 参数:
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        //
        // 返回结果:
        //     返回查询所返回的结果集中第一行的第一列。忽略额外的列或行
        Task<T> ExecuteScalarAsync<T>(string sql = "", params object[] args);

        //
        // 摘要:
        //     执行SQL 查询,判断是否存在，返回bool类型
        //
        // 参数:
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        Task<bool> ExistsAsync<T>(string sql, params object[] args);
        //
        // 摘要:
        //     执行SQL 查询,判断是否存在，返回bool类型
        //
        // 参数:
        //   primaryKey:
        //     主键值
        //
        // 类型参数:
        //   T:
        Task<bool> ExistsAsync<T>(object primaryKey);

        //
        // 摘要:
        //     获取第一个类型，若数量为0，则抛出异常
        //
        // 参数:
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        Task<T> FirstAsync<T>(string sql = "", params object[] args);

        //
        // 摘要:
        //     获取第一个类型
        //
        // 参数:
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        Task<T> FirstOrDefaultAsync<T>(string sql = "", params object[] args);

        //
        // 摘要:
        //     插入，支持主键自动获取。
        //
        // 参数:
        //   poco:
        //     对象
        Task<object> InsertAsync<T>(T poco) where T : class;

        //
        // 摘要:
        //     插入集合，不返回主键
        //
        // 参数:
        //   list:
        //
        // 类型参数:
        //   T:
        Task InsertListAsync<T>(List<T> list) where T : class;
        //
        // 摘要:
        //     生成序列化的Guid
        Guid NewGuid();

        //
        // 摘要:
        //     执行SQL 查询,返回Page类型
        //
        // 参数:
        //   page:
        //     页数
        //
        //   itemsPerPage:
        //     每页数量
        //
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sql = "", params object[] args);

        //
        // 摘要:
        //     保存
        //
        // 参数:
        //   poco:
        Task SaveAsync(object poco);

        //
        // 摘要:
        //     执行SQL 查询,返回集合
        //
        // 参数:
        //   offset:
        //     跳过
        //
        //   limit:
        //     获取个数
        //
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        Task<List<T>> SelectAsync<T>(long limit, long offset, string sql = "", params object[] args);
        //
        // 摘要:
        //     执行SQL 查询,返回集合
        //
        // 参数:
        //   limit:
        //     获取个数
        //
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        Task<List<T>> SelectAsync<T>(long limit, string sql = "", params object[] args);
        //
        // 摘要:
        //     执行SQL 查询,返回集合
        //
        // 参数:
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        Task<List<T>> SelectAsync<T>(string sql = "", params object[] args);

        //
        // 摘要:
        //     获取唯一一个类型，若数量不为1，则抛出异常
        //
        // 参数:
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        Task<T> SingleAsync<T>(string sql = "", params object[] args);

        //
        // 摘要:
        //     获取唯一一个类型，若数量不为1，则抛出异常
        //
        // 参数:
        //   primaryKey:
        //     主键名
        //
        // 类型参数:
        //   T:
        Task<T> SingleByIdAsync<T>(object primaryKey);

        //
        // 摘要:
        //     获取唯一一个类型，若数量大于1，则抛出异常
        //
        // 参数:
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        Task<T> SingleOrDefaultAsync<T>(string sql = "", params object[] args);

        //
        // 摘要:
        //     获取唯一一个类型，若数量大于1，则抛出异常
        //
        // 参数:
        //   primaryKey:
        //     主键名
        //
        // 类型参数:
        //   T:
        Task<T> SingleOrDefaultByIdAsync<T>(object primaryKey);

        // 摘要:
        //     更新
        //
        // 参数:
        //   sql:
        //     SQL 语句
        //
        //   args:
        //     SQL 参数
        //
        // 类型参数:
        //   T:
        Task<int> UpdateAsync<T>(string sql, params object[] args);
        //
        // 摘要:
        //     更新
        //
        // 参数:
        //   poco:
        //     对象
        Task<int> UpdateAsync<T>(T poco) where T : class;
        //
        // 摘要:
        //     使用缓存
        //
        // 参数:
        //   second:
        //
        //   cacheTag:
        //
        //   cacheService:
        SqlHelper UseChache(int second, string cacheTag = null, ICacheService cacheService = null);
        //
        // 摘要:
        //     使用事务
        Transaction UseTransaction();
        ////
        //// 摘要:
        ////     动态Sql拼接，
        ////
        //// 参数:
        ////   where:
        ////
        ////   args:
        ////
        //// 类型参数:
        ////   T:
        // WhereHelper<T> Where<T>(string where, params object[] args) where T : class, new();
        ////
        //// 摘要:
        ////     动态Sql拼接，
        ////
        //// 参数:
        ////   where:
        ////
        //// 类型参数:
        ////   T:
        // WhereHelper<T> Where<T>(string where) where T : class, new();
        ////
        //// 摘要:
        ////     动态Sql拼接
        ////
        //// 类型参数:
        ////   T:
        // WhereHelper<T> Where<T>() where T : class, new();
        ////
        //// 摘要:
        ////     动态Sql拼接
        ////
        //// 参数:
        ////   where:
        // WhereHelper<T> Where<T>(Expression<Func<T, bool>> where) where T : class, new();
    }
}
#endif