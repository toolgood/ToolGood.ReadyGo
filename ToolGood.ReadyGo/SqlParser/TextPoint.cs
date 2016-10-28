//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ToolGood.ReadyGo.SqlParser.Exceptions;

//namespace ToolGood.ReadyGo.SqlParser
//{
//    public class TextPoint
//    {
//        #region 静态数据

//        private static bool[] identifierFlags = new bool[256];

//        private static bool isIdentifierChar(char c)
//        {
//            if (c <= identifierFlags.Length) {
//                return identifierFlags[c];
//            }
//            return c != '　' && c != '，';
//        }

//        static TextPoint()
//        {
//            for (char c = (char)0; c < identifierFlags.Length; ++c) {
//                if (c >= 'A' && c <= 'Z') {
//                    identifierFlags[c] = true;
//                } else if (c >= 'a' && c <= 'z') {
//                    identifierFlags[c] = true;
//                } else if (c >= '0' && c <= '9') {
//                    identifierFlags[c] = true;
//                }
//            }
//            // identifierFlags['`'] = true;
//            identifierFlags['_'] = true;
//            identifierFlags['.'] = true;
//            identifierFlags['$'] = true;
//            identifierFlags['#'] = true;
//            //identifierFlags['['] = true;
//            //identifierFlags[']'] = true;
//            //identifierFlags['`'] = true;
//        }

//        #endregion 静态数据

//        public override string ToString()
//        {
//            return string.Format("[{0}]{1}", Token, Text);
//        }

//        private char[] TextChars;
//        public int Start { get; internal set; }
//        public int Length { get; internal set; }
//        public int End { get { return Start + Length; } }
//        private string _str;
//        public string Text { get { return GetString(); } }
//        public char Char { get { return TextChars[Start]; } }
//        public List<TextPoint> SubTextPoint = new List<TextPoint>();

//        private string GetString()
//        {
//            if (_str == null || _str.Length != Length) {
//                char[] t = new char[Length];
//                Array.Copy(TextChars, Start, t, 0, Length);
//                StringBuilder sb = new StringBuilder();
//                for (int i = 0; i < Length; i++) {
//                    sb.Append(TextChars[i + Start]);
//                }
//                _str = sb.ToString();
//            }
//            return _str;
//        }

//        public Token Token = Token.Unknown;

//        public static List<TextPoint> Analysis(string str)
//        {
//            str = str.Trim();
//            var cs = str.ToArray();
//            int start = 0;
//            var list = Analysis(ref cs, ref start, str.Length);
//            SetToken(list);
//            return list;
//        }

//        private static void SetToken(List<TextPoint> list)
//        {
//            foreach (var item in list) {
//                if (item.Token == Token.Unknown) {
//                    var t = Keywords.DEFAULT_KEYWORDS.getKeyword(item.Text);
//                    if (t != null) {
//                        item.Token = t.Value;
//                    }
//                }
//                SetToken(item.SubTextPoint);
//            }
//        }

//        private static List<TextPoint> Analysis(ref char[] cs, ref int start, int end, int layer = 0)
//        {
//            List<TextPoint> list = new List<TextPoint>();
//            char c, nextChar;
//            while (start < end) {
//                c = cs[start];
//                switch (c) {

//                    #region 读取分号  点号 逗号 括号 Identifier 及参数

//                    case ';':
//                        list.Add(ReadSemicolon(ref cs, ref start, end));
//                        break;

//                    case '.':
//                        nextChar = cs[start + 1];
//                        if (nextChar == '.') {
//                            if (cs[start + 2] == '.') {
//                                list.Add(ReadChars(ref cs, ref start, 3, Token.String));
//                            } else {
//                                list.Add(ReadChars(ref cs, ref start, 2, Token.String));
//                            }
//                        } else {
//                            list.Add(ReadDot(ref cs, ref start, end));
//                        }
//                        break;

//                    case ',':
//                    case '，':
//                        list.Add(ReadComma(ref cs, ref start, end));
//                        break;

//                    case '`':
//                        list.Add(ReadIdentifier(ref cs, '`', ref start, end));
//                        break;

//                    case '[':
//                        list.Add(ReadIdentifier(ref cs, ']', ref start, end));
//                        break;

