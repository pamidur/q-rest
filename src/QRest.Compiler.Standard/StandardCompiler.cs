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
        private static readonly Dictionary<string, Delegate> _cache = new Dictionary<string, Delegate>();

        public Func<TRoot, object> Compile<TRoot>(LambdaTerm sequence)
        {
            var exp = Assemble<TRoot>(sequence);
            var compiled = exp.Compile();
            return (TRoot root) => compiled(root);
        }

        public Expression<Func<TRoot, object>> Assemble<TRoot>(LambdaTerm lambdaterm)
        {
            Delegate compiled = null;
            IReadOnlyList<ConstantExpression> constants = null;

            var root = Expression.Parameter(typeof(TRoot), "r");

            if (UseCompilerCache && _cache.TryGetValue(lambdaterm.KeyView, out var @delegate))
            {
                compiled = @delegate;
                constants = _constantsCollector.Collect(lambdaterm);
            }
            else
            {
                var ctx = new StandardAssembler(this);
                var (lambda, consts) = ctx.Assemble(lambdaterm, root);

                constants = consts;
                compiled = lambda.Compile();

                if (UseCompilerCache)
                    _cache[lambdaterm.KeyView] = compiled;
            }            

            var resultInvokeParams = new Expression[] { root }.Concat(constants).ToArray();
            
            var topLambda = Expression.Lambda<Func<TRoot, object>>(Expression.Convert(Expression.Invoke(Expression.Constant(compiled), resultInvokeParams), typeof(object)), root);

            return topLambda;
        }
    }
}
