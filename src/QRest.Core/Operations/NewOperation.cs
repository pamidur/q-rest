using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using QRest.Core.Contracts;

namespace QRest.Core.Operations
{
    public class NewOperation : OperationBase
    {
        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            throw new NotImplementedException();
        }

        //public override Expression CreateExpression(Expression root, Expression context, IReadOnlyList<Expression> arguments)
        //{
        //    var expression = arguments.Any() ? DynamicContainer.CreateContainer(GetInitializers(arguments)) : root;

        //    var expName = GetName(context) ?? NamedExpression.DefaultObjectResultName;
        //    return new NamedExpression(expression, expName);
        //}
    }
}
