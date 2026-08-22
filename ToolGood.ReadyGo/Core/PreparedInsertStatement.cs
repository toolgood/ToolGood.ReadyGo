using System.Collections.Generic;

namespace ToolGood.ReadyGo.NPoco
{
    public class PreparedInsertStatement
    {
        public PocoData PocoData { get; set; }
        public string VersionName { get; set; }
        public string Sql { get; set; }
        public List<object> Rawvalues { get; set; }
    }
}