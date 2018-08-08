namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QTable<T> //: IPrecondition<QTable<T>>
    {
        /// <summary>
        /// 判断是否为真，否则跳过下一次
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public QTable<T> IfTrue(bool b)
        {
            GetSqlBuilder().IfTrue(b);
            return this;
        }
        /// <summary>
        /// 判断是否为假，否则跳过下一次
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public QTable<T> IfFalse(bool b)
        {
            GetSqlBuilder().IfFalse(b);
            return this;
        }
        /// <summary>
        /// 判断是否为设置，否则跳过下一次
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public QTable<T> IfSet(string txt)
        {
            GetSqlBuilder().IfSet(txt);
            return this;
        }
        /// <summary>
        /// 判断是否为没设置，否则跳过下一次
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public QTable<T> IfNotSet(string txt)
        {
            GetSqlBuilder().IfNotSet(txt);
            return this;
        }
        /// <summary>
        /// 判断是否为没设置，否则跳过下一次
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public QTable<T> IfNullOrEmpty(string value)
        {
            GetSqlBuilder().IfNullOrEmpty(value);
            return this;
        }
        /// <summary>
        /// 判断是否为没设置,为空格，否则跳过下一次
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public QTable<T> IfNullOrWhiteSpace(string value)
        {
            GetSqlBuilder().IfNullOrWhiteSpace(value);
            return this;
        }
        /// <summary>
        /// 判断是否为Null，否则跳过下一次
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public QTable<T> IfNull(object obj)
        {
            GetSqlBuilder().IfNull(obj);
            return this;
        }
        /// <summary>
        /// 判断是否为非Null，否则跳过下一次
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public QTable<T> IfNotNull(object obj)
        {
            GetSqlBuilder().IfNotNull(obj);
            return this;
        }
    }
}
