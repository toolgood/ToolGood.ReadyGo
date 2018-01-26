using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ToolGood.ReadyGo3.DataCentxt
{
   partial class QTable<T> //: IPrecondition<QTable<T>>
   {
       public QTable<T> IfTrue(bool b)
       {
           GetSqlBuilder().IfTrue(b);
           return this;
       }

       public QTable<T> IfFalse(bool b)
       {
           GetSqlBuilder().IfFalse(b);
           return this;
       }

       public QTable<T> IfSet(string txt)
       {
           GetSqlBuilder().IfSet(txt);
           return this;
       }

       public QTable<T> IfNotSet(string txt)
       {
           GetSqlBuilder().IfNotSet(txt);
           return this;
       }

       public QTable<T> IfNullOrEmpty(string value)
       {
           GetSqlBuilder().IfNullOrEmpty(value);
           return this;
       }

       public QTable<T> IfNullOrWhiteSpace(string value)
       {
           GetSqlBuilder().IfNullOrWhiteSpace(value);
           return this;
       }

       public QTable<T> IfNull(object obj)
       {
           GetSqlBuilder().IfNull(obj);
           return this;
       }

       public QTable<T> IfNotNull(object obj)
       {
           GetSqlBuilder().IfNotNull(obj);
           return this;
       }
   }
}
