using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ToolGood.ReadyGo3.Gadget.Caches;
using ToolGood.ReadyGo3.Gadget.Events;
using ToolGood.ReadyGo3.Gadget.Internals;
using ToolGood.ReadyGo3.Gadget.TableManager;
using ToolGood.ReadyGo3.LinQ;
using ToolGood.ReadyGo3.PetaPoco;

namespace ToolGood.ReadyGo3.Interfaces
{
    /// <summary>
    /// SqlHelper通用方法
    /// </summary>
    public interface ISqlHelperBase: IDisposable
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

        /// <summary>
        /// 生成有序的Guid
        /// </summary>
        /// <returns></returns>
        Guid NewGuid();

        /// <summary>
        /// 使用缓存
        /// </summary>
        /// <param name="second"></param>
        /// <param name="cacheTag"></param>
        /// <param name="cacheService"></param>
        /// <returns></returns>
        SqlHelper UseChache(int second, string cacheTag = null, ICacheService cacheService = null);
        /// <summary>
        /// 使用事务
        /// </summary>
        /// <returns></returns>
        Transaction UseTransaction();


        /// <summary>
        /// 动态Sql拼接，
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        WhereHelper<T> Where<T>(string where, params object[] args) where T : class, new();
        /// <summary>
        /// 动态Sql拼接，
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        WhereHelper<T> Where<T>(string where) where T : class, new();
        /// <summary>
        /// 动态Sql拼接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        WhereHelper<T> Where<T>() where T : class, new();
        /// <summary>
        /// 动态Sql拼接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        WhereHelper<T> Where<T>(Expression<Func<T, bool>> where) where T : class, new();

    }
}
