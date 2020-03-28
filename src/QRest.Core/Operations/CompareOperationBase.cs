using QRest.Core.Compilation;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class CompareOperationBase : OperationBase
    {    
        public override Expression CreateExpression(Expression context, IReadOnlyList<Expression> arguments, IAssembler assembler)
        {
            if (arguments.Count != 1)
                throw new CompilationException("Expected 1 parameter");

            var (Left, Right) = MutualConvert(assembler, context, arguments[0]);

            return PickExpression(Left, Right);
        }

        protected static (Expression Left, Expression Right) MutualConvert(IAssembler assembler, Expression left, Expression right)
        {
            var converter = assembler.TypeConverter;

            var leftType = /*left.NodeType == ExpressionType.Lambda ? ((LambdaExpression)left).ReturnType : */left.Type;
            var rightType = /*right.NodeType == ExpressionType.Lambda ? ((LambdaExpression)right).ReturnType :*/ right.Type;

            if (converter.TryConvert(left, leftType, out var updatedLeft))
                left = updatedLeft;

            if (converter.TryConvert(right, rightType, out var updatedRight))
                right = updatedRight;

            if (assembler.TypeConverter.TryConvert(right, leftType, out var newright))
                return (left, newright);
            else if (assembler.TypeConverter.TryConvert(left, rightType, out var newleft))
                return (newleft, right);
            else
                throw new CompilationException($"Cannot cast {leftType.Name} and {rightType.Name} either way.");
        }

        protected abstract Expression PickExpression(Expression a, Expression b);
    }
}
