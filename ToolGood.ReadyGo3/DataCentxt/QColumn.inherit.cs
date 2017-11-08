using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Enums;
using ToolGood.ReadyGo3.DataCentxt.Exceptions;
using ToolGood.ReadyGo3.DataCentxt.Internals;

namespace ToolGood.ReadyGo3.DataCentxt
{
    [Serializable]
    public class QColumn : QColumnBase
    {
        public QColumn As(string name)
        {
            _asName = name;
            return this;
        }
    }
    [Serializable]
    public abstract class QTableColumn : QColumnBase
    {
        protected internal ColumnChangeType _changeType;
        protected internal string _fieldType;

        internal abstract object GetValue();
        protected internal abstract void SetValue(object value);
        internal abstract QTableColumn GetNewValue();
        protected internal abstract void ClearValue();


    }
    [Serializable]
    public class QTableColumn<T> : QTableColumn
    {
        protected internal T _value;
        protected internal QTableColumn<T> _newValue;

        internal QTableColumn() : base()
        {
            _fieldType = typeof(T).Name.ToLower();
        }


        public QTableColumn<T> NewValue
        {
            get { return (QTableColumn<T>)_newValue; }
            set {
                _newValue = value;
                if (value._columnType == ColumnType.Value) {
                    _value = ((QTableColumn<T>)value)._value;
                    _changeType = ColumnChangeType.NewValue;
                    return;
                }
                _changeType = ColumnChangeType.NewSql;
            }
        }

        public QTableColumn<T> As(string name)
        {
            _asName = name;
            return this;
        }

        internal override object GetValue()
        {
            return _value;
        }

        internal override QTableColumn GetNewValue()
        {
            return _newValue;
        }

        protected internal override void ClearValue()
        {
            _value = default(T);
            _changeType = ColumnChangeType.None;
        }


        protected internal override void SetValue(object value)
        {
            object obj = ChangeType(value, typeof(T));
            _value = (T)obj;
            _changeType = ColumnChangeType.NewValue;
        }

        public static implicit operator QTableColumn<T>(QColumn value)
        {
            return new QTableColumn<T>() {
                _asName = value._asName,
                _code = value._code,
                _columnName = value._columnName,
                _columnType = value._columnType,
                _functionFormat = value._functionFormat,
                _functionName = value._functionName,
                _isPrimaryKey = value._isPrimaryKey,
                _isResultColumn = value._isResultColumn,
                _resultSql = value._resultSql,
                _table = value._table,
                _functionArgs = value._functionArgs,
            };
        }

        public static QCondition operator !(QTableColumn<T> col)
        {
            if (typeof(T) != typeof(bool)) {
                throw new ColumnTypeException();
            }
            return CreateCondition(col, "<>", true);
        }



        public static implicit operator QTableColumn<T>(Int16 value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(Int32 value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(Int64 value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(UInt16 value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(UInt32 value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(UInt64 value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(Decimal value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(Single value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(Double value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(DateTime value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(string value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(bool value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(byte value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(byte[] value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(Guid value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        public static implicit operator QTableColumn<T>(Enum value)
        {
            object obj = ChangeType(value, typeof(T));
            return new QTableColumn<T>() { _value = (T)obj, _columnType = Enums.ColumnType.Value };
        }
        internal static object ChangeType(object value, Type type)
        {
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            if (type.IsEnum) {
                if (value is string)
                    return Enum.Parse(type, value as string);
                else
                    return Enum.ToObject(type, value);
            }
            if (!type.IsInterface && type.IsGenericType) {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type);
        }



    }
}
