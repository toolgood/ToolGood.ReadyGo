using System;
using System.Globalization;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 小数转整数列序列化器：保存时数值 × 10^scale 存为整数，读取时 ÷ 10^scale 并四舍五入
    /// </summary>
    public class DecimalScaleColumnSerializer : ToolGood.ReadyGo.NPoco.IColumnSerializer
    {
        private readonly int _scale;
        private readonly decimal _multiplier;

        /// <summary>
        /// 小数转整数列序列化器
        /// </summary>
        /// <param name="scale">小数点位数</param>
        public DecimalScaleColumnSerializer(int scale)
        {
            if (scale < 0) {
                throw new ArgumentOutOfRangeException(nameof(scale), "scale 不能为负数");
            }
            _scale = scale;
            _multiplier = (decimal)Math.Pow(10, scale);
        }

        /// <summary>
        /// 数值 × 10^scale，四舍五入后保存为整数
        /// </summary>
        public string Serialize(object value)
        {
            if (value == null) {
                return null;
            }
            var d = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            var scaled = Math.Round(d * _multiplier, 0, MidpointRounding.AwayFromZero);
            return scaled.ToString("0", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 整数 ÷ 10^scale，四舍五入后还原小数
        /// </summary>
        public object Deserialize(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value)) {
                return null;
            }
            var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
            var d = Math.Round(decimal.Parse(value, CultureInfo.InvariantCulture) / _multiplier, _scale, MidpointRounding.AwayFromZero);
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
