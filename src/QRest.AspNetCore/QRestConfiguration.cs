using Microsoft.Extensions.DependencyInjection;
using QRest.AspNetCore;
using QRest.AspNetCore.Contracts;
using QRest.AspNetCore.Native;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QRestConfigurationExt
    {
        public static QRestConfiguration AddQRest(this IServiceCollection services)
        {
            return new QRestConfiguration(services);
        }

        public static QRestConfiguration UseNativeSemantics(this QRestConfiguration config, Action<NativeSemanticsOptions> options = null)
        {
            if (options != null)
                config.Services.Configure(options);

            config.Services.AddSingleton<ISemantics, NativeSemantics>();

            return config;
        }

        public static QRestConfiguration UseStandardCompiler(this QRestConfiguration config, Action<NativeCompilerOptions> options = null)
        {
            if (options != null)
                config.Services.Configure(options);

            config.Services.AddSingleton<ICompiler, NativeCompiler>();

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
