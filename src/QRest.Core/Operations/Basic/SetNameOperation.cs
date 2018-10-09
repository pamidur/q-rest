using QRest.Core.Expressions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Basic
{
    public class SetNameOperation : OperationBase
    {
        private readonly string _name;
        public SetNameOperation(string name) => _name = name;

        public override bool SupportsCall => true;

        public override Expression CreateCallExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            return new NamedExpression(context, _name);
        }
    }
}
