using QRest.Core.Containers;
using QRest.Core.Operations;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class PropertyTerm : TermBase
    {
        public string PropertyName { get; set; }

        protected override Expression CreateExpression(Expression prev, ParameterExpression root)
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


            return exp;
        }

        protected override string Debug => $".{PropertyName}";
    }
}
