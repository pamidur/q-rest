using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QRest.AspNetCore.OData.Metadata;
using QRest.Core.Terms;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QRest.AspNetCore.OData
{
    internal class ODataQueryResult : ActionResult
    {
        private readonly ODataQueryStructure _query;
        private readonly IReadOnlyDictionary<RootTerm, object> _results;

        public ODataQueryResult(ODataQueryStructure query, IReadOnlyDictionary<RootTerm, object> results)
        {
            _query = query;
            _results = results;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.ContentType = "application/json; odata.metadata=minimal; charset=utf-8";

            var ser = JsonSerializer.Create();
            var response = CreateResponse(context);

            using (var sw = new StreamWriter(context.HttpContext.Response.Body, Encoding.UTF8))
                ser.Serialize(sw, response);

            return Task.CompletedTask;
        }

        private object CreateResponse(ActionContext context)
        {
            var builder = (IModelBuilder)context.HttpContext.RequestServices.GetService(typeof(IModelBuilder));

            var meta = (ODataMetadataMiddleware)context.HttpContext.RequestServices.GetService(typeof(ODataMetadataMiddleware));

            var result = new Dictionary<string, object>();

            if (meta.IsInUse)
            {
                var metaurl = meta.GetMetaUrl(context.HttpContext);
                var edmType = builder.GetEdmName(context.ActionDescriptor);
                if (edmType != null)
                    result.Add("@odata.context", $"{metaurl}#{edmType}");
            }

            if (_query.Count != null && _results.TryGetValue(_query.Count, out var count))
                result.Add("@odata.count", count);

            if (_query.Data != null && _results.TryGetValue(_query.Data, out var data))
                result.Add("value", data);

            return result;
        }
    }
}
