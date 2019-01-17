using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using QRest.AspNetCore;
using QRest.AspNetCore.Contracts;
using QRest.AspNetCore.OData;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    internal class ODataAppConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            application.ApiExplorer.IsVisible = true;
        }
    }

    public static class QRestExtensions
    {
        public static QRestConfiguration UseODataSemantics(this QRestConfiguration qrest, Action<ODataOptions> options)
        {
            qrest.Services.Configure<MvcOptions>(mvc => mvc.Conventions.Add(new ODataAppConvention()));
            qrest.Services.Configure<ODataOptions>(options);

            qrest.Services.AddTransient<ISemantics, ODataSemantics>();
            qrest.Services.AddSingleton<ODataMetadataMiddleware>();

            return qrest;
        }

        public static IApplicationBuilder UseODataMetadata(this IApplicationBuilder app)
        {
            app.UseMiddleware<ODataMetadataMiddleware>();
            return app;
        }
    }
}
