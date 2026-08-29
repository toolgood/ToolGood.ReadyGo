using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using ToolGood.ReadyGo.JsonDiffPatch;

namespace ToolGood.ReadyGo
{
    /// <summary>
    /// 数据变动转成文本 帮助类
    /// </summary>
    public static class DataDiffHelper
    {
        private const string UnchangedText = "未修改";
        private const string AddedText = "新增";
        private const string DeletedText = "删除";
        private const string NameSeparator = "：";
        private const string Pipe = "|";
        private const string Arrow = "->";
        private const string EqualsSign = "=";
        private const string EmptyJsonObject = "{}";

        /// <summary>
        /// 数据变动转成文本（新增）
        /// </summary>
        /// <param name="right">新数据</param>
        /// <returns></returns>
        public static string Diff<T>(T right) where T : class
        {
            DataDiffTypeInfo typeInfo = new DataDiffTypeInfo(typeof(T));
            return typeInfo.DiffMessage(right);
        }

        /// <summary>
        /// 数据变动转成文本（新增）
        /// </summary>
        /// <param name="right">新数据</param>
        /// <param name="sqlHelper"></param>
        /// <returns></returns>
        public static string Diff<T>(T right, SqlHelper sqlHelper) where T : class
        {
            DataDiffTypeInfo typeInfo = new DataDiffTypeInfo(typeof(T));
            typeInfo.SetEnumNameFromDatabase(sqlHelper);
            return typeInfo.DiffMessage(right);
        }

        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="left">原数据</param>
        /// <param name="right">新数据</param>
        /// <returns></returns>
        public static string Diff<T>(T left, T right) where T : class
        {
            if (left is null && right is null) { return ""; }
            if (left is null) { return Diff(right); }

            DataDiffTypeInfo typeInfo = new DataDiffTypeInfo(typeof(T));
            if (right is null) { return typeInfo.DeleteMessage(left); }
            return typeInfo.DiffMessage(left, right);
        }

        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="left">原数据</param>
        /// <param name="right">新数据</param>
        /// <param name="sqlHelper"></param>
        /// <returns></returns>
        public static string Diff<T>(T left, T right, SqlHelper sqlHelper) where T : class
        {
            if (left is null && right is null) { return ""; }
            if (left is null) { return Diff(right, sqlHelper); }

            DataDiffTypeInfo typeInfo = new DataDiffTypeInfo(typeof(T));
            typeInfo.SetEnumNameFromDatabase(sqlHelper);
            if (right is null) { return typeInfo.DeleteMessage(left); }
            return typeInfo.DiffMessage(left, right);
        }

        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lefts">原数据</param>
        /// <param name="rights"></param>
        /// <returns></returns>
        public static string Diff(string name, List<string> lefts, List<string> rights)
        {
            lefts = new List<string>(lefts);
            rights = new List<string>(rights);
            lefts.RemoveAll(x => string.IsNullOrEmpty(x));
            rights.RemoveAll(x => string.IsNullOrEmpty(x));
            var removes = lefts.Except(rights).ToList();
            var adds = rights.Except(lefts).ToList();

            if (removes.Count == 0 && adds.Count == 0) { return ""; }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(name);
            stringBuilder.Append(NameSeparator);
            stringBuilder.AppendJoin(Pipe, lefts);
            stringBuilder.Append(Arrow);
            stringBuilder.AppendJoin(Pipe, rights);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lefts">原数据</param>
        /// <param name="rights">新数据</param>
        /// <returns></returns>
        public static string Diff<T>(string name, List<T> lefts, List<T> rights) where T : struct
        {
            var removes = lefts.Except(rights).ToList();
            var adds = rights.Except(lefts).ToList();

            if (removes.Count == 0 && adds.Count == 0) { return ""; }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(name);
            stringBuilder.Append(NameSeparator);
            stringBuilder.AppendJoin(Pipe, lefts);
            stringBuilder.Append(Arrow);
            stringBuilder.AppendJoin(Pipe, rights);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lefts">原数据</param>
        /// <param name="rights">新数据</param>
        /// <param name="dict">字典</param>
        /// <returns></returns>
        public static string Diff<T>(string name, List<T> lefts, List<T> rights, Dictionary<T, string> dict) where T : struct
        {
            var removes = lefts.Except(rights).ToList();
            var adds = rights.Except(lefts).ToList();
            if (removes.Count == 0 && adds.Count == 0) { return ""; }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(name);
            stringBuilder.Append(NameSeparator);
            AppendItems(stringBuilder, lefts, dict);
            stringBuilder.Append(Arrow);
            AppendItems(stringBuilder, rights, dict);
            return stringBuilder.ToString();
        }

        private static void AppendItems<T>(StringBuilder stringBuilder, List<T> items, Dictionary<T, string> dict) where T : struct
        {
            for (int i = 0; i < items.Count; i++) {
                if (i > 0) { stringBuilder.Append(Pipe); }
                stringBuilder.Append(items[i]);
                if (dict.TryGetValue(items[i], out string name)) {
                    if (string.IsNullOrEmpty(name) == false) {
                        stringBuilder.Append(EqualsSign);
                        stringBuilder.Append(name);
                    }
                }
            }
        }

        /// <summary>
        /// 数据变动转成文本
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lefts">原数据</param>
        /// <param name="rights">新数据</param>
        /// <param name="func"></param>
        /// <param name="dict">字典</param>
        /// <returns></returns>
        public static string Diff<T, T1>(string name, List<T> lefts, List<T> rights, Func<T, T1> func, Dictionary<T1, string> dict)
            where T : class
            where T1 : struct
        {
            var left = new List<T1>(lefts.Count);
            foreach (var item in lefts) {
                left.Add(func(item));
            }
            var right = new List<T1>(rights.Count);
            foreach (var item in rights) {
                right.Add(func(item));
            }
            return Diff(name, left, right, dict);
        }

        /// <summary>
        /// json格式 差异
        /// </summary>
        /// <param name="left">原数据</param>
        /// <param name="right">新数据</param>
        /// <returns></returns>
        public static string JsonDiff(string left, string right)
        {
            if (string.Equals(left, right)) { return UnchangedText; }
            if (string.IsNullOrWhiteSpace(left)) { return AddedText + right; }
            if (string.IsNullOrWhiteSpace(right)) { return DeletedText + left; }

            var j1 = JsonNode.Parse(left);
            var j2 = JsonNode.Parse(right);

            var diff = j1.Diff(j2);
            if (diff == null) { return EmptyJsonObject; }
            return diff.ToString();
        }
    }
}
