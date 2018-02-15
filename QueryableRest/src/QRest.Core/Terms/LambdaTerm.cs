using QRest.Core.Extensions;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class LambdaTerm : MethodTerm
    {
        protected override Expression CreateExpression(Expression prev, ParameterExpression root)
        {
            if (!Operation.SupportsQuery)
                throw new ExpressionCreationException();

            var argsroot = Expression.Parameter(prev.GetQueryElementType(), $"p_{GetHashCode()}");

            var args = Arguments.Select(a => a.CreateExpressionChain(argsroot, argsroot)).ToList();
            var exp = Operation.CreateQueryExpression(root, prev, argsroot, args);

            return exp;
        }

        protected override string Debug => $":{base.Debug.Substring(1)}";               
    }
}
