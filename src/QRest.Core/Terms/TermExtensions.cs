using System.Buffers;

namespace QRest.Core.Terms
{
    public static class TermExtensions
    {
        private static readonly ArrayPool<ITerm> _pool = ArrayPool<ITerm>.Shared;
        public static SequenceTerm Chain(this ITerm term, params ITerm[] terms)
        {
            var sq = term as SequenceTerm;

            var buffer = _pool.Rent(sq != null ? sq.Count : 1 + terms.Length);

            var index = 0;

            if (sq != null)
            {
                for (int i = 0; i < sq.Count; i++)
                    buffer[index + i] = sq[i];
                index = sq.Count;
            }
            else
            {
                buffer[index] = term;
                index++;
            }

            for (int i = 0; i < terms.Length; i++)
                buffer[index + i] = terms[i];

            var result = new SequenceTerm(buffer);

            _pool.Return(buffer, clearArray: true);

            return result;
        }
    }
}
