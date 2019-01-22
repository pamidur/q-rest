using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
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
        private readonly IApiDescriptionGroupCollectionProvider _provider;
        private readonly ODataOptions _options;
        private readonly Lazy<ODataModel> _model;

        public ODataMetadataMiddleware(IModelBuilder modelBuilder, IApiDescriptionGroupCollectionProvider provider, IOptions<ODataOptions> options)
        {
            _provider = provider;
            _options = options.Value;

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
            var urlMap = _model.Value.UrlMap.Select(u => new { name = u.Value.Name, kind = u.Value.ContainerElementKind.ToString(), url = u.Key });

            var result = new Dictionary<string, object>
            {
                { "@odata.context", $"{context.Request.Scheme}://{context.Request.Host}{_options.MetadataPath}/$metadata" },
                { "value", urlMap }
            };

            context.Response.ContentType = "application/json; odata.metadata=minimal; charset=utf-8";

            var ser = JsonSerializer.Create();

            using (var sw = new StreamWriter(context.Response.Body, Encoding.UTF8))
                ser.Serialize(sw, result);

            return Task.CompletedTask;
        }

        private Task ApiMetadata(HttpContext context)
        {
            context.Response.ContentType = "application/xml";

            using (var xw = XmlWriter.Create(context.Response.Body, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
                CsdlWriter.TryWriteCsdl(_model.Value.Edm, xw, CsdlTarget.OData, out var errors);

            return Task.CompletedTask;
        }

        private ODataModel CreateModel()
        {
            var model = ODataModel.New(_options.Namespace);
            foreach (var api in _provider.ApiDescriptionGroups.Items.SelectMany(i => i.Items).ToArray())
            {
                var cad = (ControllerActionDescriptor)api.ActionDescriptor;

                model.MapSet(api.ActionDescriptor.Parameters[0].ParameterType.GetGenericArguments()[0].GetGenericArguments()[0], cad.ControllerName, FormatUrl( api.RelativePath, api.ActionDescriptor.Parameters));
            }

            return model;
        }
        
        private string FormatUrl(string relativePath, IList<ParameterDescriptor> parameters)
        {
            relativePath = relativePath.Replace(_options.MetadataPath.ToString().TrimStart('/'), "").TrimStart('/');

            foreach (var par in parameters)
                relativePath = relativePath.Replace($"{{{par.Name}}}", "").TrimEnd('/');

            return relativePath;
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