using QRest.Core.Compilation.Containers;
using QRest.Core.Compilation.TypeConverters;
using QRest.Core.Compilation.Visitors;
using QRest.Core.Terms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Compilation
{
    public class TermCompiler
    {
        public static TermCompiler Default { get; } =
            new TermCompiler(
                new AssemblingVisitor(
                    new EmitContainerFactory(),
                    new DefaultTypeConverter(CultureInfo.InvariantCulture, parseStrings: true, assumeDateTimeKind: DateTimeKind.Utc),
                    allowUncompletedQueries: false,
                    terminateSelects: true
                    ),
                new ConstantsCollectingVisitor(),
                useCompilerCache: true);

        private readonly ConcurrentDictionary<string, ConstantExpression> _cache = new ConcurrentDictionary<string, ConstantExpression>();

        private readonly AssemblingVisitor _assemblingVisitor;
        private readonly ConstantsCollectingVisitor _constantsCollectingVisitor;
        private readonly bool _useCompilerCache;

        public TermCompiler(AssemblingVisitor assemblingVisitor, ConstantsCollectingVisitor constantsCollectingVisitor, bool useCompilerCache = true)
        {
            _assemblingVisitor = assemblingVisitor;
            _constantsCollectingVisitor = constantsCollectingVisitor;
            _useCompilerCache = useCompilerCache;
        }

        public Func<TSource, TResult> Compile<TSource, TResult>(ITerm sequence)
        {
            var exp = Assemble<TSource, TResult>(sequence);
            var compiled = exp.Compile();
            return compiled;
        }

        public Expression<Func<TSource, TResult>> Assemble<TSource, TResult>(ITerm rootTerm)
        {
            var rootType = typeof(TSource);

            var root = Expression.Parameter(rootType, "r");
            var cacheKey = $"{rootType}++{rootTerm.ViewKey}";

            IReadOnlyList<ConstantExpression> constants;
            ConstantExpression compiled;
            if (_useCompilerCache && _cache.TryGetValue(cacheKey, out var @delegate))
            {
                compiled = @delegate;
                constants = _constantsCollectingVisitor.Collect(rootTerm);
            }
            else
            {
                var (lambda, consts) = _assemblingVisitor.Assemble(rootTerm, root, typeof(TResult));

                constants = consts.Select(c => c.Value).ToArray();
                compiled = Expression.Constant(lambda.Compile());

                if (_useCompilerCache)
                    _cache[cacheKey] = compiled;
            }

            var resultInvokeParams = new Expression[] { root }.Concat(constants).ToArray();

            var topLambda = Expression.Lambda<Func<TSource, TResult>>(Expression.Invoke(compiled, resultInvokeParams), root);

            return topLambda;
        }
    }
}
