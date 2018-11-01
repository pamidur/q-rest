using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public class EveryOperation : OperationBase
    {
        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count < 1)
                throw new ExpressionCreationException();

            var body = arguments.Aggregate((e1, e2) => { return Expression.AndAlso(e1, e2); });

            return body;
        }
    }
}
