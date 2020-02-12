using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace QRest.Core.Compilation.TypeConverters
{
    public class StringParser
    {
        private static readonly string _parseMethodName = "Parse";
        private static readonly Type[] _parseWithFormatSignature = new[] { typeof(string), typeof(IFormatProvider) };
        private static readonly Type[] _parseSignature = new[] { typeof(string) };
        private static readonly MethodInfo _enumParser = typeof(Enum).GetMethod(nameof(Enum.Parse), new[] { typeof(Type), typeof(string) });

        private static readonly Dictionary<Type, Func<Expression, IFormatProvider, Expression>> _parsers = new Dictionary<Type, Func<Expression, IFormatProvider, Expression>>();

        public static Func<Expression, IFormatProvider, Expression> GetParser(Type type)
        {
            if (!_parsers.ContainsKey(type))
            {
                if (type.IsEnum)
                {
                    _parsers[type] = ParseEnum(type);
                }
                else
                {
                    _parsers[type] = ParseOtherTypes(type) ?? throw new CompilationException($"Cannot find parser for type {type}");
                }
            }

            return _parsers[type];
        }

        private static Func<Expression, IFormatProvider, Expression> ParseOtherTypes(Type type)
        {
            var method = type.GetMethod(_parseMethodName, _parseWithFormatSignature);
            if (method != null)
                return (e, c) => Expression.Call(method, new[] { e, Expression.Constant(c, typeof(IFormatProvider)) });

            method = type.GetMethod(_parseMethodName, _parseSignature);
            if (method != null)
                return (e, c) => Expression.Call(method, new[] { e });

            return null;
        }

        private static Func<Expression, IFormatProvider, Expression> ParseEnum(Type type)
        {
            return (e, c) => Expression.Convert(Expression.Call(_enumParser, new[] { Expression.Constant(type), e }), type);
        }
    }
}
