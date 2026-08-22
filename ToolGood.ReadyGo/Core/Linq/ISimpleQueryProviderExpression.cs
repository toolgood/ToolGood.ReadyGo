using ToolGood.ReadyGo.NPoco.Expressions;

namespace ToolGood.ReadyGo.NPoco.Linq
{
    public interface ISimpleQueryProviderExpression<TModel>
    {
        ISqlExpression<TModel> AtlasSqlExpression { get; }
    }
}