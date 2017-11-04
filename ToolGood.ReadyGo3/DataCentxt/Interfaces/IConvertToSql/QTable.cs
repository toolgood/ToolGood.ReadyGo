using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;
using ToolGood.ReadyGo3.DataCentxt.Internals;

namespace ToolGood.ReadyGo3.DataCentxt.Interfaces
{
    public interface ITableConvert
    {
        string ToSql(DatabaseProvider provider, bool schemaName);


        // QTable
        string ToTableSql(DatabaseProvider provider, int tableCount, bool schemaName);

        string ToJoinSql(DatabaseProvider provider, int tableCount, bool schemaName);

        List<QTableColumn> GetUpdateColumns();

        List<QTableColumn> GetInsertColumns();

        QTableColumn GetPrimaryKey();

        bool IsSetPrimaryKey();

        /// <summary>
        /// 获取 update Set 关联的Column 
        /// 用于Oracle Update with Join
        /// </summary>
        /// <returns></returns>
        List<QTableColumn> GetUpdateSetRelationColumn();

        void SetDefaultValue(bool datetimeUsedNow, bool stringUsedEmpty, bool guidUsedNew);



    }
}
namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QTable : ITableConvert
    {
        string ITableConvert.ToSql(DatabaseProvider provider, bool schemaName)
        {
            if (schemaName && string.IsNullOrEmpty(_schemaName) == false) {
                return provider.EscapeSqlIdentifier(_schemaName) + "." + provider.EscapeSqlIdentifier(_tableName);
            }
            return provider.EscapeSqlIdentifier(_tableName);
        }


        string ITableConvert.ToTableSql(DatabaseProvider provider, int tableCount, bool schemaName)
        {
            if (tableCount > 1) {
                return ((ITableConvert)this).ToSql(provider, schemaName) + " AS " + _asName;
            }
            return ((ITableConvert)this).ToSql(provider, schemaName);
        }

        string ITableConvert.ToJoinSql(DatabaseProvider provider, int tableCount, bool schemaName)
        {
            if (_joinCondition == null) throw new NoJoinOnException();
            StringBuilder stringBuilder = new StringBuilder();
            if (_joinType == JoinType.Left) {
                stringBuilder.Append("LEFT ");
            } else if (_joinType == JoinType.Right) {
                stringBuilder.Append("RIGHT ");
            } else if (_joinType == JoinType.Full) {
                stringBuilder.Append("FULL ");
            }
            stringBuilder.Append("JOIN ");
            var table = ((ITableConvert)this).ToTableSql(provider, tableCount, schemaName);
            stringBuilder.Append(table);
            stringBuilder.Append(" ON ");
            ((IConditionConvert)_joinCondition).ToSql(stringBuilder, provider, tableCount);
            return stringBuilder.ToString();
        }

        List<QTableColumn> ITableConvert.GetUpdateColumns()
        {
            List<QTableColumn> list = new List<QTableColumn>();
            foreach (var item in _columns) {
                var col = item.Value;
                if (col._isResultColumn) continue;
                if (col._changeType != Enums.ColumnChangeType.None) {
                    if (col._isPrimaryKey == false) {
                        list.Add(col);
                    }
                }
            }
            return list;
        }

        List<QTableColumn> ITableConvert.GetInsertColumns()
        {
            List<QTableColumn> list = new List<QTableColumn>();
            foreach (var item in _columns) {
                var col = item.Value;
                if (col._isResultColumn) continue;
                list.Add(col);
            }
            return list;
        }

        QTableColumn ITableConvert.GetPrimaryKey()
        {
            foreach (var item in _columns) {
                var col = item.Value;
                if (col._isPrimaryKey) {
                    return col;
                }
            }
            return null;
        }

        bool ITableConvert.IsSetPrimaryKey()
        {
            foreach (var item in _columns) {
                var col = item.Value;
                if (col._isPrimaryKey) {
                    return col._changeType == Enums.ColumnChangeType.NewValue;
                }
            }
            throw new NoPrimaryKeyException();
        }

        void ITableConvert.SetDefaultValue(bool datetimeUsedNow, bool stringUsedEmpty, bool guidUsedNew)
        {
            foreach (var item in _columns) {
                var col = item.Value;
                if (col._changeType == Enums.ColumnChangeType.None) {
                    if (col._fieldType == "string") {
                        col.SetValue("");
                    } else if (col._fieldType == "datetime") {
                        col.SetValue(DateTime.Now);
                    } else if (col._fieldType == "guid") {
                        col.SetValue(Guid.NewGuid());
                    }
                }
            }
        }

        List<QTableColumn> ITableConvert.GetUpdateSetRelationColumn()
        {
            var cols = ((ITableConvert)this).GetUpdateColumns();
            List<QTableColumn> list = new List<QTableColumn>();

            foreach (var col in cols) {
                list.Add(col);
                if (col._changeType == Enums.ColumnChangeType.NewSql) {
                    var c = col.GetNewValue();
                    GetAllTableColumn(c, list);
                }
            }
            return list.Distinct().ToList();
        }

        private void GetAllTableColumn(QColumnBase column, List<QTableColumn> list)
        {
            if (column._columnType == Enums.ColumnType.Code) return;
            if (column._columnType == Enums.ColumnType.None) return;
            if (column._columnType == Enums.ColumnType.Value) return;
            if (column._columnType == Enums.ColumnType.Column) {
                list.Add((QTableColumn)column);
                return;
            }
            var args = column._functionArgs;
            if (args == null) return;

            foreach (var item in args) {
                if (item is QColumnBase) {
                    GetAllTableColumn((QColumnBase)item, list);
                }
            }
        }

    }
}
