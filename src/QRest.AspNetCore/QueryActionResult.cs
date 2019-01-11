using Microsoft.AspNetCore.Mvc;
using QRest.AspNetCore.Contracts;
using QRest.AspNetCore.Native;
using QRest.Core.Terms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRest.AspNetCore
{
    public static class QRestExtensions
    {
        public static QueryActionResult ToActionResult<T>(this Query<T> query, T source)
        {
            return QueryActionResult.From(query, source);
        }
    }

    public class QueryActionResult : ActionResult
    {
        private readonly IQueryStructure _structure;
        private readonly IReadOnlyDictionary<LambdaTerm, object> _results;

        public QueryActionResult(IQueryStructure structure, IReadOnlyDictionary<LambdaTerm, object> results)
        {
            _structure = structure;
            _results = results;
        }

        internal static QueryActionResult From<T>(Query<T> query, T source)
        {
            var results = new Dictionary<LambdaTerm, object>();

            foreach (var lambda in query.Structure.GetAll())
                results[lambda] = query.Compiller.Compile<T>(lambda)(source);

            return new QueryActionResult(query.Structure, results);
        }

        public override void ExecuteResult(ActionContext context)
        {
            var result = GetActualResult(context);
            result.ExecuteResult(context);
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {            
            var result = GetActualResult(context);
            return result.ExecuteResultAsync(context);
        }

        private ActionResult GetActualResult(ActionContext context)
        {
            var semantics = (context.HttpContext.RequestServices.GetService(typeof(ISemantics)) as ISemantics) ?? new NativeSemantics();
            return semantics.WriteQueryResponse(_structure, _results);
        }
    }
}
