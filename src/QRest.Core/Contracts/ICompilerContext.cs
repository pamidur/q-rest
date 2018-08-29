using System.Linq.Expressions;

namespace QRest.Core.Contracts
{
    public interface ICompilerContext
    {
        Expression Compile(ITerm term, Expression context, ParameterExpression root);
    }
}
