using QRest.Core.Contracts;
using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard
{
    public class StandardCompiler : ICompiler
    {
        public Func<TRoot, object> Compile<TRoot>(SequenceTerm sequence, bool finalize = true)
        {
            var exp = Assemble<TRoot>(sequence, finalize);
            var compiled = exp.Compile();
            return (TRoot root) => compiled(root);
        }

        public Expression<Func<TRoot, object>> Assemble<TRoot>(SequenceTerm sequence, bool finalize = true)
        {
            var ctx = new StandardCompilerContext(finalize);

            var root = Expression.Parameter(typeof(TRoot), "r");
            var result = ctx.Assemble(sequence, root, finalize: finalize);

            var resultInvokeParams = new[] { root }.Concat<Expression>(result.Constants).ToArray();
            var topLambda = Expression.Lambda<Func<TRoot, object>>(Expression.Convert(Expression.Invoke(result.Lambda, resultInvokeParams), typeof(object)), root);

            return topLambda;
        }

        
    }
}
