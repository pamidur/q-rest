namespace QRest.Core.Terms
{
    public class ContextTerm : ITerm
    {
        public bool IsRoot { get; }
        public bool IsContext { get; }
        public string Name { get; }

        public ContextTerm(bool isRoot = true)
        {
            IsRoot = isRoot;
            IsContext = !isRoot;
            Name = isRoot ? "$$" : "$";
        }

        public ContextTerm(string name)
        {
            Name = name;
        }

        public string DebugView => Name;
        public string SharedView => IsRoot ? "" : Name;
        public string KeyView => IsRoot ? "" : Name;

        public ITerm Clone() => IsContext || IsRoot ? new ContextTerm(IsRoot) : new ContextTerm(Name);
    }
}
