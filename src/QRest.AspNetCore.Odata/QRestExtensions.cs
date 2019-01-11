using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using QRest.AspNetCore.Contracts;
using QRest.AspNetCore.OData;
using QRest.Semantics.OData;

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
        public static IServiceCollection AddOData(this IServiceCollection services, PathString path)
        {
            services.Configure<MvcOptions>(mvc => mvc.Conventions.Add(new ODataAppConvention()));
            services.Configure<ODataOptions>(odata => odata.MetadataPath = path);

            services.AddTransient<ISemantics, ODataSemantics>();
            services.AddSingleton<ODataMetadataMiddleware>();

            return services;
        }

        public static IApplicationBuilder UseODataMetadata(this IApplicationBuilder app)
        {
            app.UseMiddleware<ODataMetadataMiddleware>();
            return app;
        }
    }
}
