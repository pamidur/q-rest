using QRest.Core.Contracts;

namespace QRest.Core.Terms
{
    public class NameTerm : ITerm
    {
        public string Name { get; }

        public NameTerm(string name) => Name = name;

        public string SharedView => $"@{Name}";
        public string DebugView => SharedView;
        public string KeyView => SharedView;

        public ITerm Clone => new NameTerm(Name);
    }
}
