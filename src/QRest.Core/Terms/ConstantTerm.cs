using QRest.Core.Contracts;

namespace QRest.Core.Terms
{
    public class ConstantTerm : ITerm
    {
        public object Value { get; }

        public ConstantTerm(object value)
        {
            Value = value;

            SharedView = $"'{Value.ToString()}'";
            KeyView = $"[{Value.GetType()}]";
            DebugView = SharedView;
        }

        public string DebugView { get; }
        public string SharedView { get; }
        public string KeyView { get; }
        public ITerm Clone() => new ConstantTerm(Value);
    }
}
