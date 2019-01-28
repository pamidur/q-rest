using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using QRest.Core.Contracts;
using QRest.Core.Exceptions;
using QRest.Core.Extensions;

namespace QRest.Core.Operations
{
    public abstract class QueryOperationBase : OperationBase
    {
        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (!context.Type.TryGetCollectionElement(out var element))
                throw new TermTreeCompilationException($"Cannot execute '{Key}' method on non-collection type '{context.Type}'.");

            return CreateExpression(root, context, element.type, element.queryable, arguments, assembler);
        }

        protected abstract Expression CreateExpression(ParameterExpression root, Expression context, Type element, bool quearyable, IReadOnlyList<Expression> arguments, IAssemblerContext assembler);
    }
}
