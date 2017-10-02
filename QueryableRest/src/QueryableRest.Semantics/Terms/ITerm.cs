using System.Linq.Expressions;

namespace QueryableRest.Semantics.Terms
{
    public interface ITerm
    {
        ITerm Parent { get; set; }
        Expression CreateExpression(Registry registry);
    }
}
