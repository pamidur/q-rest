using QRest.Core.Contracts;
using QRest.Core.Terms;

namespace QRest.Core
{
    public static class TermExtensions
    {
        public static TermSequence AsSequence (this ITerm term)
        {
            return term as TermSequence ?? new TermSequence { term };
        }
    }
}
