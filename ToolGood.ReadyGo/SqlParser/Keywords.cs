using System;
using System.Collections.Generic;

namespace ToolGood.ReadyGo.SqlParser
{
    public class Keywords
    {
        private Dictionary<String, Token> keywords;

        public static Keywords DEFAULT_KEYWORDS = new Keywords();

        public Keywords()
        {
            keywords = new Dictionary<String, Token>();

            keywords.Add("ALL", Token.ALL);
            keywords.Add("ALTER", Token.ALTER);
            keywords.Add("AND", Token.AND);
            keywords.Add("ANY", Token.ANY);
            keywords.Add("AS", Token.AS);

            keywords.Add("ENABLE", Token.ENABLE);
            keywords.Add("DISABLE", Token.DISABLE);

            keywords.Add("ASC", Token.ASC);
            keywords.Add("BETWEEN", Token.BETWEEN);
            keywords.Add("BY", Token.BY);
            keywords.Add("CASE", Token.CASE);
            keywords.Add("CAST", Token.CAST);

            keywords.Add("CHECK", Token.CHECK);
            keywords.Add("CONSTRAINT", Token.CONSTRAINT);
            keywords.Add("CREATE", Token.CREATE);
            keywords.Add("DATABASE", Token.DATABASE);
            keywords.Add("DEFAULT", Token.DEFAULT);
            keywords.Add("COLUMN", Token.COLUMN);
            keywords.Add("TABLESPACE", Token.TABLESPACE);
            keywords.Add("PROCEDURE", Token.PROCEDURE);
            keywords.Add("FUNCTION", Token.FUNCTION);

            keywords.Add("DELETE", Token.DELETE);
            keywords.Add("DESC", Token.DESC);
            keywords.Add("DISTINCT", Token.DISTINCT);
            keywords.Add("DROP", Token.DROP);
            keywords.Add("ELSE", Token.ELSE);
            keywords.Add("EXPLAIN", Token.EXPLAIN);
            keywords.Add("EXCEPT", Token.EXCEPT);

            keywords.Add("END", Token.END);
            keywords.Add("ESCAPE", Token.ESCAPE);
            keywords.Add("EXISTS", Token.EXISTS);
            keywords.Add("FOR", Token.FOR);
            keywords.Add("FOREIGN", Token.FOREIGN);

            keywords.Add("FROM", Token.FROM);
            keywords.Add("FULL", Token.FULL);
            keywords.Add("GROUP", Token.GROUP);
            keywords.Add("HAVING", Token.HAVING);
            keywords.Add("IN", Token.IN);

            keywords.Add("INDEX", Token.INDEX);
            keywords.Add("INNER", Token.INNER);
            keywords.Add("INSERT", Token.INSERT);
            keywords.Add("INTERSECT", Token.INTERSECT);
            keywords.Add("INTERVAL", Token.INTERVAL);

            keywords.Add("INTO", Token.INTO);
            keywords.Add("IS", Token.IS);
            keywords.Add("JOIN", Token.JOIN);
            keywords.Add("KEY", Token.KEY);
            keywords.Add("LEFT", Token.LEFT);

            keywords.Add("LIKE", Token.LIKE);
            keywords.Add("LOCK", Token.LOCK);
            keywords.Add("MINUS", Token.MINUS);
            keywords.Add("NOT", Token.NOT);

            keywords.Add("NULL", Token.NULL);
            keywords.Add("ON", Token.ON);
            keywords.Add("OR", Token.OR);
            keywords.Add("ORDER", Token.ORDER);
            keywords.Add("OUTER", Token.OUTER);

            keywords.Add("PRIMARY", Token.PRIMARY);
            keywords.Add("REFERENCES", Token.REFERENCES);
            keywords.Add("RIGHT", Token.RIGHT);
            keywords.Add("SCHEMA", Token.SCHEMA);
            keywords.Add("SELECT", Token.SELECT);

            keywords.Add("SET", Token.SET);
            keywords.Add("SOME", Token.SOME);
            keywords.Add("TABLE", Token.TABLE);
            keywords.Add("THEN", Token.THEN);
            keywords.Add("TRUNCATE", Token.TRUNCATE);

            keywords.Add("UNION", Token.UNION);
            keywords.Add("UNIQUE", Token.UNIQUE);
            keywords.Add("UPDATE", Token.UPDATE);
            keywords.Add("VALUES", Token.VALUES);
            keywords.Add("VIEW", Token.VIEW);
            keywords.Add("SEQUENCE", Token.SEQUENCE);
            keywords.Add("TRIGGER", Token.TRIGGER);
            keywords.Add("USER", Token.USER);

            keywords.Add("WHEN", Token.WHEN);
            keywords.Add("WHERE", Token.WHERE);
            keywords.Add("XOR", Token.XOR);

            keywords.Add("OVER", Token.OVER);
            keywords.Add("TO", Token.TO);
            keywords.Add("USE", Token.USE);

            keywords.Add("REPLACE", Token.REPLACE);

            keywords.Add("COMMENT", Token.COMMENT);
            keywords.Add("COMPUTE", Token.COMPUTE);
            keywords.Add("WITH", Token.WITH);
            keywords.Add("GRANT", Token.GRANT);
            keywords.Add("REVOKE", Token.REVOKE);
        }

        public bool containsValue(Token token)
        {
            return this.keywords.ContainsValue(token);
        }

        public Keywords(Dictionary<String, Token> keywords)
        {
            this.keywords = keywords;
        }

        public Token? getKeyword(String key)
        {
            key = key.ToUpper();
            Token t;
            if (keywords.TryGetValue(key, out t)) {
                return t;
            }
            return null;
        }

        public Dictionary<String, Token> getKeywords()
        {
            return keywords;
        }
    }
}