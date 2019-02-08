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

            SharedView = GetValueRepresentation();
            KeyView = $"[{Value?.GetType() ?? typeof(object)}]";
            DebugView = SharedView;
        }

        public string DebugView { get; }
        public string SharedView { get; }
        public string KeyView { get; }
        public ITerm Clone() => new ConstantTerm(Value);

        private string GetValueRepresentation()
        {
            if (Value == null) return "null";
            if (Value is bool b) return b.ToString().ToLowerInvariant();

            return $"'{_converter.ConvertToInvariantString(Value)}'";
        }
    }
}
