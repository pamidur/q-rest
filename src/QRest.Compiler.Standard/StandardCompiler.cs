using QRest.Core.Contracts;
using QRest.Core.Terms;
using System;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard
{
    public class StandardCompiler : ICompiler
    {
        public Func<TRoot, object> Compile<TRoot>(TermSequence sequence, bool finalize = true)
        {
            var exp = Assemble<TRoot, TRoot>(sequence, finalize);
            var compiled = exp.Compile();
            return (TRoot root) => compiled(root, root);
        }

        public Expression<Func<TRoot, object>> Assemble<TRoot>(TermSequence sequence, bool finalize = true)
        {
            var rootParam = Expression.Parameter(typeof(TRoot), "root");
            return Expression.Lambda<Func<TRoot, object>>(Expression.Invoke(Assemble<TRoot, TRoot>(sequence, finalize), rootParam, rootParam), rootParam);
        }

        public Expression<Func<TRoot, TContext, object>> Assemble<TRoot, TContext>(TermSequence sequence, bool finalize = true)
        {
            var expression = Assemble(sequence, typeof(TRoot), typeof(TContext), finalize);

            return Expression.Lambda<Func<TRoot, TContext, object>>(Expression.Convert(expression.Body, typeof(object)), expression.Parameters);
        }

        public LambdaExpression Assemble(TermSequence sequence, Type root, Type context, bool finalize = true)
        {
            var ctx = new StandardCompilerContext(finalize);
            var expression = ctx.Assemble(sequence, root, context);
            return expression;
        }
    }
}
