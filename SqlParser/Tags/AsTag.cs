using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.SqlParser.Tags
{
    public class AsTag : BaseTag
    {
        public AsTag() { }
        public AsTag(string name)
        {
            this.AsName = name;
        }

        public string AsName { get; set; }



        internal override void ToSql(StringBuilder sb)
        {
            sb.AppendFormat(" AS {0}", AsName);
        }
    }
}
