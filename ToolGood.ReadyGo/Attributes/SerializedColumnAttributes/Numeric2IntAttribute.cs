using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 小数转整数标签：保存时数值 × 10^scale 四舍五入存为 int，读取时 ÷ 10^scale 并四舍五入还原。
    /// 例：scale=2 时，1.23 存为 123，读取 123 还原为 1.23。
    /// 值超出 int 范围（×10^scale 后超过 ±21.4 亿）会抛异常，大数请用 [Numeric2Long]。
    /// 基于 SerializedColumn + IColumnSerializer 实现。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Numeric2IntAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 小数点位数
        /// </summary>
        public int Scale { get; }

        /// <summary>
        /// 列级序列化器
        /// </summary>
        public Numeric2IntColumnSerializer Serializer { get; }

        /// <summary>
        /// 小数转整数标签
        /// </summary>
        /// <param name="scale">小数点位数，如 2 表示 ×100</param>
        public Numeric2IntAttribute(int scale = 2)
        {
            Scale = scale;
            Serializer = new Numeric2IntColumnSerializer(scale);
        }

        /// <summary>
        /// 小数转整数标签
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="scale">小数点位数，如 2 表示 ×100</param>
        public Numeric2IntAttribute(string name, int scale = 2) : base(name)
        {
            Scale = scale;
            Serializer = new Numeric2IntColumnSerializer(scale);
        }
    }
}
