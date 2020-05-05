using QRest.Core.Compilation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class LambdaOperationBase : QueryOperationBase
    {
        protected override Expression CreateExpression(Expression context, Type collection, Type element, IReadOnlyList<Expression> arguments, IAssembler assembler)
        {
            if (arguments.Count > 1)
                throw new CompilationException($"Method '{Key}' expects to have the only parameter or no parameters.");


            var arg = arguments[0];
            if (arg.NodeType != ExpressionType.Lambda)
                throw new CompilationException($"Method '{Key}' expects lambda as a parameter.");

            return CreateExpression(context, collection, element, (LambdaExpression)arg, assembler);
        }

        protected abstract Expression CreateExpression(Expression context, Type collection, Type element, LambdaExpression argument, IAssembler assembler);  
    }
}
