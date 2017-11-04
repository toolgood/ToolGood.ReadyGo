using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt.Internals
{
    partial class SqlBuilder : IPrecondition
    {
        public void IfTrue(bool b)
        {
            _jump = true;
        }

        public void IfFalse(bool b)
        {
            IfTrue(!b);
        }

        public void IfSet(string txt)
        {
            IfTrue(string.IsNullOrEmpty(txt) == false);
        }

        public void IfNotSet(string txt)
        {
            IfTrue(string.IsNullOrEmpty(txt));
        }

        public void IfNullOrEmpty(string value)
        {
            IfTrue(string.IsNullOrEmpty(value));
        }

        public void IfNullOrWhiteSpace(string value)
        {
            IfTrue(string.IsNullOrWhiteSpace(value));
        }

        public void IfNull(object obj)
        {
            IfTrue(object.Equals(null,obj));
        }

        public void IfNotNull(object obj)
        {
            IfTrue(object.Equals(null, obj)==false);
        }
    }
}