//                    case '{':
//                        list.Add(ReadParameters(ref cs, '}', ref start, end));
//                        break;

//                    case '@':
//                        list.Add(ReadParameters2(ref cs, ref start, end));
//                        break;

//                    case '$':
//                        nextChar = cs[start + 1];
//                        if (nextChar == '{') {
//                            list.Add(ReadParameters3(ref cs, ref start, end));
//                            break;
//                        }
//                        throw new ParserException("It's error, right bracket is more.");
//                    case ':':
//                        nextChar = cs[start + 1];
//                        if (nextChar == ':' || nextChar == '=') {
//                            list.Add(ReadOperator(ref cs, ref start, end));
//                        } else {
//                            list.Add(ReadParameters2(ref cs, ref start, end));
//                        }
//                        break;

//                    #region 括号解析

//                    case '(':
//                        list.Add(ReadBracket(ref cs, ref start, end, layer));
//                        break;

//                    case ')':
//                        if (layer == 0) {
//                            throw new ParserException("It's error, right bracket is more.");
//                        }
//                        start++;
//                        return list;

//                    #endregion 括号解析

//                    #endregion 读取分号  点号 逗号 括号 Identifier 及参数

//                    #region 处理数字

//                    case '0':
//                        if (cs[start + 1] == 'x') {
//                            list.Add(ReadHexNumber(ref cs, ref start, end));
//                        } else {
//                            list.Add(ReadNumber(ref cs, ref start, end));
//                        }
//                        break;

//                    case '1':
//                    case '2':
//                    case '3':
//                    case '4':
//                    case '5':
//                    case '6':
//                    case '7':
//                    case '8':
//                    case '9':
//                        list.Add(ReadNumber(ref cs, ref start, end));
//                        break;

//                    #endregion 处理数字

//                    #region 注释 及 部分算法

//                    case '-':
//                        nextChar = cs[start + 1];
//                        if (nextChar == '-') {
//                            list.Add(ReadLineComment(ref cs, ref start, end));
//                        } else if (nextChar >= '0' && nextChar <= '9') {
//                            var lost = list.Last();
//                            if (lost != null && lost.Token != Token.Number) {
//                                list.Add(ReadNumber(ref cs, ref start, end));
//                            } else {
//                                list.Add(ReadChars(ref cs, ref start, 1, Token.Operator));
//                            }
//                        } else {
//                            list.Add(ReadChars(ref cs, ref start, 1, Token.Operator));
//                        }
//                        break;

//                    case '#':
//                        list.Add(ReadLineComment(ref cs, ref start, end));
//                        break;

//                    case '/':
//                        nextChar = cs[start + 1];
//                        if (nextChar == '/') {
//                            list.Add(ReadLineComment(ref cs, ref start, end));
//                        } else if (nextChar == '*') {
//                            list.Add(ReadRangeComment(ref cs, ref start, end));
//                        } else {
//                            list.Add(ReadOperator(ref cs, ref start, end));
//                        }
//                        break;

//                    #endregion 注释 及 部分算法

//                    #region 分割符 部分算法

//                    case '*':
//                        var t = list.LastOrDefault();
//                        if (t == null && layer == 0) {
//                            throw new ParserException("It's error,this start with * .");
//                        } else if (t != null && t.Token == Token.Number) {
//                            list.Add(ReadChars(ref cs, ref start, 1, Token.Operator));
//                        } else {
//                            list.Add(ReadChars(ref cs, ref start, 1, Token.Identifier));
//                        }
//                        break;

//                    case '?':
//                    case '+':
//                    case '^':
//                    case '~':
//                    case '%':

//                        list.Add(ReadChars(ref cs, ref start, 1, Token.Operator));
//                        break;

//                    case '=':
//                    case '>':
//                    case '<':
//                    case '!':
//                    case '&':
//                    case '|':
//                        list.Add(ReadOperator(ref cs, ref start, end));
//                        break;

//                    #endregion 分割符 部分算法

//                    #region 读取字符串

//                    case 'N':
//                        if (start < end && cs[start + 1] == '\'') {
//                            list.Add(ReadNString(ref cs, ref start, end));
//                            break;
//                        }
//                        list.Add(ReadString(ref cs, ref start, end, layer + 1));
//                        break;

