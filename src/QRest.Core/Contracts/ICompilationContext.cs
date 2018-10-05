using System.Linq.Expressions;

namespace QRest.Core.Contracts
{
    public interface ICompilationContext
    {
        Expression Assemble(ITermSequence term, Expression context, ParameterExpression root);
    }
}
