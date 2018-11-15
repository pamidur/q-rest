using QRest.Core;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.Expressions
{
    public class NamedExpression : ProxyExpression
    {
        public static readonly ExpressionType ExpressionNodeType = (ExpressionType)2101;

        private NamedExpression(Expression expression, string name) : base(expression, ExpressionNodeType)
        {
            Name = name;
        }

        public string Name { get; }
        public override string ToString() => $"{base.ToString()}@{Name}";

        public static NamedExpression Create(Expression expression, string name)
        {
            if(expression is NamedExpression named)            
                expression = named.OriginalExpression;            

            return new NamedExpression(expression, name);
        }
    }
}
