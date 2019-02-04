using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using QRest.AspNetCore;
using QRest.AspNetCore.Contracts;
using QRest.Semantics.OData;
using QRest.Semantics.OData.Metadata;
using QRest.Semantics.OData.Semantics;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    class MyCtor
    {
        public ActionResult Metadata()
        {
            return new OkObjectResult(new { test = 1 });
        }
    }

    internal class ODataAppConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            application.ApiExplorer.IsVisible = true;
        }
    }

    public static class QRestExtensions
    {
        public static QRestConfiguration UseODataSemantics(this QRestConfiguration qrest, Action<ODataOptions> options = null)
        {
            qrest.Services.Configure<MvcOptions>(mvc => mvc.Conventions.Add(new ODataAppConvention()));

            if(options!=null)
            qrest.Services.Configure<ODataOptions>(options);

            qrest.Services.AddTransient<ISemantics, ODataSemantics>();
            qrest.Services.AddTransient<IEdmBuilder, ConventionalModelBuilder>();
            qrest.Services.AddSingleton<ODataMetadataMiddleware>();

            return qrest;
        }

        public static IApplicationBuilder UseODataMetadata(this IApplicationBuilder app)
        {
            var middleware = app.ApplicationServices.GetService<ODataMetadataMiddleware>();
            if (middleware != null)
            {
                middleware.IsInUse = true;
                app.UseMiddleware<ODataMetadataMiddleware>();
            }
            return app;
        }
    }
}
