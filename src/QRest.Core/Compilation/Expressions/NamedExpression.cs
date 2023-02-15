using System.Linq.Expressions;

namespace QRest.Core.Compilation.Expressions
{
    public class NamedExpression : ProxyExpression
    {
        private NamedExpression(Expression expression, string name) : base(expression)
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
