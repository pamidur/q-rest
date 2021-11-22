using QRest.Core.Compilation;
using QRest.Core.Terms;
using System.Threading.Tasks;

namespace QRest.Core.Linq
{
    public class LocalTermExecutor<TSource> : ITermExecutor
    {
        private readonly TSource _data;

        public LocalTermExecutor(TSource data)
        {
            _data = data;
        }
        public Task<TResult> Execute<TResult>(ITerm term)
        {
            var result = TermCompiler.Default.Compile<TSource, TResult>(term)(_data);
            return Task.FromResult(result);
        }
    }
}
