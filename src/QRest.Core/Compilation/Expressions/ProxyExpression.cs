using System;
using System.Linq.Expressions;

namespace QRest.Core.Compilation.Expressions
{
    public abstract class ProxyExpression : Expression
    {
        public Expression OriginalExpression { get; }

        protected ProxyExpression(Expression original)
        {
            OriginalExpression = original;
        }

        public override bool CanReduce => true;
        public override Expression Reduce() => OriginalExpression;
        public override ExpressionType NodeType => ExpressionType.Extension;
        public override Type Type => OriginalExpression.Type;
        public override string ToString() => OriginalExpression.ToString();
    }
}
