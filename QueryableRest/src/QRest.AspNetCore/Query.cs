using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QRest.Core;
using QRest.Core.Contracts;
using QRest.Semantics.MethodChain;
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

        public QueryModelBinder(IQuerySemanticsProvider parser = null)
        {
            _parser = parser ?? new MethodChainParser();
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var queryNames = _parser.QuerySelector(string.IsNullOrEmpty(bindingContext.ModelName) ? bindingContext.FieldName : bindingContext.ModelName);
            var queryFields = queryNames.ToDictionary(n => n, n => bindingContext.ValueProvider.GetValue(n).ToArray());

            var result = _parser.Parse(queryFields);

            bindingContext.Result = ModelBindingResult.Success(new Query { RootTerm = result });

            return Task.FromResult(true);
        }
    }
}
