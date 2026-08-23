using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 提供字符串处理的扩展方法。
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 在驼峰命名字符串中的小写字母与大写字母、数字与字母之间插入空格。
        /// </summary>
        /// <param name="s">原始字符串。</param>
        /// <returns>拆分后的字符串。</returns>
        public static string BreakUpCamelCase(this string s)
        {
            var patterns = new[]
            {
                "([a-z])([A-Z])",
                "([0-9])([a-zA-Z])",
                "([a-zA-Z])([0-9])"
            };
            var output = patterns.Aggregate(s, (current, pattern) => Regex.Replace(current, pattern, "$1 $2", RegexOptions.IgnorePatternWhitespace));
            return output;
        }
    }
}
