using System;

namespace ToolGood.ReadyGo.Exceptions
{
    /// <summary>
    /// 【数据库不支持】异常
    /// </summary>
    public class DatabaseUnsupportException : Exception
    {
        /// <summary>
        /// 【数据库不支持】异常
        /// </summary>
        public DatabaseUnsupportException()
        { }

        /// <summary>
        /// 【数据库不支持】异常
        /// </summary>
        /// <param name="message">异常信息</param>
        public DatabaseUnsupportException(string message)
            : base(message)
        { }

        /// <summary>
        /// 【数据库不支持】异常
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">内部异常</param>
        public DatabaseUnsupportException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
