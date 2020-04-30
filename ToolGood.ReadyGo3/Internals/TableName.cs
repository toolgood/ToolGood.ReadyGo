using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3.Internals
{
    /// <summary>
    /// 表名动态类
    /// </summary>
    public class TableName : DynamicObject
    {
        private PocoData pocoData;
        private DatabaseProvider _provider;


        public TableName(Type type)
        {
            pocoData = PocoData.ForType(type);
        }

        public TableName(Type type, DatabaseProvider provider)
        {
            pocoData = PocoData.ForType(type);
            _provider = provider;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var fieldName = binder.Name;
            if (pocoData.Columns.ContainsKey(fieldName)) {
                if (_provider != null) {
                    result = _provider.EscapeSqlIdentifier(pocoData.Columns[fieldName].ColumnName);
                } else {
                    result = pocoData.Columns[fieldName].ColumnName;
                }
                return true;
            }
            fieldName = fieldName.Replace("_", "").ToLower();
            foreach (var item in pocoData.Columns) {
                if (item.Value.PropertyName.Replace("_", "").ToLower() == fieldName) {
                    if (_provider != null) {
                        result = _provider.EscapeSqlIdentifier(item.Value.ColumnName);
                    } else {
                        result = item.Value.ColumnName;
                    }
                    return true;
                }
            }
            result = null;
            return false;
        }


        public override string ToString()
        {
            if (_provider != null) {
                return _provider.EscapeSqlIdentifier(pocoData.TableInfo.TableName);
            }
            return pocoData.TableInfo.TableName;
        }
    }
}
