using System.Linq.Expressions;

namespace QueryableRest.Semantics.Terms
{
    public class PropertyTerm : ITerm
    {
        public string PropertyName { get; set; }

        public Expression CreateExpression(Expression context, Registry registry)
        {
            return Expression.PropertyOrField(context, PropertyName);
        }
    }
}
