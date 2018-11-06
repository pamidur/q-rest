using QRest.Core.Contracts;
using System.Collections.Generic;

namespace QRest.Core.Terms
{
    public class LambdaTerm : SequenceTerm
    {
        public IRootProvider RootProvider { get; }

        public LambdaTerm(IRootProvider rootProvider, IReadOnlyList<ITerm> terms) : base(terms) => RootProvider = rootProvider;
        public LambdaTerm(IRootProvider rootProvider, params ITerm[] terms) : base(terms) => RootProvider = rootProvider;
        public LambdaTerm(IRootProvider rootProvider, SequenceTerm sequence) : base(sequence) => RootProvider = rootProvider;

        public override string DebugView => $"|{RootProvider.Key}>{base.DebugView}";
        public override string KeyView => $"|{RootProvider.Key}>{base.KeyView}";
        public override string SharedView => $"|{RootProvider.Key}>{base.SharedView}";
        public override ITerm Clone() => new LambdaTerm(RootProvider, base.Clone().AsSequence());
    }
}
