using Microsoft.AspNetCore.Mvc.ModelBinding;
using QRest.Core.Contracts;
using System;
using System.IO;
using System.Linq;

namespace QRest.AspNetCore
{
    class RequestModel : IRequestModel
    {
        private readonly ModelBindingContext _context;

        public RequestModel(ModelBindingContext context)
        {
            _context = context;
        }

        public string ModelName => string.IsNullOrEmpty(_context.ModelName) ? _context.FieldName : _context.ModelName;

        public Stream GetBody()
        {
            return _context.ActionContext.HttpContext.Request.Body;
        }

        public ReadOnlyMemory<string> GetNamedQueryPart(string name)
        {
            return _context.ValueProvider.GetValue(name).ToArray().AsMemory();
        }
    }
}
