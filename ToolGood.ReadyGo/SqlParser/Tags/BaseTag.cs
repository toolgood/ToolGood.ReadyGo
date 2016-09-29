using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser
{
    public abstract class BaseTag
    {
        internal List<TextPoint> SubPoints = new List<TextPoint>();

        public string ToSql()
        {
            StringBuilder sb = new StringBuilder();
            ToSql(sb);
            return sb.ToString().Trim();
        }
        internal abstract void ToSql(StringBuilder sb);
    }
}
