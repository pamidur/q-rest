using QRest.Core.Compiler.Debug;
using QRest.Core.Contracts;
using System;
using System.Linq.Expressions;

namespace QRest.Core.Compiler
{
    public class TermTreeCompiler
    {
        public Expression<Func<TRoot, object>> Compile<TRoot>(ITerm term, bool finalize = true)
        {
            var dataParam = Expression.Parameter(typeof(TRoot));
            var expression = Compile(term, dataParam, dataParam, finalize);

            var lambda = Expression.Lambda<Func<TRoot, object>>(Expression.Convert(expression, typeof(object)), dataParam);
            return lambda;
        }

        public DebugResult<TRoot> CompileDebug<TRoot>(ITerm term, bool finalize = true)
        {
            var dataParam = Expression.Parameter(typeof(TRoot));
            var result = CompileDebug(term, dataParam, dataParam, finalize);

            var lambda = Expression.Lambda<Func<TRoot, object>>(Expression.Convert(result.Expression,typeof(object)), dataParam);
            return new DebugResult<TRoot>
            {
                Root = result.Root,
                DebugView = result.DebugView,
                Expression = result.Expression,
                Func = lambda
            };
        }

        public DebugResult CompileDebug(ITerm term, Expression context, ParameterExpression root, bool finalize = true)
        {
            var ctx = new DebugCompillerContext(term, finalize);
            ctx.DebugResult.Expression = ctx.Compile(term, context, root);
            return ctx.DebugResult;
        }

        public Expression Compile(ITerm term, Expression context, ParameterExpression root, bool finalize = true)
        {
            var ctx = new DefaultCompilerContext(finalize);
            var expression = ctx.Compile(term, context, root);
            return expression;
        }
    }
}
