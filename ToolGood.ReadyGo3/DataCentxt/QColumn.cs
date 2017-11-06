using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Enums;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;
using ToolGood.ReadyGo3.DataCentxt.Internals;

namespace ToolGood.ReadyGo3.DataCentxt
{
    public partial class QColumnBase
    {
        protected internal ColumnType _columnType;
        // Column 信息
        protected internal QTable _table;
        protected internal string _columnName;
        protected internal bool _isPrimaryKey;
        protected internal bool _isResultColumn;
        protected internal string _resultSql;
        // Function 信息
        protected internal string _functionName;
        // FunctionFormat 信息
        protected internal string _functionFormat;
        protected internal object[] _functionArgs;
        // Code 信息
        protected internal string _code;
        // As 信息
        protected internal string _asName;


        internal QColumnBase() { }

    }
}
