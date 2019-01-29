using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using QRest.AspNetCore;
using QRest.AspNetCore.Contracts;
using QRest.AspNetCore.OData;
using QRest.AspNetCore.OData.Metadata;
using System;
using System.Reflection;

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
            //var c = new ControllerModel(typeof(MyCtor).GetTypeInfo(), new object[] { });
            //var action = new ActionModel(typeof(MyCtor).GetMethod(nameof(MyCtor.Metadata)),new object[] { });

            //c.Actions.Add(new ActionModel(action));


            //application.Controllers.Add(c);

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
            qrest.Services.AddTransient<IModelBuilder, ConventionalModelBuilder>();
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
