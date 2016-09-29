using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo;
using PetaTest;
using System.Text.RegularExpressions;

namespace ToolGood.ReadyGo.UnitTest.ReadyGoHelper2
{
    [TestFixture]
    public class ReadyGoHelper2_Test
    {
        [Test]
        public void Test()
        {
            var t = @"helper.CreateWhere<User1, UserPay, UserOnRouter>()
                 .On2((u, up) => u.AutoID == up.UserID)
                 .On3((u, up, uor) => u.AutoID == uor.UserID)
                 .Where((u, up, uor) => u.AutoID == 8)
                 .Where(";
            var s = "";
            TryAddParameters(t, out s);
            Assert.AreEqual("(u, up, uor) => ", s);
        }
        [Test]
        public void Test1()
        {
            var t = @"helper.CreateWhere<DbUser1, DbUserPay, DbUserOnRouter>()
                 .On2((u, up) => u.AutoID == up.UserID)
                 .On3((u, up, uor) => u.AutoID == uor.UserID)
                 .Where((u, up, uor) => u.AutoID == 8)
                 .Where(";
            var s = "";
            TryAddParameters(t, out s);
            Assert.AreEqual("(u, up, uor) => ", s);
        }
        [Test]
        public void Test2()
        {
            var t = @"helper.CreateWhere< DbUserPay ,  DbUser1 , DbUserOnRouter >()
                 .On2((u, up) => u.AutoID == up.UserID)
                 .On3((u, up, uor) => u.AutoID == uor.UserID)
                 .Where((u, up, uor) => u.AutoID == 8)
                 .Where(";
            var s = "";
            TryAddParameters(t, out s);
            Assert.AreEqual("(up, u, uor) => ", s);
        }
        [Test]
        public void Test4()
        {
            var t = @"helper.CreateWhere< DbUser2 ,  DbUser1 , DbUserOnRouter >()
                 .On2((u, up) => u.AutoID == up.UserID)
                 .On3((u, up, uor) => u.AutoID == uor.UserID)
                 .Where((u, up, uor) => u.AutoID == 8)
                 .Where(";
            var s = "";
            TryAddParameters(t, out s);
            Assert.AreEqual("(u, u2, uor) => ", s);
        }
        [Test]
        public void Test5()
        {
            var t = @"helper.CreateWhere< DbUserPay1 ,  DbUserPay2 , DbUserOnRouter >()
                 .On2((u, up) => u.AutoID == up.UserID)
                 .On3((u, up, uor) => u.AutoID == uor.UserID)
                 .Where((u, up, uor) => u.AutoID == 8)
                 .Where(";
            var s = "";
            TryAddParameters(t, out s);
            Assert.AreEqual("(up, up2, uor) => ", s);
        }
        [Test]
        public void Test6()
        {
            var t = @"helper.CreateWhere< DbUserPay1 ,  DbUserPay2 , DbDserOnRouter >()
                 .On2((u, up) => u.AutoID == up.UserID)
                 .On3((u, up, uor) => u.AutoID == uor.UserID)
                 .Where((u, up, uor) => u.AutoID == 8)
                 .Where(";
            var s = "";
            TryAddParameters(t, out s);
            Assert.AreEqual("(up, up2, d) => ", s);
        }
        [Test]
        public void Test7()
        {
            var t = @"helper.CreateWhere< DbUserPay1 ,DbUser ,   DbUserPay2 , DbDserOnRouter >()
                 .On2((u, up) => u.AutoID == up.UserID)
                 .On3((u, up, uor) => u.AutoID == uor.UserID)
                 .Where((u, up, uor) => u.AutoID == 8)
                 .Where(";
            var s = "";
            TryAddParameters(t, out s);
            Assert.AreEqual("(up, u, up2, d) => ", s);
        }



        #region 私有变量
        private const string ReadyGoString = "using ToolGood.ReadyGo;\r\n";
        private const string ReadyGoRegex = @"using +ToolGood\.ReadyGo;";
        private const string CreateWhereString = ".CreateWhere";
        private const string CreateWhereRegex = @"\.CreateWhere<([^>]+)>\([^\)]*\)[^;]*?\.";
        private List<string> methons = new List<string>() {
            "Where","OrderBy","GroupBy","Having","SelectColumn","On2","On3","On4","On5","SetValue",
            "Where(","OrderBy(","GroupBy(","Having(","SelectColumn(","On2(","On3(","On4(","On5(","SetValue(",
        };

        #endregion


        public bool TryAddParameters( string text,out string ps)
        {
            var sp = text.Split('.').Last();
            ps = "";
            if (methons.Contains(sp)) {
                var r = new Regex(CreateWhereRegex + sp.Replace("(", @"\(") + "$");
                var m = r.Match(text);
                if (m.Success) {
                    var typeNames = m.Groups[1].Value;
                    var typeCount = typeNames.Split(',').Length;
                    var hasbracket = sp.EndsWith("(");
                    string s = "";
                    if (hasbracket == false) {
                        s += "(";
                    }
                    if (sp.StartsWith("On")) {
                        typeCount = int.Parse(sp.Replace("On", "").Replace("(", ""));
                    }
                    if (sp == "SetValue" || sp == "SetValue(") {
                        typeCount = 1;
                    }
                    s += getsql(typeNames, typeCount);

                    if (hasbracket == false) {
                        s += ")";
                    }
                    ps = s;
                    return true;
                }
            }
            return false;

        }
        private string getsql(string types, int typeCount)
        {
            var typenames = types.Split(',').ToList();
            for (int i = 0; i < typenames.Count; i++) {
                typenames[i] = typenames[i].Trim();
                if (typenames[i].StartsWith("Db") || typenames[i].StartsWith("Vm")) {
                    typenames[i] = typenames[i].Substring(2);
                }
            }

            var dict = getSimpleName(typenames);
            var list = getMinName(dict, typenames);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < typeCount; i++) {
                if (sb.Length > 0) {
                    sb.Append(", ");
                }
                sb.Append(list[i]);
            }
            if (typeCount > 0) {
                sb.Insert(0, "(");
                sb.Append(")");
            }
            sb.Append(" => ");
            return sb.ToString();
        }
        private Dictionary<string, string> getSimpleName(List<string> names)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            for (int i = 0; i < names.Count; i++) {
                var name = getSimpleName( names[i]);
                list[names[i]] = name;
            }
            return list;
        }
        private string getSimpleName(string name)
        {
            var t = Regex.Replace(name, "[^A-Z]", "");
            if (string.IsNullOrEmpty(t) == false) {
                return t.ToLower();
            } else if (string.IsNullOrEmpty(name) == false) {
                return name.ToLower();
            } else {
                return name.ToLower();
            }
        }

        private List<string> getMinName(Dictionary<string, string> dict, List<string> names)
        {
            NameTree root = new NameTree();
            foreach (var item in dict) {
                root.Load(item.Key, item.Value);
            }
            root.OrderBy();

            //var names2 = names.OrderBy(q => q).ToList();

            List<string> list = new List<string>();
            foreach (var item in names) {
                var name = root.GetName(item);
                if (list.Contains(name) == false) {
                    list.Add(name);
                    continue;
                }
                name = getSimpleName(item);
                if (list.Contains(name) == false) {
                    list.Add(name);
                    continue;
                }
                for (int i = 2; i < 10; i++) {
                    var n = name + i;
                    if (list.Contains(n) == false) {
                        list.Add(n);
                        break;
                    }
                }
            }
            return list;

            //List<string> list2 = new List<string>();
            //foreach (var item in names) {
            //    var t = names2.IndexOf(item);
            //    list2.Add(list[t]);
            //}


            //return list2;
        }

    }


    public class NameTree
    {
        public List<string> Keys { get; set; }
        public List<string> CurKeys { get; set; }
        public List<string> NextKeys { get; set; }
        public string Name { get; set; }
        public char Char { get; set; }
        //public bool IsNum { get; set; }
        public List<NameTree> Nodes { get; set; }
        public NameTree()
        {
            Keys = new List<string>();
            CurKeys = new List<string>();
            NextKeys = new List<string>();
            Name = "";
            Nodes = new List<NameTree>();
        }

        public void Load(string key, string name)
        {
            Keys.Add(key);
            var ch = name[0];
            var node = Nodes.FirstOrDefault(q => q.Char == ch);
            if (node == null) {
                node = new NameTree();
                node.Char = ch;
                //node.IsNum = "0123456789".Contains(ch);
                node.Name = this.Name + ch;
                this.Nodes.Add(node);
            }
            if (name.Length == 1) {
                node.CurKeys.Add(key);
            } else {
                var next = name.Substring(1);
                node.NextKeys.Add(key);
                node.Load(key, next);
            }
        }

        public void OrderBy()
        {
            CurKeys = CurKeys.Distinct().OrderBy(q => q).ToList();
            NextKeys = NextKeys.Distinct().OrderBy(q => q).ToList();
            foreach (var node in Nodes) {
                node.OrderBy();
            }
        }

        public string GetName(string key)
        {
            //当 当前值 
            if (CurKeys.Contains(key)) {
                return Name;
            }
            if (Nodes.Count == 1 && Name.Length == 1 && Keys.Count == 1) {
                return Name;
            }
            foreach (var node in Nodes) {
                var t = node.GetName(key);
                if (t != null) {
                    return t;
                }
            }
            return null;
        }


    }
}
