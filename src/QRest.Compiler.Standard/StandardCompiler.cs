using QRest.Core.Contracts;
using QRest.Core.Terms;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard
{
    public class StandardCompiler : ICompiler
    {
        private readonly StandardCompillerOptions _options;

        public StandardCompiler(StandardCompillerOptions options = null)
        {
            _options = options ?? new StandardCompillerOptions();
        }

        public Func<TRoot, object> Compile<TRoot>(LambdaTerm sequence, bool finalize = true)
        {
            var exp = Assemble<TRoot>(sequence, finalize);
            var compiled = exp.Compile();
            return (TRoot root) => compiled(root);
        }

        public Expression<Func<TRoot, object>> Assemble<TRoot>(LambdaTerm lambda, bool finalize = true)
        {
            var ctx = new StandardAssembler(_options);

            var result = ctx.Assemble(lambda, Expression.Parameter(typeof(TRoot), "r"), finalize: finalize);
            var root = result.Lambda.Parameters[0];

            var resultInvokeParams = new[] { root }.Concat<Expression>(result.Constants).ToArray();
            var topLambda = Expression.Lambda<Func<TRoot, object>>(Expression.Convert(Expression.Invoke(result.Lambda, resultInvokeParams), typeof(object)), root);

            return topLambda;
        }


    }
}
