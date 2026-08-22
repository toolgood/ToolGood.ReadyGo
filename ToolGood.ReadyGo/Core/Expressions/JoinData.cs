using System.Collections.Generic;

namespace ToolGood.ReadyGo.NPoco.Expressions
{
    public class JoinData
    {
        public string OnSql { get; set; }
        public PocoMember PocoMember { get; set; }
        public PocoMember PocoMemberJoin { get; set; }
        public List<PocoMember> PocoMembers { get; set; }
        public JoinType JoinType { get; set; }
        public string Hint { get; set; }
    }
}
