using QRest.Core.Containers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QRest.Core.Operations.Basic
{
    public class GetPropertyOperation : OperationBase
    {
        private readonly string _name;

        public GetPropertyOperation(string name)
        {
            _name = name;
        }

        public override bool SupportsCall => true;

        public override Expression CreateCallExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            Expression exp;

            if (DynamicContainer.IsContainerType(context.Type))
            {
                exp = DynamicContainer.CreateReadProperty(context, _name);
            }
            else
            {
                exp = Expression.PropertyOrField(context, _name);
            }


            return exp;
        }
    }
}
