using System.Linq;
using QRest.Core.Contracts;
using QRest.Core.Compiler;

namespace QRest.Core
{
    public abstract class QueryBase
    {
        public ITermSequence Sequence { get; set; }

        private static readonly TermTreeCompiler _compiler = new TermTreeCompiler();

        public object Apply<T>(IQueryable<T> target, bool finalize = true)
        {
            var lambda = _compiler.Compile<IQueryable<T>>(Sequence);

            var result = lambda.Compile()(target);

            //var debug = _compiler.CompileDebug<IQueryable<T>>(RootTerm);
            //debug.Func.Compile()(target);

            return result;
        }
    }
}
