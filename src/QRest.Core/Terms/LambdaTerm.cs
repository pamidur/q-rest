using QRest.Core.Contracts;
using System.Linq;

namespace QRest.Core.Terms
{
    public class LambdaTerm : SequenceTerm
    {
        public IRootProvider RootProvider { get; }

        public LambdaTerm(IRootProvider rootProvider, params ITerm[] terms) : base(terms)
        {
            RootProvider = rootProvider;

            DebugView = $"|{RootProvider.Key}>{base.DebugView}";
            KeyView = $"|{RootProvider.Key}>{base.KeyView}";
            SharedView = $"|{RootProvider.Key}>{base.SharedView}";
        }

        public LambdaTerm(IRootProvider rootProvider, SequenceTerm sequence) : this(rootProvider, sequence.ToArray()) { }

        public override string DebugView { get; }
        public override string KeyView { get; }
        public override string SharedView { get; }
        public override ITerm Clone() => new LambdaTerm(RootProvider, base.Clone().AsSequence());
    }
}
