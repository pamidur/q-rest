using QRest.Core.Containers;
using QRest.Core.Operations;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class PropertyTerm : ITerm
    {
        public string PropertyName { get; set; }
        public ITerm Next { get; set; }

        public Expression CreateExpression(Expression prev, ParameterExpression root, QueryContext context)
        {
            Expression exp;

            if (context.ContainerProvider.IsContainerType(prev.Type))
            {
                exp = context.ContainerProvider.CreateReadProperty(prev, PropertyName);
            }
            else
            {
                exp = Expression.PropertyOrField(prev, PropertyName);
            }


            return Next?.CreateExpression(exp, root, context) ?? exp;
        }

        public override string ToString()
        {
            return $".{PropertyName}{Next?.ToString()}";
        }
    }
}
