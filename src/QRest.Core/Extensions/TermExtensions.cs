using QRest.Core.Contracts;
using QRest.Core.Terms;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Core
{
    public static class TermExtensions
    {
        public static SequenceTerm AsSequence(this ITerm term)
        {
            return term as SequenceTerm ?? new SequenceTerm(new[] { term });
        }

        public static SequenceTerm AsSequence(this IEnumerable<ITerm> terms)
        {
            return new SequenceTerm(terms.ToArray());
        }
    }
}
