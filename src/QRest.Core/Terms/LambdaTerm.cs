using QRest.Core.Contracts;

namespace QRest.Core.Terms
{
    public class LambdaTerm : SequenceTerm
    {
        public LambdaTerm()
        {

        }

        public override string DebugView => $":>{base.DebugView}";
        public override string KeyView => $":>{base.KeyView}";
        public override string SharedView => $":>{base.SharedView}";
        public override ITerm Clone => new SequenceTerm { Root.Clone };
    }
}
