namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QTable<T>
    {
        /// <summary>
        /// Insert Into T(*)  Select * from T
        /// </summary>
        /// <param name="insertTableName">可以设置为null</param>
        /// <param name="replaceColumns">替换插入的列</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int SelectInsert(string insertTableName=null, string replaceColumns = null, params object[] args)
        {
            return GetSqlBuilder().SelectInsert(insertTableName,replaceColumns, args);
        }

        /// <summary>
        /// Insert Into T1(*)  Select * from T
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="insertTableName">可以设置为null</param>
        /// <param name="replaceColumns">替换插入的列</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int SelectInsert<T1>(string insertTableName = null, string replaceColumns = null, params object[] args)
        {
            return GetSqlBuilder().SelectInsert<T1>(insertTableName,replaceColumns, args);
        }
 

    }
}
