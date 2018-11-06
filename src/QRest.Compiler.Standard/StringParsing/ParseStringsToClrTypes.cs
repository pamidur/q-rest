using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.StringParsing
{
    public class ParseStringsToClrTypes : IStringParsingBehavior
    {
        private static readonly string _parseMethodName = "Parse";
        private static readonly Type[] _parseWithFormatSignature = new[] { typeof(string), typeof(IFormatProvider) };
        private static readonly Type[] _parseSignature = new[] { typeof(string) };

        private readonly Dictionary<Type, Func<Expression, Expression>> _parsers = new Dictionary<Type, Func<Expression, Expression>>();

        public CultureInfo ParsingCulture { get; set; } = CultureInfo.InvariantCulture;

        public Func<Expression, Expression> GetParser(Type type)
        {
            if (!_parsers.ContainsKey(type))
            {
                var source = Expression.Parameter(typeof(string));
                var format = Expression.Constant(ParsingCulture, typeof(IFormatProvider));

                Func<Expression,Expression[]> parameters;

                var method = type.GetMethod(_parseMethodName, _parseWithFormatSignature);
                if (method != null) parameters = e => new Expression[] { e, format };
                else
                {
                    method = type.GetMethod(_parseMethodName, _parseSignature);
                    if (method != null) parameters = e=> new Expression[] { e };
                    else return null;
                } 

                _parsers[type] = e => Expression.Call(method, parameters(e));
            }

            return _parsers[type];
        }

        public Expression Parse(Expression expression, Type target)
        {
            var parser = GetParser(target);
            if (parser != null)
                return parser(expression);
            return null;
        }
    }
}
