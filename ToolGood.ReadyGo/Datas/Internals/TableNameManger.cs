using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo.Poco;

namespace ToolGood.ReadyGo.Internals
{
    /// <summary>
    /// 表名修饰管理
    /// </summary>
    public class TableNameManger : IDisposable
    {
        /// <summary>
        /// 表名修饰类
        /// </summary>
        public class TableFix
        {
            /// <summary>
            /// 标记名
            /// </summary>
            public string TagName;
            /// <summary>
            /// 表名前缀
            /// </summary>
            public string TablePrefix;
            /// <summary>
            /// 表名后缀
            /// </summary>
            public string TableSuffix;
            /// <summary>
            /// ToString
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("[{0},{1},{2}]", TagName, TablePrefix, TableSuffix);
            }
        }

        private Dictionary<string, TableFix> dict = new Dictionary<string, TableFix>();

        /// <summary>
        /// 根据名称获取 TableName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TableFix Get(string name)
        {
            if (name == null) return null;
            TableFix tag;
            if (dict.TryGetValue(name, out tag)) {
                return tag;
            }
            return null;
        }

        /// <summary>
        /// 根据名称消除TableName
        /// </summary>
        /// <param name="name"></param>
        public void Clear(string name)
        {
            dict.Remove(name);
        }

        /// <summary>
        /// 判断是否存在TableName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Has(string name)
        {
            return dict.ContainsKey(name);
        }

        /// <summary>
        /// 根据名称设置TableName
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tablePrefix"></param>
        /// <param name="tableSuffix"></param>
        public void Set(string name, string tablePrefix, string tableSuffix)
        {
            dict[name] = new TableFix() {
                TablePrefix = tablePrefix,
                TableSuffix = tableSuffix,
                TagName = name
            };
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dict.OrderBy(q => q.Key)) {
                sb.Append(item.ToString());
            }
            return sb.ToString();
        }

        internal TableFix Get<T>()
        {
            var tag = PocoData.ForType(typeof(T)).TableInfo.FixTag;
            return Get(tag);
        }

        /// <summary>
        /// 释放内存
        /// </summary>
        public void Dispose()
        {
            dict.Clear();
            dict = null;
        }
    }
}