using QueryableRest.Semantics.Containers;
using QueryableRest.Semantics.Operations;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.Terms
{
    public class PropertyTerm : ITerm
    {
        public string PropertyName { get; set; }
        public ITerm Next { get; set; }

        public Expression CreateExpression(Expression context, Expression root, Registry registry)
        {
            Expression exp;

            if(context.Type == PropertiesContainer.Type)
            {
                exp = Expression.Property(context, "Item", Expression.Constant(PropertyName));
            }
            else
            {
                exp = Expression.PropertyOrField(context, PropertyName);
            }


            return Next?.CreateExpression(exp, root, registry) ?? exp;
        }

        public override string ToString()
        {
            return $".{PropertyName}{Next?.ToString()}";
        }
    }
}
