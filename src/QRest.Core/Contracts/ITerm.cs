using System.Linq.Expressions;

namespace QRest.Core.Contracts
{
    public interface ITerm
    {
        Expression CreateExpression(ICompilationContext compiler, Expression prev, ParameterExpression root);
        ITerm Clone();
        string DebugView { get; }
    }
}