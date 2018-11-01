using QRest.Core.Contracts;

namespace QRest.Core.Terms
{
    public class ConstantTerm : ITerm
    {
        public object Value { get; }

        public ConstantTerm(object value) => Value = value;

        public string DebugView => SharedView;
        public string SharedView => $"'{Value.ToString()}'";
        public string KeyView => $"[{Value.GetType()}]";
        public ITerm Clone() => new ConstantTerm(Value);
    }
}
