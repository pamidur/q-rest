using System.Collections.Generic;
using QRest.Core.Contracts;
using QRest.Core.Operations.Selectors;

namespace QRest.Core.Terms
{
    public class LambdaSequence: TermSequence
    {
        private static readonly IOperation _elementRoot = new ElementOperation();
        private readonly LinkedList<ITerm> _sequence = new LinkedList<ITerm>();

        public override IOperation RootSelector => _elementRoot;

        public override string DebugView => $":>{base.DebugView}";
        public override string KeyView => $":>{base.KeyView}";
        public override string SharedView => $":>{base.SharedView}";
    }
}
