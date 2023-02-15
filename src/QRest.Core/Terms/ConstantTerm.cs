using System.ComponentModel;

namespace QRest.Core.Terms
{
    public sealed class ConstantTerm : ITerm
    {
        private static readonly TypeConverter _converter = new TypeConverter();
        public object Value { get; }

        public ConstantTerm(object value)
        {
            Value = value;

            ViewQuery = GetValueRepresentation();
            ViewKey = $"[{Value?.GetType() ?? typeof(object)}]";
            ViewDebug = ViewQuery;
        }

        public string ViewDebug { get; }
        public string ViewQuery { get; }
        public string ViewKey { get; }
        public ITerm Clone() => new ConstantTerm(Value);

        private string GetValueRepresentation()
        {
            if (Value == null) return "null";
            if (Value is bool b) return b.ToString().ToLowerInvariant();

            return $"'{_converter.ConvertToInvariantString(Value)}'";
        }
    }
}
