using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace QRest.Core.Compilation.TypeConverters
{
    public class DefaultTypeConverter : ITypeConverter
    {
        private readonly IFormatProvider _format;
        private readonly bool _parseStrings;
        private readonly Dictionary<(Type From, Type To), Func<Expression, IFormatProvider, Expression>> _converters = new Dictionary<(Type From, Type To), Func<Expression, IFormatProvider, Expression>>();

        public DefaultTypeConverter(IFormatProvider format, bool parseStrings = true, DateTimeKind assumeDateTimeKind = DateTimeKind.Utc)
        {
            _format = format;
            _parseStrings = parseStrings;

            var dateTimeMethod = assumeDateTimeKind == DateTimeKind.Local ? _dateTimeAssumeLocal : _dateTimeAssumeUtc;
            _converters[(typeof(DateTime), typeof(DateTime))] = (e, f) => Expression.Convert(e, typeof(DateTime), dateTimeMethod);
            RegisterConverter<string, DateTime>((s, f) => ParseDate(s, f, assumeDateTimeKind));
        }

        public bool TryConvert(Expression expression, Type target, out Expression result)
        {
            result = null;
            var key = (expression.Type, target);

            if (_converters.TryGetValue(key, out var converter))            
                result = converter(expression, _format);            
            else if(expression.Type == target)            
                result = expression;                       
            else if (target.IsAssignableFrom(expression.Type))            
                result = Expression.Convert(expression, target);            
            else if (expression.Type == typeof(string) && _parseStrings)
            {
                var parser = StringParser.GetParser(target);
                if (parser != null)
                {
                    _converters[key] = parser;
                    result = parser(expression, _format);
                }
            }            
            
            return result != null;
        }

        public void RegisterConverter<TI, TO>(Func<TI, IFormatProvider, TO> converter)
        {
            _converters[(typeof(TI), typeof(TO))] = (e, f) => Expression.Invoke(Expression.Constant(converter), e, Expression.Constant(f, typeof(IFormatProvider)));
        }

        public static DateTime DateTimeToUtc(DateTime dateTime, DateTimeKind assumeDateTimeKind)
        {
            if (dateTime.Kind == DateTimeKind.Local)
                return dateTime.ToUniversalTime();

            if (dateTime.Kind == DateTimeKind.Unspecified)
                return DateTimeToUtc(DateTime.SpecifyKind(dateTime, assumeDateTimeKind), assumeDateTimeKind);

            return dateTime;
        }

        private static readonly MethodInfo _dateTimeAssumeUtc = typeof(DefaultTypeConverter).GetMethod(nameof(DateTimeAssumeUtc));
        private static readonly MethodInfo _dateTimeAssumeLocal = typeof(DefaultTypeConverter).GetMethod(nameof(DateTimeAssumeLocal));

        public static DateTime DateTimeAssumeUtc(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Local)
                return dateTime.ToUniversalTime();

            if (dateTime.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            return dateTime;
        }

        public static DateTime DateTimeAssumeLocal(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Local)
                return dateTime.ToUniversalTime();

            if (dateTime.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();

            return dateTime;
        }

        private static DateTime ParseDate(string s, IFormatProvider f, DateTimeKind assumeDateTimeKind)
        {
            var dt = DateTime.Parse(s, f);

            if (dt.Kind == DateTimeKind.Unspecified)
                dt = DateTime.SpecifyKind(dt, assumeDateTimeKind);

            if (dt.Kind == DateTimeKind.Local)
                dt = dt.ToUniversalTime();

            return dt;
        }
    }
}
