using QRest.Core.Contracts;
using QRest.Core.Extensions;
using System.Linq.Expressions;

namespace QRest.Core.RootProviders
{
    public class ElementRootProvider : IRootProvider
    {
        private readonly IRootProvider _baseProvider;
        public ElementRootProvider(IRootProvider baseProvider) => _baseProvider = baseProvider;

        public string Key => _baseProvider.Key + "e";

        public ParameterExpression GetRoot(ParameterExpression root, Expression context)
        {
            var baseroot = _baseProvider.GetRoot(root, context);
            var elementType = baseroot.GetQueryElementType();
            return Expression.Parameter(elementType, baseroot.Name + "e");
        }
    }
}
