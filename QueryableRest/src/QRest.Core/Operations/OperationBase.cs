using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class OperationBase : IOperation
    {
        public virtual bool SupportsQuery => false;
        public virtual bool SupportsCall => false;

        public virtual Expression CreateCallExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            throw new NotSupportedException();
        }        

        public virtual Expression CreateQueryExpression(ParameterExpression root, Expression context, ParameterExpression argumentsRoot, IReadOnlyList<Expression> arguments)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return GetType().Name.ToLowerInvariant().Replace("operation", "");
        }
    }
}