//                    case '\'':
//                        list.Add(ReadStringToEnd(ref cs, c, ref start, end));
//                        break;

//                    case '"':
//                        list.Add(ReadStringToEnd(ref cs, c, ref start, end));
//                        break;

//                    #endregion 读取字符串

//                    default:
//                        if (!isIdentifierChar(c)) {
//                            start++;
//                            break;
//                        }
//                        list.Add(ReadString(ref cs, ref start, end, layer + 1));
//                        break;
//                }
//            }
//            return list;
//        }

//        #region 读取分号  点号 逗号 括号 Identifier 及参数

//        private static TextPoint ReadChars(ref char[] cs, ref int start, int length, Token token)
//        {
//            TextPoint point = new TextPoint();
//            point.Token = token;
//            point.Start = start;
//            point.TextChars = cs;
//            point.Length = length;
//            start = start + length;
//            return point;
//        }

//        /// <summary>
//        /// 读取分号
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadSemicolon(ref char[] cs, ref int start, int end)
//        {
//            return ReadChars(ref cs, ref start, 1, Token.Semicolon);
//        }

//        /// <summary>
//        /// 读取点号
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadDot(ref char[] cs, ref int start, int end)
//        {
//            return ReadChars(ref cs, ref start, 1, Token.Dot);
//        }

//        /// <summary>
//        /// 读取逗号
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadComma(ref char[] cs, ref int start, int end)
//        {
//            return ReadChars(ref cs, ref start, 1, Token.Comma);
//        }

//        /// <summary>
//        /// 读取括号
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadBracket(ref char[] cs, ref int start, int end, int layer)
//        {
//            TextPoint point = new TextPoint();
//            point.Token = Token.Bracket;
//            point.Start = start;
//            point.TextChars = cs;
//            start++;
//            var list = Analysis(ref cs, ref start, end, layer + 1);
//            point.SubTextPoint = list;
//            point.Length = start - point.Start;
//            return point;
//        }

//        /// <summary>
//        /// 适用 [{ `
//        /// 返回类型
//        /// Identifier
//        /// Parameters
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="endChar"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadIdentifier(ref char[] cs, char endChar, ref int start, int end)
//        {
//            TextPoint point = new TextPoint();
//            point.Token = Token.Identifier;
//            point.Start = start;
//            point.TextChars = cs;
//            var pos = start + 1;

//            while (pos < end) {
//                var c = cs[pos];
//                if (c == endChar) {
//                    start = pos + 1;
//                    point.Length = start - point.Start;
//                    return point;
//                }
//                pos++;
//            }
//            throw new ParserException("Can't find char \"" + endChar + "\"");
//        }

//        private static TextPoint ReadParameters(ref char[] cs, char endChar, ref int start, int end)
//        {
//            TextPoint point = new TextPoint();
//            point.Token = Token.Parameters;
//            point.Start = start;
//            point.TextChars = cs;
//            var pos = start + 1;

//            while (pos < end) {
//                var c = cs[pos];
//                if (c == endChar) {
//                    start = pos + 1;
//                    point.Length = start - point.Start;
//                    return point;
//                }
//                pos++;
//            }
//            throw new ParserException("Can't find char \"" + endChar + "\"");
//        }

//        private static TextPoint ReadParameters2(ref char[] cs, ref int start, int end)
//        {
//            TextPoint point = new TextPoint();
//            point.Token = Token.Parameters;
//            point.Start = start;
//            point.TextChars = cs;
//            start++;

//            while (start < end) {
//                var c = cs[start];
//                if (!isIdentifierChar(c)) {
//                    point.Length = start - point.Start;
//                    return point;
//                }
//                start++;
//            }
//            start = end;
//            point.Length = start - point.Start;
//            return point;
//        }

//        private static TextPoint ReadParameters3(ref char[] cs, ref int start, int end)
//        {
//            TextPoint point = new TextPoint();
//            point.Token = Token.Parameters;
//            point.Start = start;
//            point.TextChars = cs;
//            var pos = start + 2;

