using QRest.Core.Contracts;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class OperationBase : IOperation
    {
        public virtual string Key { get; }

        public OperationBase()
        {
            Key = GetType().Name.ToLowerInvariant().Replace("operation", "");
        }

        public override string ToString() => Key;

        public abstract Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler);
    }
}
