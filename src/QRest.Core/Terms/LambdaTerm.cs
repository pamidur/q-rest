using QRest.Core.Contracts;

namespace QRest.Core.Terms
{
    public class LambdaTerm : SequenceTerm
    {
        public IRootProvider RootProvider { get; }

        public LambdaTerm(IRootProvider rootProvider) => RootProvider = rootProvider;

        public override string DebugView => $"|{RootProvider.Key}>{base.DebugView}";
        public override string KeyView => $"|{RootProvider.Key}>{base.KeyView}";
        public override string SharedView => $"|{RootProvider.Key}>{base.SharedView}";
        public override ITerm Clone() => new LambdaTerm(RootProvider) { Root.Clone() };
    }
}
