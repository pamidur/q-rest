using System.ComponentModel;

namespace QRest.Core.Terms
{
    public sealed class ConstantTerm : ITerm
    {
        public object Value { get; }

        public ConstantTerm(object value)
        {
            Value = value;

            SharedView = $"'{new TypeConverter().ConvertToInvariantString(Value)}'";
            KeyView = $"[{Value.GetType()}]";
            DebugView = SharedView;
        }

        public string DebugView { get; }
        public string SharedView { get; }
        public string KeyView { get; }
        public ITerm Clone() => new ConstantTerm(Value);
    }
}
