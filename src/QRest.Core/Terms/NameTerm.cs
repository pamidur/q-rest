namespace QRest.Core.Terms
{
    public sealed class NameTerm : ITerm
    {
        public string Name { get; }

        public NameTerm(string name)
        {
            Name = name;

            ViewQuery = $"@{Name}";
            ViewDebug = ViewQuery;
            ViewKey = ViewQuery;
        }

        public string ViewQuery { get; }
        public string ViewDebug { get; }
        public string ViewKey { get; }

        public ITerm Clone() => new NameTerm(Name);
    }
}
