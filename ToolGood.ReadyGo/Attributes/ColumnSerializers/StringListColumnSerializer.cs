using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ToolGood.ReadyGo.Attributes.ColumnSerializers
{
    /// <summary>
    /// 字符串列表列序列化器：将 List&lt;string&gt; / string[] 序列化为以分隔符连接的文本列，
    /// 读取时按分隔符还原。支持转义（反斜杠与分隔符前加 \），可存储含分隔符的项。
    /// </summary>
    public class StringListColumnSerializer : NPoco.IColumnSerializer
    {
        private readonly string _separator;

        /// <summary>
        /// 字符串列表列序列化器
        /// </summary>
        /// <param name="separator">分隔符，默认逗号</param>
        public StringListColumnSerializer(string separator = ",")
        {
            if (string.IsNullOrEmpty(separator)) {
                throw new ArgumentException("separator 不能为空", nameof(separator));
            }
            _separator = separator;
        }

        /// <summary>
        /// 序列化为分隔符文本
        /// </summary>
        /// <param name="value">字符串列表（List&lt;string&gt; / string[]）</param>
        /// <returns>分隔符文本，null 输入返回 null</returns>
        public object Serialize(object value)
        {
            switch (value) {
                case null:
                    return null;
                case IList list:
                    if (list.Count == 0) {
                        return "";
                    }
                    var sb = new StringBuilder();
                    for (int i = 0; i < list.Count; i++) {
                        if (i > 0) {
                            sb.Append(_separator);
                        }
                        sb.Append(Escape(list[i]?.ToString() ?? ""));
                    }
                    return sb.ToString();
                default:
                    throw new NotSupportedException($"StringList 不支持的类型：{value.GetType().Name}");
            }
        }

        /// <summary>
        /// 从分隔符文本反序列化
        /// </summary>
        /// <param name="value">分隔符文本</param>
        /// <param name="targetType">目标类型（List&lt;string&gt; / string[]）</param>
        /// <returns>还原的字符串列表</returns>
        public object Deserialize(object value, Type targetType)
        {
            if (value == null) {
                return null;
            }
            var s = value as string ?? value?.ToString();
            var isArray = targetType == typeof(string[]);
            if (!isArray && targetType != typeof(List<string>)) {
                throw new NotSupportedException($"StringList 目标类型仅支持 List<string> 或 string[]：{targetType.Name}");
            }
            if (string.IsNullOrEmpty(s)) {
                return isArray ? Array.Empty<string>() : new List<string>();
            }
            // 单遍扫描：同时处理转义（\ 与 \分隔符）与分隔符切分，避免先切分再反转义破坏转义序列
            var items = new List<string>();
            var sb = new StringBuilder();
            var escaped = false;
            for (int i = 0; i < s.Length; i++) {
                var c = s[i];
                if (escaped) {
                    sb.Append(c);
                    escaped = false;
                } else if (c == '\\') {
                    escaped = true;
                } else if (IsSeparatorAt(s, i)) {
                    items.Add(sb.ToString());
                    sb.Clear();
                    i += _separator.Length - 1;
                } else {
                    sb.Append(c);
                }
            }
            if (escaped) {
                sb.Append('\\'); // 结尾孤立反斜杠，宽容保留
            }
            items.Add(sb.ToString());

            if (isArray) {
                return items.ToArray();
            }
            return items;
        }

        private string Escape(string item)
        {
            return item.Replace("\\", "\\\\").Replace(_separator, "\\" + _separator);
        }

        private bool IsSeparatorAt(string s, int i)
        {
            if (_separator.Length == 1) {
                return s[i] == _separator[0];
            }
            return i + _separator.Length <= s.Length
                && s.AsSpan(i, _separator.Length).SequenceEqual(_separator);
        }
    }
}
