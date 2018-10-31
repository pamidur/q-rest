using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public class ItOperation : OperationBase
    {
        public override bool SupportsCall => true;

        public override Expression CreateCallExpression(Expression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            return root;
        }
    }
}
