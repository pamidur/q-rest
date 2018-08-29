using System;
using System.Linq.Expressions;

namespace QRest.Core.Expressions
{
    class AscendingExpression : Expression
    {
        public static readonly ExpressionType AscendingExpressionType = (ExpressionType)1011;

        public AscendingExpression(Expression expression)
        {
            Expression = expression;
        }

        public Expression Expression { get; }

        public override ExpressionType NodeType => AscendingExpressionType;
        public override Type Type => Expression.Type;

        public override Expression Reduce() => Expression;
        public override bool CanReduce => true;
    }
}
