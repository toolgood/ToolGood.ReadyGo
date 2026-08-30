using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ToolGood.ReadyGo.NPoco.Internal;

namespace ToolGood.ReadyGo.NPoco
{
    public partial class Database
    {
        /// <summary>
        /// 插入语句的构建与主键、版本号赋值的帮助类。
        /// </summary>
        public class InsertStatements
        {
            /// <summary>
            /// 根据 POCO 元数据构建插入语句。
            /// </summary>
            /// <typeparam name="T">POCO 类型。</typeparam>
            /// <param name="database">当前数据库实例。</param>
            /// <param name="pd">POCO 元数据。</param>
            /// <param name="tableName">表名。</param>
            /// <param name="primaryKeyName">主键列名。</param>
            /// <param name="autoIncrement">主键是否自增。</param>
            /// <param name="poco">待插入的对象。</param>
            /// <returns>准备好的插入语句。</returns>
            public static PreparedInsertStatement PrepareInsertSql<T>(Database database, PocoData pd, string tableName, string primaryKeyName, bool autoIncrement, T poco)
            {
                var names = new List<string>();
                var values = new List<string>();
                var rawvalues = new List<object>();
                var index = 0;
                var versionName = "";

                foreach (var pocoColumn in pd.Columns.Values)
                {
                    // Don't insert result columns
                    if (pocoColumn.ResultColumn
                        || (pocoColumn.ComputedColumn && (pocoColumn.ComputedColumnType == ComputedColumnType.Always || pocoColumn.ComputedColumnType == ComputedColumnType.ComputedOnInsert))
                        || (pocoColumn.VersionColumn && pocoColumn.VersionColumnType == VersionColumnType.RowVersion))
                    {
                        continue;
                    }

                    // Don't insert the primary key (except under oracle where we need bring in the next sequence value)
                    if (autoIncrement && primaryKeyName != null && string.Compare(pocoColumn.ColumnName, primaryKeyName, true) == 0)
                    {
                        // Setup auto increment expression
                        string autoIncExpression = database.DatabaseType.GetAutoIncrementExpression(pd.TableInfo);
                        if (autoIncExpression != null)
                        {
                            names.Add(pocoColumn.ColumnName);
                            values.Add(autoIncExpression);
                        }
                        continue;
                    }

                    names.Add(database.DatabaseType.EscapeSqlIdentifier(pocoColumn.ColumnName));
                    values.Add(string.Format("@{0}", index++));

                    object val;
                    PocoColumn valueColumn;
                    if (pocoColumn.ReferenceType == ReferenceType.Foreign)
                    {
                        var member = pd.Members.Single(x => x.MemberInfoData == pocoColumn.MemberInfoData);
                        var column = member.PocoMemberChildren.Single(x => x.Name == member.ReferenceMemberName);
                        valueColumn = column.PocoColumn;
                        val = database.ProcessMapper(valueColumn, valueColumn.GetValue(poco));
                    }
                    else
                    {
                        valueColumn = pocoColumn;
                        val = database.ProcessMapper(pocoColumn, pocoColumn.GetValue(poco));
                    }

                    if (pocoColumn.VersionColumn && pocoColumn.VersionColumnType == VersionColumnType.Number)
                    {
                        val = Convert.ToInt64(val) > 0 ? val : 1;
                        versionName = pocoColumn.ColumnName;
                    }

                    rawvalues.Add(ParameterHelper.WrapNullWithDbType(database.DatabaseType, valueColumn, val));
                }

                var sql = string.Empty;
                var outputClause = String.Empty;
                if (autoIncrement || !string.IsNullOrEmpty(pd.TableInfo.SequenceName))
                {
                    outputClause = database.DatabaseType.GetInsertOutputClause(primaryKeyName, pd.TableInfo.UseOutputClause);
                }

                if (names.Count != 0)
                {
                    sql = string.Format("INSERT INTO {0} ({1}){2} VALUES ({3})",
                        database.DatabaseType.EscapeTableName(tableName),
                        string.Join(",", names.ToArray()),
                        outputClause,
                        string.Join(",", values.ToArray()));
                }
                else
                {
                    sql = database.DatabaseType.GetDefaultInsertSql(tableName, primaryKeyName, pd.TableInfo.UseOutputClause, names.ToArray(), values.ToArray());
                }

                var prep = new PreparedInsertStatement()
                {
                    PocoData = pd,
                    Sql = sql,
                    Rawvalues = rawvalues,
                    VersionName = versionName
                };

                foreach (var item in pd.TableInfo.AlterStatementHooks)
                {
                    prep = item.AlterInsert(database, prep);
                }

                return prep;
            }

            /// <summary>
            /// 获取非自增主键的当前值。
            /// </summary>
            /// <typeparam name="T">POCO 类型。</typeparam>
            /// <param name="primaryKeyName">主键列名。</param>
            /// <param name="poco">POCO 对象。</param>
            /// <param name="preparedSql">准备好的插入语句。</param>
            /// <returns>主键值，若不存在则返回 null。</returns>
            public static object AssignNonIncrementPrimaryKey<T>(string primaryKeyName, T poco, PreparedInsertStatement preparedSql)
            {
                PocoColumn pkColumn;
                if (primaryKeyName != null && preparedSql.PocoData.Columns.TryGetValue(primaryKeyName, out pkColumn))
                    return pkColumn.GetValue(poco);
                return null;
            }

            /// <summary>
            /// 为对象分配版本号。
            /// </summary>
            /// <typeparam name="T">POCO 类型。</typeparam>
            /// <param name="poco">POCO 对象。</param>
            /// <param name="preparedSql">准备好的插入语句。</param>
            public static void AssignVersion<T>(T poco, PreparedInsertStatement preparedSql)
            {
                if (!string.IsNullOrEmpty(preparedSql.VersionName))
                {
                    PocoColumn pc;
                    if (preparedSql.PocoData.Columns.TryGetValue(preparedSql.VersionName, out pc))
                    {
                        pc.SetValue(poco, pc.ChangeType(1));
                    }
                }
            }

            /// <summary>
            /// 为对象分配主键值。
            /// </summary>
            /// <typeparam name="T">POCO 类型。</typeparam>
            /// <param name="primaryKeyName">主键列名。</param>
            /// <param name="poco">POCO 对象。</param>
            /// <param name="id">主键值。</param>
            /// <param name="preparedSql">准备好的插入语句。</param>
            public static void AssignPrimaryKey<T>(string primaryKeyName, T poco, object id, PreparedInsertStatement preparedSql)
            {
                if (primaryKeyName != null && id != null && id.GetType().GetTypeInfo().IsValueType)
                {
                    PocoColumn pc;
                    if (preparedSql.PocoData.Columns.TryGetValue(primaryKeyName, out pc))
                    {
                        pc.SetValue(poco, pc.ChangeType(id));
                    }
                }
            }
        }
    }
}
