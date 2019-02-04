using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Compilation.TypeConverters
{
    public class StringParser
    {
        private static readonly string _parseMethodName = "Parse";
        private static readonly Type[] _parseWithFormatSignature = new[] { typeof(string), typeof(IFormatProvider) };
        private static readonly Type[] _parseSignature = new[] { typeof(string) };

        private static readonly Dictionary<Type, Func<Expression, IFormatProvider, Expression>> _parsers = new Dictionary<Type, Func<Expression, IFormatProvider, Expression>>();

        public static Func<Expression, IFormatProvider, Expression> GetParser(Type type)
        {
            if (!_parsers.ContainsKey(type))
            {
                var source = Expression.Parameter(typeof(string));

                Func<Expression, IFormatProvider, Expression[]> parameters = null;

                var method = type.GetMethod(_parseMethodName, _parseWithFormatSignature);
                if (method != null) parameters = (e, c) => new Expression[] { e, Expression.Constant(c, typeof(IFormatProvider)) };
                else
                {
                    method = type.GetMethod(_parseMethodName, _parseSignature);
                    if (method != null)
                        parameters = (e, c) => new Expression[] { e };
                }

                if (method != null && parameters != null)
                    _parsers[type] = (e, c) => Expression.Call(method, parameters(e, c));
                else
                    _parsers[type] = null;
            }

            return _parsers[type];
        }
    }
}
