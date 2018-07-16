using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using QRest.Core;
using QRest.Core.Contracts;
using System.Linq;
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
            var queryNames = _parser.QuerySelector(string.IsNullOrEmpty(bindingContext.ModelName) ? bindingContext.FieldName : bindingContext.ModelName);
            var queryFields = queryNames.ToDictionary(n => n, n => bindingContext.ValueProvider.GetValue(n).ToArray());

            var result = _parser.Parse(queryFields);

            var query = new Query();

            if (result != null)
                query.RootTerm = result;

            bindingContext.Result = ModelBindingResult.Success(query);

            return Task.FromResult(true);
        }
    }
}
