using System;
using System.Linq.Expressions;

namespace QRest.Core.Expressions
{
    class DescendingExpression : Expression
    {
        public static readonly ExpressionType DescendingExpressionType = (ExpressionType)1012;

        public DescendingExpression(Expression expression)
        {
            Expression = expression;
        }

        public Expression Expression { get; }

        public override ExpressionType NodeType => DescendingExpressionType;
        public override Type Type => Expression.Type;

        public override Expression Reduce() => Expression;
        public override bool CanReduce => true;
    }
}
