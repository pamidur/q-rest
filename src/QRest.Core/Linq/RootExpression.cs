using QRest.Core.Operations;
using System;
using System.Linq.Expressions;

namespace QRest.Core.Linq
{
    public class RootExpression : Expression
    {
        public RootExpression(Type type)
        {
            Type = type;
        }

        public override Type Type { get; }
        public override ExpressionType NodeType =>ExpressionType.Extension;

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is ConvertingVisitor convertingVisitor)
                return convertingVisitor.VisitRoot();
            return base.Accept(visitor);
        }

        public override string ToString() => "$";
    }
}
