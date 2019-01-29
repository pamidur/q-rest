using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.StringParsing
{
    public class ParseStringsToClrTypes : IStringParsingBehavior
    {
        public static readonly IStringParsingBehavior Instance = new ParseStringsToClrTypes();

        private static readonly string _parseMethodName = "Parse";
        private static readonly Type[] _parseWithFormatSignature = new[] { typeof(string), typeof(IFormatProvider) };
        private static readonly Type[] _parseSignature = new[] { typeof(string) };

        private readonly Dictionary<Type, Func<Expression, IFormatProvider, Expression>> _parsers = new Dictionary<Type, Func<Expression, IFormatProvider, Expression>>() {
            //{ typeof(DateTime),(e,c)=>Expression.Call(typeof(Parsers), "ParseDateTime",new Type[]{ }, e,Expression.Constant(c, typeof(IFormatProvider)))}
        };

        public ParseStringsToClrTypes()
        {
            AddParser(Parsers.ParseDateTime);
        }

        public CultureInfo ParsingCulture { get; set; } = CultureInfo.InvariantCulture;

        public void AddParser<T>(Func<string, IFormatProvider, T> parser)
        {
            _parsers[typeof(T)] = (e, f) => Expression.Invoke(Expression.Constant(parser), e, Expression.Constant(f, typeof(IFormatProvider)));
        }

        public Func<Expression, CultureInfo, Expression> GetParser(Type type)
        {
            if (!_parsers.ContainsKey(type))
            {
                var source = Expression.Parameter(typeof(string));

                Func<Expression, IFormatProvider, Expression[]> parameters;

                var method = type.GetMethod(_parseMethodName, _parseWithFormatSignature);
                if (method != null) parameters = (e, c) => new Expression[] { e, Expression.Constant(c, typeof(IFormatProvider)) };
                else
                {
                    method = type.GetMethod(_parseMethodName, _parseSignature);
                    if (method != null) parameters = (e, c) => new Expression[] { e };
                    else return null;
                }

                _parsers[type] = (e, c) => Expression.Call(method, parameters(e, c));
            }

            return _parsers[type];
        }

        public Expression Parse(Expression expression, Type target)
        {
            var parser = GetParser(target);
            if (parser != null)
                return parser(expression, ParsingCulture);
            return null;
        }
    }
}
