namespace QRest.Core.Terms
{
    public sealed class ContextTerm : ITerm
    {
        public static ContextTerm Root => new ContextTerm();
        public static ContextTerm Result => new ContextTerm("");
        public bool IsRoot { get; }
        public bool IsResult { get; }
        public string Name { get; }

        public ContextTerm() : this("$") { }

        public ContextTerm(string name)
        {
            if (string.IsNullOrEmpty(name))
                IsResult = true;
            if (name == "$")
                IsRoot = true;

            Name = name;
            ViewDebug = "$" + name;
        }

        public string ViewDebug { get; }
        public string ViewQuery => ViewDebug;
        public string ViewKey => ViewDebug;

        public ITerm Clone() => new ContextTerm(Name);
    }
}
