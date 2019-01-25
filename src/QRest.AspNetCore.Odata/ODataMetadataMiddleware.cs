using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm.Csdl;
using Newtonsoft.Json;
using QRest.AspNetCore.OData.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QRest.AspNetCore.OData
{
    internal class ODataMetadataMiddleware : IMiddleware
    {
        private readonly ODataOptions _options;
        private readonly string _serviceRoot;
        private readonly Lazy<ODataModel> _model;

        public bool IsInUse { get; set; }

        public ODataMetadataMiddleware(IModelBuilder modelBuilder, IOptions<ODataOptions> options)
        {
            _options = options.Value;
            _serviceRoot = _options.ServiceRoot.ToString().Trim('/');
            _model = new Lazy<ODataModel>(modelBuilder.Build);
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
            var urlMap = _model.Value.Registry.Select(u => new { name = u.Value.Name, kind = u.Value.ContainerElementKind.ToString(), url = u.Key });
            var metaurl = GetMetaUrl(context);

            var result = new Dictionary<string, object>
            {
                { "@odata.context", metaurl },
                { "value", urlMap }
            };

            context.Response.ContentType = "application/json; odata.metadata=minimal; charset=utf-8";

            var ser = JsonSerializer.Create();

            using (var sw = new StreamWriter(context.Response.Body, Encoding.UTF8))
                ser.Serialize(sw, result);

            return Task.CompletedTask;
        }

        public string GetMetaUrl(HttpContext context)
        {
            var metaurl = $"{context.Request.Scheme}://{context.Request.Host}/{_serviceRoot}";
            metaurl = $"{metaurl.TrimEnd('/')}/$metadata";

            return metaurl;
        }

        private Task ApiMetadata(HttpContext context)
        {
            context.Response.ContentType = "application/xml; charset=utf-8";

            using (var xw = XmlWriter.Create(context.Response.Body, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
                CsdlWriter.TryWriteCsdl(_model.Value.Schema, xw, CsdlTarget.OData, out var errors);

            return Task.CompletedTask;
        }

        private bool IsMetadataRequest(HttpContext context)
        {
            return context.Request.Method == "GET" && context.Request.Path == _serviceRoot + "/$metadata";
        }

        private bool IsListRequest(HttpContext context)
        {
            return context.Request.Method == "GET" && (
                context.Request.Path == _serviceRoot ||
                context.Request.Path == _serviceRoot + "/");
        }
    }
}