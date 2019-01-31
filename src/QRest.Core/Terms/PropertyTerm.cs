using QRest.Core.Contracts;

namespace QRest.Core.Terms
{
    public sealed class PropertyTerm : ITerm
    {
        public string Name { get; }

        public PropertyTerm(string name)
        {
            Name = name;

            SharedView = $".{Name}";
            DebugView = SharedView;
            KeyView = SharedView;
        }

        public string SharedView { get; }
        public string DebugView { get; }
        public string KeyView { get; }

        public ITerm Clone() => new PropertyTerm(Name);
    }
}
