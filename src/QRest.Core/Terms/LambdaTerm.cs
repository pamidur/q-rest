namespace QRest.Core.Terms
{
    public sealed class LambdaTerm : ITerm
    {
        public LambdaTerm(ITerm term)
        {            
            Term = term;

            ViewDebug = $":{term.ViewDebug}";
            ViewKey = $":{term.ViewKey}";
            ViewQuery = $":{term.ViewQuery}";
        }

        public ITerm Term { get; }

        public string ViewDebug { get; }
        public string ViewQuery { get; }
        public string ViewKey { get; }
        public ITerm Clone() => new LambdaTerm(Term.Clone());
    }
}
