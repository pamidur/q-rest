using QRest.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class OperationBase : IOperation
    {
        private static readonly string _parseMethodName = "Parse";
        private static readonly Type[] _parseWithFormatSignature = new[] { typeof(string), typeof(IFormatProvider) };
        private static readonly Type[] _parseSignature = new[] { typeof(string) };

        public bool TryParseFromStrings { get; set; } = true;

        public Dictionary<Type, Func<string, object>> Parsers { get; set; } = new Dictionary<Type, Func<string, object>>();

        public OperationBase()
        {
            Key = GetType().Name.ToLowerInvariant().Replace("operation", "");
        }

        public virtual string Key { get; } 

        public virtual Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return GetType().Name.ToLowerInvariant().Replace("operation", "");
        }

        protected bool TryCast(Expression expression, Type target, out Expression result)
        {
            if (expression.Type == target)
            {
                result = expression;
                return true;
            }
            else if (target.IsAssignableFrom(expression.Type))
            {
                result = Expression.Convert(expression, target);
                return true;
            }
            else if(TryParseFromStrings && expression.NodeType == ExpressionType.Constant && expression.Type == typeof(string))
            {
                var parser = GetParser(target);
                if (parser != null)
                {
                    var value = (string)((ConstantExpression)expression).Value;

                    try
                    {
                        result = Expression.Constant(parser(value));
                        return true;
                    }
                    catch (FormatException)
                    {
                        throw new ExpressionCreationException($"Cannot parse '{value}' to {target.Name}");
                    }
                    
                }
            }

            result = null;
            return false;            
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
                    else return null;
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
