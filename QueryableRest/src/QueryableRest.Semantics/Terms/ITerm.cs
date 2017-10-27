using System.Linq.Expressions;

namespace QueryableRest.Semantics.Terms
{
    public interface ITerm
    {
        ITerm Next { get; set; }
        Expression CreateExpression(Expression context, Registry registry);        
    }
}
