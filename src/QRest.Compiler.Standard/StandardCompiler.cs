using QRest.Compiler.Standard.Assembler;
using QRest.Core.Contracts;
using QRest.Core.Terms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard
{
    public partial class StandardCompiler : ICompiler
    {
        public bool UseCompilerCache { get; set; } = true;
        private static readonly ConstantsCollector _constantsCollector = new ConstantsCollector();
        private static readonly ConcurrentDictionary<string, ConstantExpression> _cache = new ConcurrentDictionary<string, ConstantExpression>();

        public Func<TRoot, object> Compile<TRoot>(RootTerm sequence)
        {
            var exp = Assemble<TRoot>(sequence);
            var compiled = exp.Compile();
            return (TRoot root) => compiled(root);
        }

        public Expression<Func<TRoot, object>> Assemble<TRoot>(RootTerm rootTerm)
        {
            ConstantExpression compiled = null;
            IReadOnlyList<ConstantExpression> constants = null;

            var rootType = typeof(TRoot);

            var root = Expression.Parameter(rootType, "r");
            var cacheKey = $"{rootType.ToString()}++{rootTerm.KeyView}";

            if (UseCompilerCache && _cache.TryGetValue(cacheKey, out var @delegate))
            {
                compiled = @delegate;
                constants = _constantsCollector.Collect(rootTerm);
            }
            else
            {
                var ctx = new StandardAssembler(this);
                var (lambda, consts) = ctx.Assemble(rootTerm, root, typeof(object));

                constants = consts;
                compiled = Expression.Constant(lambda.Compile());

                if (UseCompilerCache)
                    _cache[cacheKey] = compiled;
            }

            var resultInvokeParams = new Expression[] { root }.Concat(constants).ToArray();

            var topLambda = Expression.Lambda<Func<TRoot, object>>(Expression.Invoke(compiled, resultInvokeParams), root);

            return topLambda;
        }
    }
}
