using System;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class ProxyExpression : Expression
    {
        public Expression OriginalExpression { get; }

        protected ProxyExpression(Expression original, ExpressionType expressionType)
        {
            OriginalExpression = original;
            _expressionType = expressionType;
        }

        private readonly ExpressionType _expressionType;

        public override bool CanReduce => true;
        public override Expression Reduce() => OriginalExpression;
        public override ExpressionType NodeType => _expressionType;
        public override Type Type => OriginalExpression.Type;
        public override string ToString() => OriginalExpression.ToString();
    }
}
