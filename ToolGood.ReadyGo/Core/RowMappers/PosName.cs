namespace ToolGood.ReadyGo.NPoco.RowMappers
{
    /// <summary>
    /// 表示结果集中某列的位置序号与列名。
    /// </summary>
    public class PosName
    {
        /// <summary>
        /// 列在结果集中的位置序号。
        /// </summary>
        public int Pos { get; set; }

        /// <summary>
        /// 列名。
        /// </summary>
        public string Name { get; set; }
    }
}