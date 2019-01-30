using Microsoft.Extensions.Options;
using QRest.Compiler.Standard.Assembler;
using QRest.Compiler.Standard.StringParsing;
using QRest.Core.Contracts;
using QRest.Core.Terms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard
{
    public class StandardCompiler : ICompiler
    {
        private static readonly ConstantsCollectingVisitor _constantsCollector = new ConstantsCollectingVisitor();
        private static readonly ConcurrentDictionary<string, ConstantExpression> _cache = new ConcurrentDictionary<string, ConstantExpression>();

        private readonly StandardCompilerOptions _options;
        private readonly IStringParsingBehavior _stringParsingBehavior;
        private readonly AssemblingVisitor _assembler;

        public StandardCompiler(IStringParsingBehavior stringParsingBehavior = null, IOptions<StandardCompilerOptions> options = null)
        {
            _options = options?.Value ?? new StandardCompilerOptions();
            _stringParsingBehavior = stringParsingBehavior ?? ParseStringsToClrTypes.Instance;

            _assembler = new AssemblingVisitor(_options.AllowUncompletedQueries);
        }

        public Func<TRoot, object> Compile<TRoot>(ITerm sequence)
        {
            var exp = Assemble<TRoot>(sequence);
            var compiled = exp.Compile();
            return (TRoot root) => compiled(root);
        }

        public Expression<Func<TRoot, object>> Assemble<TRoot>(ITerm rootTerm)
        {
            ConstantExpression compiled = null;
            IReadOnlyList<ConstantExpression> constants = null;

            var rootType = typeof(TRoot);

            var root = Expression.Parameter(rootType, "r");
            var cacheKey = $"{rootType.ToString()}++{rootTerm.KeyView}";

            if (_options.UseCompilerCache && _cache.TryGetValue(cacheKey, out var @delegate))
            {
                compiled = @delegate;
                constants = _constantsCollector.Collect(rootTerm);
            }
            else
            {
                var (lambda, consts) = _assembler.Assemble(rootTerm, root, typeof(object));

                constants = consts;
                compiled = Expression.Constant(lambda.Compile());

                if (_options.UseCompilerCache)
                    _cache[cacheKey] = compiled;
            }

            var resultInvokeParams = new Expression[] { root }.Concat(constants).ToArray();

            var topLambda = Expression.Lambda<Func<TRoot, object>>(Expression.Invoke(compiled, resultInvokeParams), root);

            return topLambda;
        }
    }
}
