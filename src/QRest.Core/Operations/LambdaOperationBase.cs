using QRest.Core.Contracts;
using QRest.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class LambdaOperationBase : QueryOperationBase
    {       
        protected override Expression CreateExpression(ParameterExpression root, Expression context, Type element, Type collectionType, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count > 1)
                throw new TermTreeCompilationException($"Method '{Key}' expects to have the only parameter or no parameters.");


            var arg = arguments[0];

            if (arg.NodeType != ExpressionType.Lambda)
                throw new TermTreeCompilationException($"Method '{Key}' expects lambda as a parameter.");

            return CreateExpression(root, context, element, collectionType, (LambdaExpression)arg, assembler);
        }

        protected abstract Expression CreateExpression(ParameterExpression root, Expression context, Type element, Type collectionType, LambdaExpression argument, IAssemblerContext assembler);

    }
}
