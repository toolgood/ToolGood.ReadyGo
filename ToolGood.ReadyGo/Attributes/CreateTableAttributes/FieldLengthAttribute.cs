using System;

namespace ToolGood.ReadyGo.Attributes
{
    /// <summary>
    /// 文本列类型
    /// </summary>
    public enum FieldTextType
    {
        /// <summary>
        /// 非文本（普通长度列）
        /// </summary>
        None = 0,

        /// <summary>
        /// 文本
        /// </summary>
        Text = 1,

        /// <summary>
        /// 中等文本
        /// </summary>
        MediumText = 2,

        /// <summary>
        /// 长文本
        /// </summary>
        LongText = 3,
    }

    /// <summary>
    /// 列长度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldLengthAttribute : Attribute
    {
        /// <summary>
        /// 文本列长度（供 Text/MediumText/LongText 使用）
        /// </summary>
        protected FieldLengthAttribute()
        {
            TextType = FieldTextType.Text;
        }

        /// <summary>
        /// 最大长度
        /// </summary>
        /// <param name="length">长度</param>
        public FieldLengthAttribute(int length)
        {
            if (length <= 0) {
                throw new ArgumentOutOfRangeException(nameof(length), "length 必须大于 0");
            }
            Length = length;
        }

        /// <summary>
        /// 适用字段
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="pointLength">小数位数</param>
        public FieldLengthAttribute(int length, int pointLength)
        {
            if (length <= 0) {
                throw new ArgumentOutOfRangeException(nameof(length), "length 必须大于 0");
            }
            if (pointLength < 0) {
                throw new ArgumentOutOfRangeException(nameof(pointLength), "pointLength 不能为负数");
            }
            Length = length;
            PointLength = pointLength;
        }

        /// <summary>
        /// 列长度（未设置时为 0）
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 小数位数（仅 decimal 适用，未设置时为 null）
        /// </summary>
        public int? PointLength { get; }

        /// <summary>
        /// 文本列类型
        /// </summary>
        public FieldTextType TextType { get; protected set; }

        /// <summary>
        /// 是否为文本列
        /// </summary>
        public bool IsText => TextType != FieldTextType.None;

        /// <summary>
        /// 是否为中等文本列
        /// </summary>
        public bool IsMediumText => TextType == FieldTextType.MediumText || TextType == FieldTextType.LongText;

        /// <summary>
        /// 是否为长文本列
        /// </summary>
        public bool IsLongText => TextType == FieldTextType.LongText;

        /// <summary>
        /// 字段长度定义（长度，或 长度,小数位数）
        /// </summary>
        public string FieldLength => Length == 0
            ? null
            : PointLength.HasValue ? $"{Length},{PointLength}" : Length.ToString();
    }

    /// <summary>
    /// 文本
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class TextAttribute : FieldLengthAttribute
    {
        /// <summary>
        /// 文本类型
        /// </summary>
        public TextAttribute() : base()
        {
            TextType = FieldTextType.Text;
        }
    }

    /// <summary>
    /// 中等文本
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class MediumTextAttribute : FieldLengthAttribute
    {
        /// <summary>
        /// 中等文本类型
        /// </summary>
        public MediumTextAttribute() : base()
        {
            TextType = FieldTextType.MediumText;
        }
    }

    /// <summary>
    /// 长文本
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class LongTextAttribute : FieldLengthAttribute
    {
        /// <summary>
        /// 长文本类型
        /// </summary>
        public LongTextAttribute() : base()
        {
            TextType = FieldTextType.LongText;
        }
    }
}
