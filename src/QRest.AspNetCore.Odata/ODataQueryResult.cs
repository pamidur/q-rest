using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QRest.AspNetCore.OData
{
    internal class ODataQueryResult : ActionResult
    {
        private readonly object _value;

        public ODataQueryResult(object value)
        {
            _value = value;
        }

        public override void ExecuteResult(ActionContext context)
        {
            base.ExecuteResult(context);
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.ContentType = "application/json; odata.metadata=minimal";

            var ser = JsonSerializer.Create();

            using (var sw = new StreamWriter(context.HttpContext.Response.Body))
                ser.Serialize(sw, _value);
        }
    }
}
