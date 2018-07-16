using Microsoft.AspNetCore.Mvc.ModelBinding;
using QRest.AspNetCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QRestOptionsExtensions
    {
        public static IMvcBuilder AddQRestOptions(this IMvcBuilder builder, Action<QRestOptions> setupAction)
        {
            if (builder == null)            
                throw new ArgumentNullException(nameof(builder));
            
            if (setupAction == null)            
                throw new ArgumentNullException(nameof(setupAction));

            builder.Services.AddSingleton(typeof(QueryModelBinder));
            builder.Services.Configure(setupAction);            
            return builder;
        }        
    }
}
