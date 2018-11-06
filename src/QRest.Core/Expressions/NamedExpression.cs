using System;
using System.Linq.Expressions;

namespace QRest.Core.Expressions
{
    public class NamedExpression : Expression
    {
        public static readonly string DefaultQueryResultName = "Query";
        public static readonly string DefaultObjectResultName = "Object";

        public static readonly ExpressionType NamedExpressionType = (ExpressionType)1010;

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
