using Microsoft.Extensions.Options;
using QRest.AspNetCore.Contracts;
using QRest.Core.Compilation;
using QRest.Core.Compilation.Containers;
using QRest.Core.Compilation.TypeConverters;
using QRest.Core.Compilation.Visitors;
using QRest.Core.Terms;
using System;

namespace QRest.AspNetCore.Native
{
    public class NativeCompiler : ICompiler
    {
        private readonly NativeCompilerOptions _options;
        private readonly TermCompiler _compiler;

        public NativeCompiler(IOptions<NativeCompilerOptions> options = null)
        {
            _options = options?.Value ?? new NativeCompilerOptions();

            var converter = new DefaultTypeConverter(_options.CultureInfo, true, _options.AssumeDateTimeKind);

            _compiler = new TermCompiler(
                new AssemblingVisitor(new EmitContainerFactory(), converter, allowUncompletedQueries: false, terminateSelects: true),
                new ConstantsCollectingVisitor(),
                useCompilerCache: _options.UseCompilerCache);
        }

        public Func<TRoot, object> Compile<TRoot>(ITerm sequence)
        {
            return _compiler.Compile<TRoot>(sequence);
        }
    }
}
