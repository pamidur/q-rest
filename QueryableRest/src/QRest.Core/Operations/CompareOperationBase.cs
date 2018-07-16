using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class CompareOperationBase : OperationBase
    {
        private static readonly string _parseMethodName = "Parse";
        private static readonly Type[] _parseWithFormatSignature = new[] { typeof(string), typeof(IFormatProvider) };
        private static readonly Type[] _parseSignature = new[] { typeof(string) };


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
                var source = Expression.Parameter(typeof(string));
                var format = Expression.Constant(CultureInfo.InvariantCulture, typeof(IFormatProvider));

                Expression[] parameters;

                var method = type.GetMethod(_parseMethodName, _parseWithFormatSignature);
                if (method != null) parameters = new Expression[] { source, format };
                else
                {
                    method = type.GetMethod(_parseMethodName, _parseSignature);
                    if (method != null) parameters = new Expression[] { source };
                    else throw new NotSupportedException($"Cannot create parser for {type.FullName}.");
                }

                var call = Expression.Call(method, parameters);
                var convert = Expression.Convert(call, typeof(object));
                var lambda = Expression.Lambda<Func<string, object>>(convert, source);
                Parsers.Add(type, lambda.Compile());
            }

            return Parsers[type];
        }
    }
}
