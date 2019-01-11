using QRest.Core.Contracts;
using System.Linq;

namespace QRest.Core.Terms
{
    public class RootTerm : SequenceTerm
    {
        public RootTerm(params ITerm[] terms) : base(terms)
        {
            DebugView = $">{base.DebugView}";
            KeyView = $">{base.KeyView}";
            SharedView = $"{base.SharedView}";
        }

        public RootTerm(SequenceTerm sequence) : this(sequence.ToArray()) { }

        public override ITerm Clone() => new RootTerm(base.Clone().AsSequence());
    }
}
