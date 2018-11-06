using QRest.Compiler.Standard.Assembler;
using QRest.Core.Contracts;
using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard
{
    public partial class StandardCompiler : ICompiler
    {
        public bool UseCompilerCache { get; set; } = true;
        private static readonly ConstantsCollector _constantsCollector = new ConstantsCollector();
        private static readonly Dictionary<string, LambdaExpression> _cache = new Dictionary<string, LambdaExpression>();

        public Func<TRoot, object> Compile<TRoot>(LambdaTerm sequence)
        {
            var exp = Assemble<TRoot>(sequence);
            var compiled = exp.Compile();
            return (TRoot root) => compiled(root);
        }

        public Expression<Func<TRoot, object>> Assemble<TRoot>(LambdaTerm lambdaterm)
        {
            LambdaExpression lambda = null;
            IReadOnlyList<ConstantExpression> constants = null;

            if (UseCompilerCache && _cache.TryGetValue(lambdaterm.KeyView, out var expression))
            {
                lambda = expression;
                constants = _constantsCollector.Collect(lambdaterm);
            }
            else
            {
                var ctx = new StandardAssembler(this);
                (lambda, constants) = ctx.Assemble(lambdaterm, Expression.Parameter(typeof(TRoot), "r"));

                if (UseCompilerCache)
                    _cache[lambdaterm.KeyView] = lambda;
            }

            var root = lambda.Parameters[0];

            var deleg = lambda.Compile();

            var resultInvokeParams = new[] { root }.Concat<Expression>(constants).ToArray();
            var topLambda = Expression.Lambda<Func<TRoot, object>>(Expression.Convert(Expression.Invoke(lambda, resultInvokeParams), typeof(object)), root);

            return topLambda;
        }
    }
}
