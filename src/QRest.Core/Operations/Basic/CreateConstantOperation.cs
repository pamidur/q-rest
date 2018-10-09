using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Basic
{
    public class CreateConstantOperation : OperationBase
    {
        private readonly object _value;
        private readonly Type _type;

        public CreateConstantOperation(object value, Type type)
        {
            _value = value;
            _type = type;
        }

        public override bool SupportsCall => true;

        public override Expression CreateCallExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            return Expression.Constant(_value, _type);
        }
    }
}
