using QRest.Core.Contracts;
using QRest.Core.Terms;

namespace QRest.Core
{
    public static class TermExtensions
    {
        public static SequenceTerm AsSequence (this ITerm term)
        {
            return term as SequenceTerm ?? new SequenceTerm { term };
        }
    }
}
