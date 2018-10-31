using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using QRest.Core;
using QRest.Core.Contracts;
using QRest.Core.Terms;
using System.Threading.Tasks;

namespace QRest.AspNetCore
{
    [ModelBinder(typeof(QueryModelBinder))]
    public class Query : QueryBase
    {
        public Query(TermSequence sequence, ICompiler compiller) : base(sequence, compiller)
        {
        }
    }

    public class QueryModelBinder : IModelBinder
    {
        private readonly QRestOptions _options;

        public QueryModelBinder(IOptions<QRestOptions> options)
        {
            _options = options.Value;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var sequence = _options.Semantics.Parse(new RequestModel(bindingContext));

            var query = new Query(sequence, _options.Compiler);

            bindingContext.Result = ModelBindingResult.Success(query);

            return Task.FromResult(true);
        }
    }
}
