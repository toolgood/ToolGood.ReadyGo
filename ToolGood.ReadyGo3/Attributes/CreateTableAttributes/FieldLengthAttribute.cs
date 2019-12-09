using System;

namespace ToolGood.ReadyGo3.Attributes
{
    /// <summary>
    /// 列长度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
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
    /// 文本 长度无限
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
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

    /// <summary>
    /// 手机长度 20位
    /// </summary>
    public class PhoneLengthAttribute : FieldLengthAttribute
    {
        /// <summary>
        /// 文本类型
        /// </summary>
        public PhoneLengthAttribute() : base(20)
        {
        }
    }

    /// <summary>
    /// 用户名长度 20位
    /// </summary>
    public class UserNameLengthAttribute: FieldLengthAttribute
    {
        public UserNameLengthAttribute() : base(20)
        {
        }
    }

    /// <summary>
    /// 密码长度 32位
    /// </summary>
    public class PasswrodLengthAttribute : FieldLengthAttribute
    {
        public PasswrodLengthAttribute() : base(32)
        {
        }
    }

    /// <summary>
    /// 注释长度 500位
    /// </summary>
    public class CommentLengthAttribute : FieldLengthAttribute
    {
        public CommentLengthAttribute() : base(500)
        {
        }
    }

    /// <summary>
    /// GUID长度 40位
    /// </summary>
    public class GuidLengthAttribute : FieldLengthAttribute
    {
        public GuidLengthAttribute() : base(40)
        {
        }
    }

    /// <summary>
    /// Url长度 200位
    /// </summary>
    public class UrlLengthAttribute : FieldLengthAttribute
    {
        public UrlLengthAttribute() : base(200)
        {
        }
    }

    /// <summary>
    /// 标题长度 100位
    /// </summary>
    public class TitleNameLengthAttribute : FieldLengthAttribute
    {
        public TitleNameLengthAttribute() : base(100)
        {
        }
    }

    /// <summary>
    /// 短名称 50位
    /// </summary>
    public class ShortNameLengthAttribute : FieldLengthAttribute
    {
        public ShortNameLengthAttribute() : base(50)
        {
        }
    }

    /// <summary>
    /// Ip地址长度 46位
    /// </summary>
    public class IpLengthAttribute : FieldLengthAttribute
    {
        public IpLengthAttribute() : base(46)
        {
        }
    }

    /// <summary>
    /// UserAgent长度 250位
    /// </summary>
    public class UserAgentLengthAttribute : FieldLengthAttribute
    {
        public UserAgentLengthAttribute() : base(250)
        {
        }
    }

    /// <summary>
    /// Email地址长度 50位
    /// </summary>
    public class EmailLengthAttribute : FieldLengthAttribute
    {
        public EmailLengthAttribute() : base(50)
        {
        }
    }

    /// <summary>
    /// 标签 长度 500位
    /// </summary>
    public class TagsLengthAttribute : FieldLengthAttribute
    {
        public TagsLengthAttribute() : base(500)
        {
        }
    }

    /// <summary>
    /// MAC 地址 18位
    /// </summary>
    public class MacAddressLengthAttribute : FieldLengthAttribute
    {
        public MacAddressLengthAttribute() : base(18)
        {
        }
    }

    /// <summary>
    /// 错误信息长度 200位
    /// </summary>
    public class ErrorMessageLengthAttribute : FieldLengthAttribute
    {
        public ErrorMessageLengthAttribute() : base(200)
        {
        }
    }


}