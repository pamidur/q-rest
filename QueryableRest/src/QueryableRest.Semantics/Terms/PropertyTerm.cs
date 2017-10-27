using System.Linq.Expressions;

namespace QueryableRest.Semantics.Terms
{
    public class PropertyTerm : ITerm
    {
        public string PropertyName { get; set; }
        public ITerm Next { get; set; }

        public Expression CreateExpression(Expression context, Registry registry)
        {
            var exp = Expression.PropertyOrField(context, PropertyName);
            return Next?.CreateExpression(exp, registry) ?? exp;
        }

        public override string ToString()
        {
            return $".{PropertyName}{Next?.ToString()}";
        }
    }
}
