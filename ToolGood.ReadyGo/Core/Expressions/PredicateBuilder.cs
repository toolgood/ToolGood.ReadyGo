using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ToolGood.ReadyGo.NPoco.Expressions
{
    /// <summary>
    /// 提供查询谓词表达式的高效动态组合能力。
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// 根据指定 Lambda 表达式创建谓词表达式。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="predicate">谓词 Lambda 表达式。</param>
        /// <returns>与输入等价的谓词表达式。</returns>
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate)
        {
            return predicate;
        }

        /// <summary>
        /// 使用逻辑“与”组合第一个谓词与第二个谓词。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="first">第一个谓词。</param>
        /// <param name="second">第二个谓词。</param>
        /// <returns>组合后的谓词表达式。</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
                                                       Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// Combines the first expression with the second using the specified merge function.
        /// </summary>
        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
                                                Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            Dictionary<ParameterExpression, ParameterExpression> map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            Expression secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
    }
}