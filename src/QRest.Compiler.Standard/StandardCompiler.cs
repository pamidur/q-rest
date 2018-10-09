using QRest.Core.Contracts;
using System;
using System.Linq.Expressions;

namespace QRest.Compiller.Standard
{
    public class StandardCompiler : ICompiler
    {
        public Expression<Func<TRoot, object>> CompileRaw<TRoot>(ITermSequence sequence, bool finalize = true)
        {
            var dataParam = Expression.Parameter(typeof(TRoot));
            var expression = Compile(sequence, dataParam, dataParam, finalize);

            var lambda = Expression.Lambda<Func<TRoot, object>>(Expression.Convert(expression, typeof(object)), dataParam);
            return lambda;
        }

        public Func<TRoot, object> Compile<TRoot>(ITermSequence sequence, bool finalize = true)
        {
            var exp = CompileRaw<TRoot>(sequence, finalize);
            return exp.Compile();
        }

        public Expression Compile(ITermSequence sequence, Expression context, ParameterExpression root, bool finalize = true)
        {
            var ctx = new StandardCompilerContext(finalize);
            var expression = ctx.Assemble(sequence, context, root);
            return expression;
        }
    }
}
