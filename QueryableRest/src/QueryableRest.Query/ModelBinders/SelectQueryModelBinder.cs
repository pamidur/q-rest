using Microsoft.AspNetCore.Mvc.ModelBinding;
using QueryableRest.Query.Configuration;
using QueryableRest.Query.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace QueryableRest.Query.ModelBinders
{
    public class SelectQueryModelBinder : IModelBinder
    {
        private readonly QuerySemantics _semantics;

        public SelectQueryModelBinder(QuerySemantics semantics)
        {
            _semantics = semantics ?? QuerySemantics.Default;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var key = bindingContext.ModelName;

            var resourceType = bindingContext.ModelType.GetTypeInfo().GetGenericArguments()[0];

            var selectObject = (SelectQuery)Activator.CreateInstance(bindingContext.ModelType, true);

            if (bindingContext.ValueProvider.ContainsPrefix(key))
            {
                var selectBlocks = bindingContext.ValueProvider.GetValue(key).Values;
                var entries = selectBlocks.SelectMany(_semantics.StatementSplitter);

                selectObject.Fields.AddRange(entries.Select(e => _semantics.FieldNameResolver(e, resourceType)));
            }

            bindingContext.Model = selectObject;
            return Task.CompletedTask;
        }
    }
}