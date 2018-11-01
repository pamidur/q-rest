using QRest.Core.Contracts;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class CompareOperationBase : OperationBase
    {    
        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            var compareArgs = assembler.Convert(context, arguments[0]);

            return PickExpression(compareArgs.Left, compareArgs.Right);
        }

        protected abstract Expression PickExpression(Expression a, Expression b);
    }
}
