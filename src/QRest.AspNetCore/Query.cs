using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using QRest.Core;
using QRest.Core.Contracts;
using System.Threading.Tasks;

namespace QRest.AspNetCore
{
    [ModelBinder(typeof(QueryModelBinder))]
    public class Query : QueryBase
    {
    }

    public class QueryModelBinder : IModelBinder
    {
        private readonly IQuerySemanticsProvider _parser;

        public QueryModelBinder(IOptions<QRestOptions> options)
        {
            _parser = options.Value.Semantics;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var result = _parser.Parse(new RequestModel(bindingContext));

            var query = new Query { Sequence = result };

            bindingContext.Result = ModelBindingResult.Success(query);

            return Task.FromResult(true);
        }
    }
}
