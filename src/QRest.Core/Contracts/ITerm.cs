using System.Linq.Expressions;

namespace QRest.Core.Contracts
{
    public interface ITerm
    {
        ITerm Next { get; set; }
        Expression CreateExpression(ICompilerContext compiler, Expression prev, ParameterExpression root);
        ITerm Clone();
        string DebugView { get; }
    }
}