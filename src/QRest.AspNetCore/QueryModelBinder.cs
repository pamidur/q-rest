using Microsoft.AspNetCore.Mvc.ModelBinding;
using QRest.AspNetCore.Contracts;
using QRest.AspNetCore.Native;
using System.Linq;
using System.Threading.Tasks;

namespace QRest.AspNetCore
{
    public class QueryModelBinder : IModelBinder
    {
        private readonly ISemantics _semantics;
        private readonly ICompiler _compiler;

        public QueryModelBinder(ISemantics semantics = null, ICompiler compiler = null)
        {
            _semantics = semantics ?? new NativeSemantics();
            _compiler = compiler ?? new NativeCompiler();
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            try
            {
                var queryStructure = _semantics.ReadQueryStructure(bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToArray(), bindingContext.HttpContext.Request);
                var query = new Query(queryStructure, _compiler);

                bindingContext.Result = ModelBindingResult.Success(query);
                return Task.CompletedTask;
            }
            catch(InvalidSemanticsException se)
            {
                var expected = se.Expectations.Any() ? $"Expected : { string.Join(",", se.Expectations)}" : string.Empty;
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"{se.Message} at position {se.Position}. {expected}"/*  se, bindingContext.ModelMetadata*/);
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }            
        }
    }
}
