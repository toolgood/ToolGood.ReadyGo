using System.Collections.Generic;
using System.Linq.Expressions;

namespace ToolGood.ReadyGo.NPoco.Expressions
{
    //http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx
    /// <summary>
    /// 参数重绑定器，用于将表达式中的参数替换为映射中对应的参数。
    /// </summary>
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        /// <summary>
        /// 使用参数映射初始化实例。
        /// </summary>
        /// <param name="map">参数替换映射。</param>
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// 替换表达式中的参数。
        /// </summary>
        /// <param name="map">参数替换映射。</param>
        /// <param name="exp">待处理的表达式。</param>
        /// <returns>替换参数后的表达式。</returns>
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        /// <summary>
        /// 访问参数表达式并按映射进行替换。
        /// </summary>
        /// <param name="p">待访问的参数表达式。</param>
        /// <returns>访问后的表达式。</returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }
}