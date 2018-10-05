using QRest.Core.Contracts;
using QRest.Core.Terms;

namespace QRest.Core
{
    public static class TermExtensions
    {
        public static ITermSequence AsSequence (this ITerm term)
        {
            return term as ITermSequence ?? new TermSequence { term };
        }
    }
}