//            while (pos < end) {
//                var c = cs[pos];
//                if (c == '}') {
//                    start = pos + 1;
//                    point.Length = start - point.Start;
//                    return point;
//                }
//                pos++;
//            }
//            throw new ParserException("Can't find char \"}\"");
//        }

//        #endregion 读取分号  点号 逗号 括号 Identifier 及参数

//        /// <summary>
//        /// 读取操作
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadOperator(ref char[] cs, ref int start, int end)
//        {
//            var ch = cs[start];
//            char nextChar = cs[start + 1];
//            switch (ch) {
//                case '>':
//                    if (nextChar == '=' || nextChar == '>')
//                        return ReadChars(ref cs, ref start, 2, Token.Operator);
//                    return ReadChars(ref cs, ref start, 1, Token.Operator);

//                case '<':
//                    if (nextChar == '=' && cs[start + 2] == '>')
//                        return ReadChars(ref cs, ref start, 3, Token.Operator);
//                    if (nextChar == '<' || nextChar == '=' || nextChar == '>')
//                        return ReadChars(ref cs, ref start, 2, Token.Operator);
//                    return ReadChars(ref cs, ref start, 1, Token.Operator);

//                case '=':
//                    if (nextChar == '=')
//                        return ReadChars(ref cs, ref start, 3, Token.Operator);
//                    return ReadChars(ref cs, ref start, 1, Token.Operator);

//                case '!':
//                    if (nextChar == '<' || nextChar == '=' || nextChar == '>' || nextChar == '!')
//                        return ReadChars(ref cs, ref start, 2, Token.Operator);
//                    return ReadChars(ref cs, ref start, 1, Token.Operator);

//                case '|':
//                    if (nextChar == '|' && cs[start + 2] == '/')
//                        return ReadChars(ref cs, ref start, 3, Token.Operator);
//                    if (nextChar == '|' || nextChar == '/')
//                        return ReadChars(ref cs, ref start, 2, Token.Operator);
//                    return ReadChars(ref cs, ref start, 1, Token.Operator);

//                case '&':
//                    if (nextChar == '&')
//                        return ReadChars(ref cs, ref start, 2, Token.Operator);
//                    return ReadChars(ref cs, ref start, 1, Token.Operator);

//                default:
//                    break;
//            }
//            throw new ParserException("It's error,this has error Operator.");
//        }

//        #region 读取注释

//        /// <summary>
//        /// 读取一行注释
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadRangeComment(ref char[] cs, ref int start, int end)
//        {
//            TextPoint point = new TextPoint();
//            point.Token = Token.Comment;
//            point.Start = start;
//            point.TextChars = cs;
//            var pos = start + 2;
//            while (pos <= end - 1) {
//                var c = cs[pos];
//                if (c == '*' && cs[pos + 1] == '/') {
//                    start = pos + 2;
//                    point.Length = start - point.Start;
//                    return point;
//                }
//                pos++;
//            }
//            throw new ParserException("It's error,this has no */ .");
//        }

//        /// <summary>
//        /// 读取一行注释
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadLineComment(ref char[] cs, ref int start, int end)
//        {
//            TextPoint point = new TextPoint();
//            point.Token = Token.Comment;
//            point.Start = start;
//            point.TextChars = cs;
//            var pos = start + 1;
//            while (pos < end) {
//                var c = cs[pos];
//                if (c == '\r' || c == '\n') {
//                    if (cs[pos + 1] == '\r' || cs[pos + 1] == '\n') {
//                        start = pos + 2;
//                        point.Length = start - point.Start;
//                        return point;
//                    } else {
//                        start = pos + 1;
//                        point.Length = start - point.Start;
//                        return point;
//                    }
//                }
//                pos++;
//            }
//            start = end;
//            point.Length = start - point.Start;
//            return point;
//        }

//        #endregion 读取注释

//        #region 读取数字

//        /// <summary>
//        /// 读取HEX数字
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadHexNumber(ref char[] cs, ref int start, int end)
//        {
//            TextPoint point = new TextPoint();
//            point.Token = Token.Number;
//            point.Start = start;
//            point.TextChars = cs;
//            var pos = start + 2;
//            string t = "abcdefABCDEF1234567890";
//            while (pos < end) {
//                var c = cs[pos];
//                if (t.Contains(c) == false) {
//                    if (isIdentifierChar(c)) {
//                        throw new ParserException("It's error,this is not hex number.");
//                    }

