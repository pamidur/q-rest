using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public interface ITerm
    {
        ITerm Next { get; set; }
        Expression CreateExpressionChain(Expression prev, ParameterExpression root);        
    }
}