using System.Linq;
using QRest.Core.Contracts;
using QRest.Core.Terms;

namespace QRest.Core
{
    public abstract class QueryBase
    {
        private readonly ICompiler _compiller;

        public LambdaTerm Lambda { get; }

        protected QueryBase(LambdaTerm lambda, ICompiler compiller)
        {
            Lambda = lambda;
            _compiller = compiller;
        }

        public object Apply<T>(IQueryable<T> target, bool finalize = true)
        {
            var lambda = _compiller.Compile<IQueryable<T>>(Lambda);
            var result = lambda(target);
            return result;
        }
    }
}
