using ToolGood.ReadyGo.NPoco;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 序列化列标签：用于标记需要自定义序列化的列。
    /// </summary>
    public class SerializedColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 序列化列标签
        /// </summary>
        public SerializedColumnAttribute()
        {
            
        }

        /// <summary>
        /// 序列化列标签
        /// </summary>
        /// <param name="name">列名</param>
        public SerializedColumnAttribute(string name) : base(name)
        {
            
        }

        /// <summary>
        /// 列级序列化器；未指定时返回 null，表示使用全局默认序列化器（FastJsonColumnSerializer）。
        /// </summary>
        public virtual IColumnSerializer Serializer => null;
    }
}
