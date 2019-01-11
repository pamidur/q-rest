using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm.Csdl;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QRest.AspNetCore.OData
{
    public class ODataMetadataMiddleware : IMiddleware
    {
        private readonly IApiDescriptionGroupCollectionProvider _provider;
        private readonly ODataOptions _options;

        public ODataMetadataMiddleware(IApiDescriptionGroupCollectionProvider provider, IOptions<ODataOptions> options)
        {
            _provider = provider;
            _options = options.Value;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Response.Headers.Add("OData-Version", "4.0");

            if (IsMetadataRequest(context))
                return ApiMetadata(context);

            if (IsListRequest(context))
                return ApiList(context);

            return next(context);
        }

        private Task ApiList(HttpContext context)
        {
            throw new NotImplementedException();
        }

        private Task ApiMetadata(HttpContext context)
        {
            var edm = MetadataBuilder.New(context.Request.Host.ToString());

            foreach (var api in _provider.ApiDescriptionGroups.Items.SelectMany(i => i.Items).ToArray())
                edm.Map(api.ActionDescriptor.Parameters[0].ParameterType.GetGenericArguments()[0]);

            context.Response.ContentType = "application/xml";

            using (var xw = XmlWriter.Create(context.Response.Body, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
                CsdlWriter.TryWriteCsdl(edm.Build(), xw, CsdlTarget.OData, out var errors);


            return Task.CompletedTask;
        }

        private bool IsMetadataRequest(HttpContext context)
        {
            return context.Request.Method == "GET" && context.Request.Path == _options.MetadataPath + "/$metadata";
        }

        private bool IsListRequest(HttpContext context)
        {
            return context.Request.Method == "GET" && (
                context.Request.Path == _options.MetadataPath ||
                context.Request.Path == _options.MetadataPath + "/");
        }
    }
}