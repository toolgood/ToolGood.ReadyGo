using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// The interface to define value object mapping
    /// </summary>
    /// <typeparam name="TColumnType">Type of the column to map to</typeparam>
    public interface IValueObject<TColumnType> : IValueObject
    {
        /// <summary>
        /// 获取或设置值对象所封装的列值。
        /// </summary>
        TColumnType Value { get; set; }
    }
        
    /// <summary>
    /// 值对象映射的标记接口，用于标识该类型需要按值对象方式映射到数据库列。
    /// </summary>
    public interface IValueObject
    {
        
    }
}
