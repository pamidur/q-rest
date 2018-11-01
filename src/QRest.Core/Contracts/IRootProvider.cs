using System.Linq.Expressions;

namespace QRest.Core.Contracts
{
    public interface IRootProvider
    {
        ParameterExpression GetRoot(ParameterExpression root, Expression context);
        string Key { get; }
    }
}
