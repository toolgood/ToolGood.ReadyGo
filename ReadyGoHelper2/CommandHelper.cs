using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace ReadyGoHelper
{
    class CommandHelper
    {
        private static CommandHelper _Default;
        public static CommandHelper Default
        {
            get
            {
                if (_Default == null) {
                    _Default = new CommandHelper();
                }
                return _Default;
            }
        }
        private CommandHelper()
        {
            dte = ((EnvDTE.DTE)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE).GUID));
        }

        #region 私有变量
        private const string ReadyGoString = "using ToolGood.ReadyGo;\r\n";
        private const string ReadyGoRegex = @"using +ToolGood\.ReadyGo;";
        private const string CreateWhereString = ".CreateWhere";
        private const string CreateWhereRegex = @"\.CreateWhere<([^>]+)>\([^\)]*\)[^;]*?\.";
        //private const string t1_t1 = "t1=>";
        //private const string t1_t2 = "(t1,t2)=>";
        //private const string t1_t3 = "(t1,t2,t3)=>";
        //private const string t1_t4 = "(t1,t2,t3,t4)=>";
        //private const string t1_t5 = "(t1,t2,t3,t4,t5)=>";
        private List<string> methons = new List<string>() {
            "Where","OrderBy","GroupBy","Having","SelectColumn","On2","On3","On4","On5","SetValue",
            "Where(","OrderBy(","GroupBy(","Having(","SelectColumn(","On2(","On3(","On4(","On5(","SetValue(",
        };

        #endregion

        #region 常用方法
        private DTE dte { get; set; }
        private Project CurrentProject
        {
            get
            {
                EnvDTE.ProjectItem containingProjectItem = dte.Solution.FindProjectItem(dte.ActiveDocument.FullName);
                return containingProjectItem.ContainingProject;

            }
        }
        private TextSelection Selection
        {
            get
            {
                return dte.ActiveDocument.Selection;
            }
        }
        public bool IsLoadReadGo()
        {
            var project = CurrentProject;
            if (project.Name == "ToolGood.ReadyGo") return false;

            var codes = project.CodeModel.CodeElements;
            var code = ToList(codes).FirstOrDefault(q => q.Name == "ToolGood");
            if (code == null) return false;
            if (code.Kind != vsCMElement.vsCMElementNamespace) return false;
            codes = ((CodeNamespace)code).Members;
            code = ToList(codes).FirstOrDefault(q => q.Name == "ReadyGo");
            if (code == null) return false;
            if (code.Kind != vsCMElement.vsCMElementNamespace) return false;
            return true;
        }
        private List<CodeElement> ToList(CodeElements codes)
        {
            List<CodeElement> list = new List<CodeElement>();
            foreach (CodeElement code in codes) {
                list.Add(code);
            }
            return list;
        }
        private CodeFunction GetCodeFunction()
        {
            var codeFunction = Selection.ActivePoint.CodeElement[vsCMElement.vsCMElementFunction];
            if (codeFunction == null) {
                return null;
            }
            return codeFunction as CodeFunction;
        }
        private string GetFunString()
        {
            var fun = GetCodeFunction();
            return fun.StartPoint.CreateEditPoint().GetText((Selection.ActivePoint.AbsoluteCharOffset - fun.StartPoint.AbsoluteCharOffset));
        }
        private string GetDocText()
        {
            TextDocument doc = dte.ActiveDocument.Object("TextDocument");
            var length = doc.EndPoint.AbsoluteCharOffset;
            return doc.StartPoint.CreateEditPoint().GetText(length);
        }

        public bool TryGetFunString(out string text)
        {
            text = "";
            var codeFunction = Selection.ActivePoint.CodeElement[vsCMElement.vsCMElementFunction];
            if (codeFunction == null) {
                return false;
            }
            var fun = codeFunction as CodeFunction;
            text = fun.StartPoint.CreateEditPoint().GetText((Selection.ActivePoint.AbsoluteCharOffset - fun.StartPoint.AbsoluteCharOffset));

            return true;

        }
        #endregion

        public bool TryAddNamespace()
        {
            if (IsLoadReadGo() == false) return false;
            string text;
            if (TryGetFunString(out text) == false) {
                return false;
            }
            if (text.EndsWith(CreateWhereString)) {
                var docText = GetDocText();
                Regex r = new Regex(ReadyGoRegex);
                var m = r.Match(docText);
                if (m.Success == false) {
                    TextDocument doc = dte.ActiveDocument.Object("TextDocument");
                    doc.StartPoint.CreateEditPoint().Insert(ReadyGoString);
                    return true;
                }
            }
            return false;
        }

        public bool TryAddParameters()
        {
            if (IsLoadReadGo() == false) return false;
            string text;
            if (TryGetFunString(out text) == false) return false;
            var sp = text.Split('.').Last();
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
                    Selection.Text = "";
                    Selection.ActivePoint.CreateEditPoint().Insert(s);
                    if (hasbracket == false) {
                        Selection.CharLeft(false, 1);
                    }
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
                if (typenames[i].StartsWith("Db") || typenames[i].StartsWith("Vm") || typenames[i].StartsWith("Ds")) {
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
                var name = getSimpleName(names[i]);
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
