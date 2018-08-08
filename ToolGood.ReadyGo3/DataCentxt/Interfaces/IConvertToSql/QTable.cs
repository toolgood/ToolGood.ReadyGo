using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;

namespace ToolGood.ReadyGo3.DataCentxt
{
    /// <summary>
    /// 
    /// </summary>
    partial class QTable
    {
        internal string ToSql(DatabaseProvider provider, bool schemaName)
        {
            var pd = PetaPoco.Core.PocoData.ForType(GetTableType());
            if (schemaName) {
                return provider.GetTableName(pd,GetSqlHelper()._tableNameManager);
            }
            return provider.GetMiniTableName(pd, GetSqlHelper()._tableNameManager);
        }


        internal string ToTableSql(DatabaseProvider provider, int tableCount, bool schemaName)
        {
            if (tableCount > 1) {
                return (this).ToSql(provider, schemaName) + " AS " + _asName;
            }
            return (this).ToSql(provider, schemaName);
        }

        internal string ToJoinSql(DatabaseProvider provider, int tableCount, bool schemaName)
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
            var table = (this).ToTableSql(provider, tableCount, schemaName);
            stringBuilder.Append(table);
            stringBuilder.Append(" ON ");
            (_joinCondition).ToSql(stringBuilder, provider, tableCount);
            return stringBuilder.ToString();
        }

        internal List<QTableColumn> GetUpdateColumns()
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

        internal List<QTableColumn> GetInsertColumns()
        {
            List<QTableColumn> list = new List<QTableColumn>();
            foreach (var item in _columns) {
                var col = item.Value;
                if (col._isResultColumn) continue;
                list.Add(col);
            }
            return list;
        }

        internal QTableColumn GetPrimaryKey()
        {
            return _primaryKey;
        }

        internal bool IsSetPrimaryKey()
        {
            if (object.Equals(null, _primaryKey) == false) {
                return _primaryKey._changeType == Enums.ColumnChangeType.NewValue;
            }
            throw new NoPrimaryKeyException();
        }

        internal void SetDefaultValue(bool datetimeUsedNow, bool stringUsedEmpty, bool guidUsedNew)
        {
            if (datetimeUsedNow || stringUsedEmpty || guidUsedNew) {
                foreach (var item in _columns) {
                    var col = item.Value;
                    if (col._changeType == Enums.ColumnChangeType.None)
                        if (col._fieldType == "string") {
                            if (stringUsedEmpty) col.SetValue("");
                        } else if (col._fieldType == "datetime") {
                            if (datetimeUsedNow) col.SetValue(DateTime.Now);
                        } else if (col._fieldType == "guid") {
                            if (guidUsedNew) col.SetValue(Guid.NewGuid());
                        }
                }
            }
        }

        internal List<QTableColumn> GetUpdateSetRelationColumn()
        {
            var cols = (this).GetUpdateColumns();
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

        private void GetAllTableColumn(QColumn column, List<QTableColumn> list)
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
                if (item is QColumn) {
                    GetAllTableColumn((QColumn)item, list);
                }
            }
        }

    }
}
