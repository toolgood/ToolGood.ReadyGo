using System;

namespace ToolGood.ReadyGo3.Attributes
{
    /// <summary>
    /// 列长度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property , AllowMultiple = false, Inherited = true)]
    public class FieldLengthAttribute : Attribute
    {
        /// <summary>
        /// 列长度
        /// </summary>
        protected FieldLengthAttribute() { IsText = true; }

        /// <summary>
        /// 最大长度
        /// </summary>
        /// <param name="length">长度</param>
        public FieldLengthAttribute(int length)
        {
            IsText = false;
            FieldLength = length.ToString();
        }

        /// <summary>
        /// 适用字段
        /// </summary>
        /// <param name="length"></param>
        /// <param name="pointLength"></param>
        public FieldLengthAttribute(int length, int pointLength)
        {
            IsText = false;
            FieldLength = length.ToString() + "," + pointLength.ToString();
        }

        /// <summary>
        /// 是否TEXT
        /// </summary>
        public bool IsText;

        /// <summary>
        ///
        /// </summary>
        public string FieldLength;
    }
    /// <summary>
    /// 文本
    /// </summary>
    [AttributeUsage(AttributeTargets.Property , AllowMultiple = false, Inherited = true)]
    public class TextAttribute : FieldLengthAttribute
    {
        /// <summary>
        /// 文本类型
        /// </summary>
        public TextAttribute() : base()
        {
            IsText = true;
        }
    }
}