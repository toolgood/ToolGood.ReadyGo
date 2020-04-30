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
        private string _asName;
        private PocoData _pocoData;
        private DatabaseProvider _provider;




        public TableName(Type type, DatabaseProvider provider, string asName)
        {
            _pocoData = PocoData.ForType(type);
            _provider = provider;
            _asName = asName;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var fieldName = binder.Name;
            if (_pocoData.Columns.ContainsKey(fieldName)) {
                if (_provider != null) {
                    if (string.IsNullOrEmpty(_asName)) {
                        result = _provider.EscapeSqlIdentifier(_pocoData.Columns[fieldName].ColumnName);
                    } else {
                        result = _asName + "." + _provider.EscapeSqlIdentifier(_pocoData.Columns[fieldName].ColumnName);
                    }
                } else if (string.IsNullOrEmpty(_asName)) {
                    result = _pocoData.Columns[fieldName].ColumnName;
                } else {
                    result = _asName + "." + _pocoData.Columns[fieldName].ColumnName;
                }
                return true;
            }
            fieldName = fieldName.Replace("_", "").ToLower();
            foreach (var item in _pocoData.Columns) {
                if (item.Value.PropertyName.Replace("_", "").ToLower() == fieldName) {
                    if (_provider != null) {
                        if (string.IsNullOrEmpty(_asName)) {
                            result = _provider.EscapeSqlIdentifier(item.Value.ColumnName);
                        } else {
                            result = _asName + "." + _provider.EscapeSqlIdentifier(item.Value.ColumnName);
                        }
                    } else if (string.IsNullOrEmpty(_asName)) {
                        result = item.Value.ColumnName;
                    } else {
                        result = _asName + "." + item.Value.ColumnName;
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
                return _provider.EscapeSqlIdentifier(_pocoData.TableInfo.TableName);
            }
            return _pocoData.TableInfo.TableName;
        }
    }
}
