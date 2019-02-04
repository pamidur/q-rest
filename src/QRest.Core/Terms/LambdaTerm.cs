namespace QRest.Core.Terms
{
    public sealed class LambdaTerm : ITerm
    {
        public LambdaTerm(ITerm term)
        {            
            Term = term;

            DebugView = $":{term.DebugView}";
            KeyView = $":{term.KeyView}";
            SharedView = $":{term.SharedView}";
        }

        public ITerm Term { get; }

        public string DebugView { get; }
        public string SharedView { get; }
        public string KeyView { get; }
        public ITerm Clone() => new LambdaTerm(Term.Clone());
    }
}
