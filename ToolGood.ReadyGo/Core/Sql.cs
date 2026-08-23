using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 以链式方式构建 SQL 语句的辅助类。
    /// </summary>
    public class Sql
    {
        /// <summary>
        /// 指示是否复用参数。
        /// </summary>
        public bool ReuseParameters { get; set; }

        /// <summary>
        /// 初始化 Sql 类的新实例。
        /// </summary>
        public Sql()
        { }

        /// <summary>
        /// 使用指定 SQL 片段与参数初始化 Sql 类的新实例。
        /// </summary>
        /// <param name="sql">SQL 片段。</param>
        /// <param name="args">参数。</param>
        public Sql(string sql, params object[] args)
        {
            _sql = sql;
            _args = args;
        }

        /// <summary>
        /// 使用指定 SQL 片段与参数初始化 Sql 类的新实例，并可选地立即完成构建。
        /// </summary>
        /// <param name="isBuilt">是否立即构建。</param>
        /// <param name="sql">SQL 片段。</param>
        /// <param name="args">参数。</param>
        public Sql(bool isBuilt, string sql, params object[] args)
        {
            _sql = sql;
            _args = args;

            if (!isBuilt) return;

            _sqlFinal = _sql;
            _argsFinal = _args;
        }

        /// <summary>
        /// 获取一个新的 Sql 构建器实例。
        /// </summary>
        public static Sql Builder => new Sql();

        string _sql;
        object[] _args;
        Sql _rhs;
        string _sqlFinal;
        object[] _argsFinal;

        private void Build()
        {
            // already built?
            if (_sqlFinal != null)
                return;

            // Build it
            var sb = new StringBuilder();
            var args = new List<object>();
            Build(sb, args, null);
            _sqlFinal = sb.ToString();
            _argsFinal = args.ToArray();
        }

        /// <summary>
        /// 最终构建完成的 SQL 语句。
        /// </summary>
        public string SQL
        {
            get
            {
                Build();
                return _sqlFinal;
            }
        }

        /// <summary>
        /// 最终构建完成的参数数组。
        /// </summary>
        public object[] Arguments
        {
            get
            {
                Build();
                return _argsFinal;
            }
        }

        /// <summary>
        /// 在末尾追加一个 Sql 片段。
        /// </summary>
        /// <param name="sql">要追加的 Sql 片段。</param>
        /// <returns>当前实例。</returns>
        public Sql Append(Sql sql)
        {
            _sqlFinal = null;

            if (_rhs != null)
            {
                _rhs.Append(sql);
            }
            else if (_sql != null)
            {
                _rhs = sql;
            }
            else
            {
                _sql = sql._sql;
                _args = sql._args;
                _rhs = sql._rhs;
            }

            return this;
        }

        /// <summary>
        /// 在指定 Sql 片段前添加分隔符并追加到末尾。
        /// </summary>
        /// <param name="sql">要追加的 Sql 片段。</param>
        /// <param name="delimiter">分隔符。</param>
        /// <returns>当前实例。</returns>
        public Sql Concat(Sql sql, string delimiter)
        {
            sql._sql = delimiter + sql._sql;
            return Append(sql);
        }

        /// <summary>
        /// 在末尾追加一段 SQL 文本与参数。
        /// </summary>
        /// <param name="sql">SQL 文本。</param>
        /// <param name="args">参数。</param>
        /// <returns>当前实例。</returns>
        public Sql Append(string sql, params object[] args)
        {
            Append(new Sql(sql, args));
            return this;
        }

        static bool Is(Sql sql, string sqltype)
        {
            return sql?._sql != null && sql._sql.StartsWith(sqltype, StringComparison.OrdinalIgnoreCase);
        }

        private void Build(StringBuilder sb, List<object> args, Sql lhs)
        {
            if (!string.IsNullOrEmpty(_sql))
            {
                // add SQL to the string
                if (sb.Length > 0)
                    sb.Append("\n");

                var sql = ParameterHelper.ProcessParams(_sql, _args, args, ReuseParameters);

                if (Is(lhs, "WHERE ") && Is(this, "WHERE "))
                    sql = "AND " + sql.Substring(6);
                if (Is(lhs, "ORDER BY ") && Is(this, "ORDER BY "))
                    sql = ", " + sql.Substring(9);

                sb.Append(sql);
            }

            // now do rhs
            _rhs?.Build(sb, args, this);
        }

        /// <summary>
        /// 追加 WHERE 子句。
        /// </summary>
        /// <param name="sql">WHERE 条件。</param>
        /// <param name="args">参数。</param>
        /// <returns>当前实例。</returns>
        public Sql Where(string sql, params object[] args)
        {
            Append("WHERE (" + sql + ")", args);
            return this;
        }

        /// <summary>
        /// 追加 ORDER BY 子句。
        /// </summary>
        /// <param name="columns">排序的列。</param>
        /// <returns>当前实例。</returns>
        public Sql OrderBy(params object[] columns)
        {
            Append("ORDER BY " + string.Join(", ", columns));
            return this;
        }

        /// <summary>
        /// 追加 SELECT 子句。
        /// </summary>
        /// <param name="columns">要查询的列。</param>
        /// <returns>当前实例。</returns>
        public Sql Select(params object[] columns)
        {
            Append("SELECT " + string.Join(", ", columns));
            return this;
        }

        /// <summary>
        /// 追加 FROM 子句。
        /// </summary>
        /// <param name="tables">要查询的表。</param>
        /// <returns>当前实例。</returns>
        public Sql From(params object[] tables)
        {
            Append("FROM " + string.Join(", ", tables));
            return this;
        }

        /// <summary>
        /// 追加 GROUP BY 子句。
        /// </summary>
        /// <param name="columns">分组的列。</param>
        /// <returns>当前实例。</returns>
        public Sql GroupBy(params object[] columns)
        {
            Append("GROUP BY " + string.Join(", ", columns));
            return this;
        }

        private SqlJoinClause Join(string joinType, string table)
        {
            Append(joinType + table);
            return new SqlJoinClause(this);
        }

        /// <summary>
        /// 追加 INNER JOIN 子句。
        /// </summary>
        /// <param name="table">要连接的表。</param>
        /// <returns>连接子句对象。</returns>
        public SqlJoinClause InnerJoin(string table) { return Join("INNER JOIN ", table); }
        /// <summary>
        /// 追加 LEFT JOIN 子句。
        /// </summary>
        /// <param name="table">要连接的表。</param>
        /// <returns>连接子句对象。</returns>
        public SqlJoinClause LeftJoin(string table) { return Join("LEFT JOIN ", table); }
        /// <summary>
        /// 追加 RIGHT JOIN 子句。
        /// </summary>
        /// <param name="table">要连接的表。</param>
        /// <returns>连接子句对象。</returns>
        public SqlJoinClause RightJoin(string table) { return Join("RIGHT JOIN ", table); }

        /// <summary>
        /// 表示 JOIN 之后的连接条件子句。
        /// </summary>
        public class SqlJoinClause
        {
            private readonly Sql _sql;

            /// <summary>
            /// 初始化 SqlJoinClause 类的新实例。
            /// </summary>
            /// <param name="sql">所属的 Sql 实例。</param>
            public SqlJoinClause(Sql sql)
            {
                _sql = sql;
            }

            /// <summary>
            /// 追加 ON 连接条件并返回所属的 Sql 实例。
            /// </summary>
            /// <param name="onClause">连接条件。</param>
            /// <param name="args">参数。</param>
            /// <returns>所属的 Sql 实例。</returns>
            public Sql On(string onClause, params object[] args)
            {
                _sql.Append("ON " + onClause, args);
                return _sql;
            }
        }
    }
}
