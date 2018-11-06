using QRest.Core.Contracts;
using System.Linq.Expressions;

namespace QRest.Core.RootProviders
{
    public class SameRootProvider : IRootProvider
    {
        public string Key => "r";

        public ParameterExpression GetRoot(ParameterExpression root, Expression context)
        {
            return root;
        }
    }
}
