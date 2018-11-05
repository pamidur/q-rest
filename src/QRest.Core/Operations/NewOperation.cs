using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using QRest.Core.Contracts;
using QRest.Core.Expressions;

namespace QRest.Core.Operations
{
    public class NewOperation : OperationBase
    {
        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {            
            var expression = arguments.Any() ? assembler.CreateContainer(arguments) : context;

            var expName = assembler.GetName(context) ?? NamedExpression.DefaultObjectResultName;
            return NamedExpression.Create(expression, expName);
        }
    }
}
