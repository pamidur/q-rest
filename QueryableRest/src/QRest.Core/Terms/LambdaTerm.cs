using QRest.Core.Extensions;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class LambdaTerm : MethodTerm
    {
        public override Expression CreateExpression(Expression prev, ParameterExpression root)
        {
            if (!Operation.SupportsQuery)
                throw new ExpressionCreationException();

            var argsroot = Expression.Parameter(prev.GetQueryElementType(), $"p_{GetHashCode()}");

            var args = Arguments.Select(a => a.CreateExpression(argsroot, argsroot)).ToList();
            var exp = Operation.CreateQueryExpression(root, prev, argsroot, args);

            return Next?.CreateExpression(exp, root) ?? exp;
        }

        public override string ToString()
        {
            return $":{base.ToString().Substring(1)}";
        }       
    }
}
