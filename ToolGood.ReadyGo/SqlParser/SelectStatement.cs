using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo.SqlParser.Tags;

namespace ToolGood.ReadyGo.SqlParser
{
    public class SelectStatement
    {
        public SelectStatement()
        {
            Invalid = new InvalidTag();
            Tables = new List<TableTag>();
            Joins = new List<JoinTag>();
        }
        private InvalidTag Invalid { get; set; }

        public SelectTag Select { get; set; }


        public List<TableTag> Tables { get; set; }


        public FromTag From { get; set; }

        public List<JoinTag> Joins { get; set; }

        public WhereTag Where { get; set; }

        public OrderByTag OrderBy { get; set; }

        public GroupByTag GroupBy { get; set; }

        public HavingTag Having { get; set; }

        public LimitOffsetTag LimitOffset { get; set; }


        public static SelectStatement Load(List<TextPoint> points)
        {
            SelectStatement ss = new SelectStatement();
            points.RemoveAll(q => q.Token == Token.Comment);
            ss.InitSql(points);
            return ss;
        }
        internal void InitSql(List<TextPoint> points)
        {
            BaseTag lostTag = this.Invalid;
            #region 分割表达式
            for (int i = 0; i < points.Count; i++) {
                var item = points[i];
                if (item.Token== Token.Semicolon) {
                    break;
                }
                var tag = GetNextTag(item);
                if (tag is SelectTag) {
                    this.Select = tag;
                } else if (tag is FromTag) {
                    this.From = tag;
                } else if (tag is JoinTag) {
                    this.Joins.Add(tag);
                    lostTag = tag;
                    lostTag.SubPoints.Add(item);
                    if (item.Token != Token.JOIN) {
                        lostTag.SubPoints.Add(points[i + 1]);
                        i++;
                        if (points[i + 1].Token != Token.JOIN) {
                            lostTag.SubPoints.Add(points[i + 2]);
                            i++;
                        }
                    }
                    continue;
                } else if (tag is WhereTag) {
                    this.Where = tag;
                } else if (tag is OrderByTag) {
                    this.OrderBy = tag;
                } else if (tag is GroupByTag) {
                    this.GroupBy = tag;
                } else if (tag is HavingTag) {
                    this.Having = tag;
                } else if (tag is LimitOffsetTag) {
                    this.LimitOffset = tag;
                }
                lostTag = tag;
                lostTag.SubPoints.Add(item);
            } 
            #endregion

        }



        private dynamic GetNextTag(TextPoint point)
        {
            if (point.Token == Token.SELECT) {
                return new SelectTag();
            }
            if (point.Token == Token.FROM) {
                return new FromTag();
            }
            if (point.Token == Token.JOIN || point.Token == Token.FULL || point.Token == Token.LEFT || point.Token == Token.INNER || point.Token == Token.RIGHT) {
                return new JoinTag();
            }
            if (point.Token == Token.WHERE) {
                return new WhereTag();
            }
            if (point.Token == Token.ORDER) {
                return new OrderByTag();
            }
            if (point.Token == Token.GROUP) {
                return new GroupByTag();
            }
            if (point.Token == Token.HAVING) {
                return new HavingTag();
            }
            if (point.Token == Token.OFFSET) {
                return new LimitOffsetTag();
            }
            if (point.Token == Token.LIMIT) {
                return new LimitOffsetTag();
            }
            return null;
        }

    }
}
