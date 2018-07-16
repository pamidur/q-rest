using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class CompareOperationBase : OperationBase
    {
        public override bool SupportsCall => true;

        public Dictionary<Type, Func<string, object>> Parsers { get; set; } = new Dictionary<Type, Func<string, object>>();
        public bool TryParseFromStrings { get; set; }

        public override Expression CreateCallExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            var compareArgs = Convert(context, arguments[0]);

            return PickExpression(compareArgs.Left, compareArgs.Right);
        }

        protected abstract Expression PickExpression(Expression a, Expression b);


        protected virtual (Expression Left, Expression Right) Convert(Expression left, Expression right)
        {
            if (left.Type == right.Type)
                return (left, right);

            if (left.Type.IsAssignableFrom(right.Type))
                return (Expression.Convert(left, right.Type), right);

            if (right.Type.IsAssignableFrom(left.Type))
                return (left, Expression.Convert(right, left.Type));

            if (TryParseFromStrings)
            {
                if (left.NodeType == ExpressionType.Constant && left.Type == typeof(string))
                    return (ParseConstant((ConstantExpression)left, right.Type), right);

                if (right.NodeType == ExpressionType.Constant && right.Type == typeof(string))
                    return (left, ParseConstant((ConstantExpression)right, left.Type));
            }

            throw new ExpressionCreationException();
        }

        protected virtual Expression ParseConstant(ConstantExpression constant, Type target)
        {
            var newValue = GetParser(target)((string)constant.Value);
            return Expression.Constant(newValue, target);
        }

        private Func<string, object> GetParser(Type type)
        {
            if (!Parsers.ContainsKey(type))
            {
                try
                {
                    var param = Expression.Parameter(typeof(string));
                    var call = Expression.Call(type, "Parse", new Type[] { }, param);
                    var convert = Expression.Convert(call, typeof(object));
                    var lambda = Expression.Lambda<Func<string, object>>(convert, param);
                    Parsers.Add(type, lambda.Compile());
                }
                catch
                {
                    throw new NotSupportedException($"Cannot create parser for {type.FullName}.");
                }

            }

            return Parsers[type];
        }
    }
}
