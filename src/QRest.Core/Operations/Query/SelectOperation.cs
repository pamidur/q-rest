using QRest.Core.Contracts;
using QRest.Core.Expressions;
using QRest.Core.Extensions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public class SelectOperation : OperationBase
    {      
        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            var queryElement = context.GetQueryElementType();

            var fields = GetInitializers(arguments);

            var expression = DynamicContainer.IsContainerType(queryElement) || !UseStaticTerminatingQuery ?
                QueryDynamic(context, argumentsRoot, fields) :
                QueryNonDynamic(context, argumentsRoot, fields);

            var expName = GetName(context) ?? NamedExpression.DefaultQueryResultName;
            return new NamedExpression(expression, expName);
        }
    }
}
