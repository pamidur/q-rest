using Microsoft.Extensions.DependencyInjection;
using QRest.AspNetCore;
using QRest.AspNetCore.Contracts;
using QRest.AspNetCore.Native;
using QRest.Compiler.Standard;
using QRest.Core.Contracts;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QRestConfigurationExt
    {
        public static QRestConfiguration AddQRest(this IServiceCollection services)
        {
            return new QRestConfiguration(services);
        }

        public static QRestConfiguration UseNativeSemantics(this QRestConfiguration config, Action<NativeSemanticsOptions> options)
        {
            config.Services.Configure(options);
            config.Services.AddSingleton<ISemantics, NativeSemantics>();

            return config;
        }

        public static QRestConfiguration UseStandardCompiler(this QRestConfiguration config, Action<StandardCompilerOptions> options)
        {
            config.Services.Configure(options);
            config.Services.AddSingleton<ICompiler, StandardCompiler>();

            return config;
        }
    }
}

namespace QRest.AspNetCore
{
    public class QRestConfiguration
    {
        public IServiceCollection Services { get; }

        public QRestConfiguration(IServiceCollection services)
        {
            Services = services;
        }
    }
}
