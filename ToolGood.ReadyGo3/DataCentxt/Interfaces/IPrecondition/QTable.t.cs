using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt
{
   partial class QTable<T> : IPrecondition<QTable<T>>
   {
       public QTable<T> IfTrue(bool b)
       {
           getSqlBuilder().IfTrue(b);
           return this;
       }

       public QTable<T> IfFalse(bool b)
       {
           getSqlBuilder().IfFalse(b);
           return this;
       }

       public QTable<T> IfSet(string txt)
       {
           getSqlBuilder().IfSet(txt);
           return this;
       }

       public QTable<T> IfNotSet(string txt)
       {
           getSqlBuilder().IfNotSet(txt);
           return this;
       }

       public QTable<T> IfNullOrEmpty(string value)
       {
           getSqlBuilder().IfNullOrEmpty(value);
           return this;
       }

       public QTable<T> IfNullOrWhiteSpace(string value)
       {
           getSqlBuilder().IfNullOrWhiteSpace(value);
           return this;
       }

       public QTable<T> IfNull(object obj)
       {
           getSqlBuilder().IfNull(obj);
           return this;
       }

       public QTable<T> IfNotNull(object obj)
       {
           getSqlBuilder().IfNotNull(obj);
           return this;
       }
   }
}
