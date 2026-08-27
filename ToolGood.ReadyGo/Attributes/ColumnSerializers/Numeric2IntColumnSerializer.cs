using System;
using System.Globalization;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 小数转整数列序列化器：保存时数值 × 10^scale 四舍五入存为 int（超出 int 范围抛异常），读取时 ÷ 10^scale 还原
    /// </summary>
    public class Numeric2IntColumnSerializer : NPoco.IColumnSerializer
    {
        private readonly int _scale;
        private readonly decimal _multiplier;

        /// <summary>
        /// 小数转整数列序列化器
        /// </summary>
        /// <param name="scale">小数点位数</param>
        public Numeric2IntColumnSerializer(int scale)
        {
            if (scale < 0) {
                throw new ArgumentOutOfRangeException(nameof(scale), "scale 不能为负数");
            }
            if (scale > 10) {
                throw new ArgumentOutOfRangeException(nameof(scale), "scale 不能超过 10");
            }
            _scale = scale;
            _multiplier = (decimal)Math.Pow(10, scale);
        }

        /// <summary>
        /// 数值 × 10^scale，四舍五入后保存为 int
        /// </summary>
        /// <param name="value">要序列化的小数值</param>
        /// <returns>四舍五入后的 int 值</returns>
        public object Serialize(object value)
        {
            if (value == null) {
                return null;
            }
            var d = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            var scaled = Math.Round(d * _multiplier, 0, MidpointRounding.AwayFromZero);
            // decimal → int 超出范围时会抛 OverflowException
            return (int)scaled;
        }

        /// <summary>
        /// 整数 ÷ 10^scale，四舍五入后还原小数
        /// </summary>
        /// <param name="value">数据库中的整数值</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>还原后的小数值</returns>
        public object Deserialize(object value, Type targetType)
        {
            var s = value as string ?? value?.ToString();
            if (string.IsNullOrEmpty(s)) {
                return null;
            }
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            var d = Math.Round(decimal.Parse(s, CultureInfo.InvariantCulture) / _multiplier, _scale, MidpointRounding.AwayFromZero);
            if (t == typeof(double)) {
                return (double)d;
            }
            if (t == typeof(float)) {
                return (float)d;
            }
            return d;
        }
    }
}
