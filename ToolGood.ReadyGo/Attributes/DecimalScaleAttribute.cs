using System;
using ToolGood.ReadyGo.Attributes.ColumnSerializers;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 小数转整数标签：保存时数值 × 10^scale 存为整数，读取时 ÷ 10^scale 并四舍五入。
    /// 例：scale=2 时，1.23 存为 123，读取 123 还原为 1.23。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DecimalScaleAttribute : Attributes.SerializedColumnAttribute
    {
        /// <summary>
        /// 小数点位数
        /// </summary>
        public int Scale { get; }

        /// <summary>
        /// 列级序列化器
        /// </summary>
        public DecimalScaleColumnSerializer Serializer { get; }

        /// <summary>
        /// 小数转整数标签
        /// </summary>
        /// <param name="scale">小数点位数，如 2 表示 ×100</param>
        public DecimalScaleAttribute(int scale = 2)
        {
            Scale = scale;
            Serializer = new DecimalScaleColumnSerializer(scale);
        }

        /// <summary>
        /// 小数转整数标签
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="scale">小数点位数，如 2 表示 ×100</param>
        public DecimalScaleAttribute(string name, int scale = 2) : base(name)
        {
            Scale = scale;
            Serializer = new DecimalScaleColumnSerializer(scale);
        }
    }
}
