using System.Linq;
using QRest.Core.Contracts;
using QRest.Core.Terms;

namespace QRest.Core
{
    public abstract class QueryBase
    {
        private readonly ICompiler _compiller;

        public TermSequence Sequence { get; }

        protected QueryBase(TermSequence sequence, ICompiler compiller)
        {
            Sequence = sequence;
            _compiller = compiller;
        }

        public object Apply<T>(IQueryable<T> target, bool finalize = true)
        {
            var lambda = _compiller.Compile<IQueryable<T>>(Sequence);
            var result = lambda(target);
            return result;
        }
    }
}
