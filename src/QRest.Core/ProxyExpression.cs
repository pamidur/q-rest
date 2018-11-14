using System;
using System.Linq.Expressions;

namespace QRest.Core
{
    public abstract class ProxyExpression : Expression
    {
        protected ProxyExpression(Expression original, ExpressionType expressionType)
        {
            _original = original;
            _expressionType = expressionType;
        }

        private readonly Expression _original;
        private readonly ExpressionType _expressionType;

        public override bool CanReduce => true;
        public override Expression Reduce() => _original;
        public override ExpressionType NodeType => _expressionType;
        public override Type Type => _original.Type;
        public override string ToString() => _original.ToString();
    }
}
