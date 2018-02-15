using QRest.Core.Containers;
using QRest.Core.Operations;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class PropertyTerm : ITerm
    {
        public string PropertyName { get; set; }
        public ITerm Next { get; set; }

        public Expression CreateExpression(Expression prev, ParameterExpression root)
        {
            Expression exp;

            if (DynamicContainer.IsContainerType(prev.Type))
            {
                exp = DynamicContainer.CreateReadProperty(prev, PropertyName);
            }
            else
            {
                exp = Expression.PropertyOrField(prev, PropertyName);
            }


            return Next?.CreateExpression(exp, root) ?? exp;
        }

        public override string ToString()
        {
            return $".{PropertyName}{Next?.ToString()}";
        }
    }
}
