using QRest.Core.Contracts;
using System.Linq.Expressions;

namespace QRest.Core.RootProviders
{
    public class SameContextProvider : IRootProvider
    {
        public string Key => "c";

        public ParameterExpression GetRoot(ParameterExpression root, Expression context)
        {
            return Expression.Parameter(context.Type, "c");
        }
    }
}
