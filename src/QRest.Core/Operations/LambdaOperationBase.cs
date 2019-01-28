using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using QRest.Core.Contracts;
using QRest.Core.Exceptions;

namespace QRest.Core.Operations
{
    public abstract class LambdaOperationBase : QueryOperationBase
    {
        protected override Expression CreateExpression(ParameterExpression root, Expression context, Type element, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count > 1)
                throw new TermTreeCompilationException($"Method '{Key}' expects to have the only parameter or no parameters.");


            var arg = arguments[0];

            if (arg.NodeType != ExpressionType.Lambda)
                throw new TermTreeCompilationException($"Method '{Key}' expects lambda as a parameter.");

            return CreateExpression(root, context, element, (LambdaExpression)arg, assembler);
        }

        protected abstract Expression CreateExpression(ParameterExpression root, Expression context, Type element, LambdaExpression argument, IAssemblerContext assembler);

    }
}
