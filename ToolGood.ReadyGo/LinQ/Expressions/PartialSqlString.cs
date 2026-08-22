namespace ToolGood.ReadyGo.LinQ.Expressions
{
    /// <summary>
    /// SQL片段
    /// </summary>
    public class PartialSqlString
    {
        /// <summary>
        /// SQL片段
        /// </summary>
        /// <param name="text"></param>
        public PartialSqlString(string text)
        {
            Text = text;
        }

        /// <summary>
        /// SQL文本
        /// </summary>
        public string Text;

        /// <summary>
        /// 转为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }
    }
}
