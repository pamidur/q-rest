using Microsoft.AspNetCore.Mvc.ModelBinding;
using QRest.AspNetCore.Contracts;
using QRest.AspNetCore.Native;
using QRest.Compiler.Standard;
using QRest.Core.Contracts;
using System;
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
            _compiler = compiler ?? new StandardCompiler();
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var queryStructure = _semantics.ReadQueryStructure(bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToArray(), bindingContext.HttpContext.Request);

            var query = Activator.CreateInstance(bindingContext.ModelType, queryStructure, _compiler);

            bindingContext.Result = ModelBindingResult.Success(query);
            return Task.FromResult(true);
        }
    }
}
