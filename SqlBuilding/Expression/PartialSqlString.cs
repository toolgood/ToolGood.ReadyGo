namespace ToolGood.ReadyGo.SqlBuilding
{
    public class PartialSqlString
    {
        public PartialSqlString(string text)
        {
            Text = text;
        }

        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}