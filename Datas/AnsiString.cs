using System;

namespace ToolGood.ReadyGo
{
    /// <summary>
    ///     Wrap strings in an instance of this class to force use of DBType.AnsiString
    /// </summary>
    [Serializable]
    public class AnsiString
    {
        /// <summary>
        ///     The string value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Constructs an AnsiString
        /// </summary>
        /// <param name="str">The C# string to be converted to ANSI before being passed to the DB</param>
        public AnsiString(string str)
        {
            Value = str;
        }
        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }
        /// <summary>
        /// 转化 为string
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator string(AnsiString s)
        {
            return s.Value;
        }
        /// <summary>
        /// 转化 为AnsiString
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator AnsiString(string s)
        {
            return new AnsiString(s);
        }
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var b = obj as AnsiString;
            if (b != null) {
                return b.Value == this.Value;

            }
            return base.Equals(obj);
        }
        /// <summary>
        /// 获取 HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}