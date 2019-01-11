using QRest.Core.Contracts;
using System.Linq;

namespace QRest.Core.Terms
{
    public class LambdaTerm : SequenceTerm
    {
        public LambdaTerm(params ITerm[] terms) : base(terms)
        {
            DebugView = $":{base.DebugView}";
            KeyView = $":{base.KeyView}";
            SharedView = $":{base.SharedView}";
        }

        public LambdaTerm(SequenceTerm sequence) : this(sequence.ToArray()) { }

        public override ITerm Clone() => new LambdaTerm(base.Clone().AsSequence());
    }
}
