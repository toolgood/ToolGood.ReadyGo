using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlBuilding
{
    public class SqlSearch
    {
        public bool HasSelect { get; private set; }
        public bool HasFrom { get; private set; }
        public bool HasT1 { get; private set; }

        public static SqlSearch Search(string sql)
        {
            SqlSearch ss = new SqlSearch();
            sql = sql.Trim();
            if (sql.Length < 6) return ss;

            if (sql.Substring(0, 6).TrimEnd().ToLower() == "select") {
                ss.HasSelect = true;
                return ss;
            }
            if (sql.Substring(0, 5).TrimEnd().ToLower() == "from") {
                ss.HasFrom = true;
            }
            bool isInText = false;
            var t = 'a';
            bool isStart = false;
            string t1 = "";
            for (int i = 0; i < sql.Length - 3; i++) {
                var c = sql[i];
                if (isInText) {
                    if (c == t) isInText = false;
                } else if ("'\"`".Contains(c)) {
                    isInText = true;
                    isStart = false;
                    t = c;
                } else if (" \t\r\n(".Contains(c)) {
                    isStart = true;
                    t1 = "";
                } else if (isStart) {
                    t1 += c;
                    if (t1.Length == 3) {
                        if (t1 == "t1.") {
                            ss.HasT1 = true;
                            return ss;
                        } else {
                            isStart = false;
                        }
                    }
                }
            }
            return ss;
        }

    }
}
