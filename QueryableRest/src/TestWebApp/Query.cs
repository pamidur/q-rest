using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QRest.Core;
using QRest.Core.Contracts;
using QRest.Core.Terms;
using QRest.Semantics.MethodChain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TestWebApp
{
    [ModelBinder(typeof(QueryModelBinder))]
    public class Query
    {
        public ITerm RootTerm { get; set; }

        public object Apply<T>(IQueryable<T> target)
        {
            if (RootTerm == null)
                return target;

            var dataParam = Expression.Parameter(typeof(IQueryable<T>));

            var registry = new Registry();
            Registry.RegisterDefaultOperations(registry);

            var e = RootTerm.CreateExpression(dataParam, dataParam, new QueryContext(registry));

            var l = Expression.Lambda(e, dataParam);

            var r = l.Compile().DynamicInvoke(target);

            return r;
        }
    }

    public class QueryModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            IQuerySemanticsProvider parser = new MethodChainParser();
            var queryNames = parser.QuerySelector(string.IsNullOrEmpty(bindingContext.ModelName) ? bindingContext.FieldName : bindingContext.ModelName);
            var queryFields = queryNames.ToDictionary(n => n, n => bindingContext.ValueProvider.GetValue(n).ToArray());

            var result = parser.Parse(queryFields);

            bindingContext.Result = ModelBindingResult.Success(new Query { RootTerm = result });
        }
    }
}
