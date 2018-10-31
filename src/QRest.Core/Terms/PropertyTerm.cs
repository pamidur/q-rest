using QRest.Core.Contracts;

namespace QRest.Core.Terms
{
    public class PropertyTerm : ITerm
    {
        public string Name { get; }

        public PropertyTerm(string name) => Name = name;

        public string SharedView => $".{Name}";
        public string DebugView => SharedView;
        public string KeyView => SharedView;

        public ITerm Clone => new PropertyTerm(Name);
    }
}
