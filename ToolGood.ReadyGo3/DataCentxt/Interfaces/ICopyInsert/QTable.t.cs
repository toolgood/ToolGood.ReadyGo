using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QTable<T>
    {
        /// <summary>
        /// Insert Into T(*)  Select * from T
        /// </summary>
        /// <param name="replaceColumns"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int SelectInsert(string replaceColumns = "", params object[] args)
        {
            return GetSqlBuilder().SelectInsert(replaceColumns, args);
        }
 
        /// <summary>
        /// Insert Into T1(*)  Select * from T
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="replaceColumns"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int SelectInsert<T1>(string replaceColumns = "", params object[] args)
        {
            return GetSqlBuilder().SelectInsert<T1>(replaceColumns, args);
        }
 

    }
}
