using QRest.Core.Extensions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Selectors
{
    public class ElementOperation : OperationBase
    {
        public override Expression CreateCallExpression(Expression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            var etype = context.GetQueryElementType();
            var arg = Expression.Parameter(etype, etype.Name.ToLowerInvariant());
            return arg;
        }
    }
}
