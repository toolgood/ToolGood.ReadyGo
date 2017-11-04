using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.DataCentxt.Exceptions
{
    public class ColumnTypeException : Exception
    {
        public ColumnTypeException()
            :base("The column is not table's column.")
        {
            
        }

    }
}