//                    start = pos;
//                    point.Length = pos - point.Start;
//                    return point;
//                }
//                pos++;
//            }
//            start = end;
//            point.Length = start - point.Start;
//            return point;
//        }

//        /// <summary>
//        /// 读取数字
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadNumber(ref char[] cs, ref int start, int end)
//        {
//            TextPoint point = new TextPoint();
//            point.Token = Token.Number;
//            point.Start = start;
//            point.TextChars = cs;
//            var pos = start + 1;
//            var hasE = false;

//            var str = "0123456789.";
//            while (pos < end) {
//                var c = cs[pos];
//                if (hasE == false && (c == 'e' || c == 'E')) {
//                    if (cs[pos + 1] == '+' || cs[pos + 1] == '-') {
//                        pos++;
//                    } else if (str.Contains(cs[pos + 1]) == false) {
//                        throw new ParserException("It's error,this is not hex number.");
//                    }
//                    pos++;
//                    continue;
//                }
//                if (str.Contains(c) == false) {
//                    if (isIdentifierChar(c)) {
//                        throw new ParserException("It's error,this is not hex number.");
//                    }
//                    start = pos;
//                    point.Length = pos - point.Start;
//                    return point;
//                }
//                pos++;
//            }
//            start = end;
//            point.Length = start - point.Start;

//            return point;
//        }

//        #endregion 读取数字

//        #region 读取字符串

//        /// <summary>
//        /// 字符串专用
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="endChar"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadStringToEnd(ref char[] cs, char endChar, ref int start, int end)
//        {
//            TextPoint point = new TextPoint();
//            point.Token = Token.String;
//            point.Start = start;
//            point.TextChars = cs;
//            var pos = start + 1;

//            while (pos < end) {
//                var c = cs[pos];
//                if (c == endChar) {
//                    if (pos < end && cs[pos + 1] == c) {
//                    } else {
//                        start = pos + 1;
//                        point.Length = start - point.Start;
//                        return point;
//                    }
//                }
//                if (c == '\\' && cs[pos + 1] == c) {
//                    pos++;
//                }
//                pos++;
//            }
//            throw new ParserException("Can't find char \"" + endChar + "\"");
//        }

//        /// <summary>
//        /// N字符串专用
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="endChar"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadNString(ref char[] cs, ref int start, int end)
//        {
//            TextPoint point = new TextPoint();
//            point.Start = start;
//            point.TextChars = cs;
//            point.Token = Token.String;
//            var pos = start + 2;

//            while (pos < end) {
//                var c = cs[pos];
//                if (c == '\'') {
//                    if (pos < end && cs[pos + 1] == c) {
//                    } else {
//                        start = pos + 1;
//                        point.Length = start - point.Start;
//                        return point;
//                    }
//                }
//                pos++;
//            }
//            throw new ParserException("Can't find char \"'\"");
//        }

//        #endregion 读取字符串

//        /// <summary>
//        /// N字符串专用 并判断是否为函数
//        /// 返回类型
//        /// Unknown
//        /// Identifier
//        /// Function
//        /// </summary>
//        /// <param name="cs"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        private static TextPoint ReadString(ref char[] cs, ref int start, int end, int layer)
//        {
//            TextPoint point = new TextPoint();
//            point.Token = Token.Unknown;
//            point.Start = start;
//            point.TextChars = cs;
//            start++;

//            while (start < end) {
//                var c = cs[start];
//                if (c == '.') {
//                    point.Token = Token.Identifier;
//                    if (!isIdentifierChar(cs[start + 1])) {
//                        point.Length = start - point.Start;
//                        return point;
//                    }
//                } else if (c == '(') {
//                    start++;
//                    point.Token = Token.Function;
//                    point.SubTextPoint = Analysis(ref cs, ref start, end, layer + 1);
//                    point.Length = start - point.Start;
//                    return point;
//                } else if (!isIdentifierChar(c)) {
//                    point.Length = start - point.Start;
//                    return point;
//                }
//                start++;
//            }
//            start = end;
//            point.Length = start - point.Start;
//            return point;
//        }
//    }
//}