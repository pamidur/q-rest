using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard
{
    public class DataStringParser
    {
        private static readonly string _parseMethodName = "Parse";
        private static readonly Type[] _parseWithFormatSignature = new[] { typeof(string), typeof(IFormatProvider) };
        private static readonly Type[] _parseSignature = new[] { typeof(string) };

        private readonly Dictionary<Type, Func<string, object>> _parsers = new Dictionary<Type, Func<string, object>>();

        public Func<string, object> GetParser(Type type)
        {
            if (!_parsers.ContainsKey(type))
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
                _parsers[type] = lambda.Compile();
            }

            return _parsers[type];
        }
    }
}
