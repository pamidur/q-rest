using System;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.Expressions
{
    public class NamedExpression : Expression
    {
        public static readonly ExpressionType NamedExpressionType = (ExpressionType)2101;

        private NamedExpression(Expression expression, string name)
        {
            Expression = expression;
            Name = name;
        }

        public Expression Expression { get; }
        public string Name { get; }

        public override ExpressionType NodeType => NamedExpressionType;
        public override Type Type => Expression.Type;

        public override Expression Reduce() => Expression;
        public override bool CanReduce => true;

        public override string ToString()
        {
            return $"{Expression.ToString()}@{Name}";
        }

        public static NamedExpression Create(Expression expression, string name)
        {
            if(expression is NamedExpression named)            
                expression = named.Expression;            

            return new NamedExpression(expression, name);
        }
    }
}
