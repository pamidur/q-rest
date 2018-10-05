using QRest.Core.Containers;
using QRest.Core.Contracts;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class PropertyTerm : TermBase
    {
        public string PropertyName { get; set; }

        public override Expression CreateExpression(ICompilationContext compiler, Expression prev, ParameterExpression root)
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

        public override string DebugView => $".{PropertyName}";

        public override ITerm Clone() => new PropertyTerm { PropertyName = PropertyName};
    }
}
