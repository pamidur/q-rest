using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public interface ITerm
    {
        ITerm Next { get; set; }
        Expression CreateExpression(Expression prev, ParameterExpression root, QueryContext context);        
    }
}
