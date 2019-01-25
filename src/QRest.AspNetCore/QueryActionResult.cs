using Microsoft.AspNetCore.Mvc;
using QRest.AspNetCore.Contracts;
using QRest.AspNetCore.Native;
using QRest.Core.Terms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QRest.AspNetCore
{
    public static class QRestExtensions
    {
        public static QueryActionResult<T> ToActionResult<T>(this Query query, T source)
        {
            return QueryActionResult.From(query, source);
        }
    }

    public class QueryActionResult<T> : QueryActionResult
    {
        internal QueryActionResult(IQueryStructure structure, IReadOnlyDictionary<RootTerm, object> results) : base(structure, results)
        {
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
            return semantics.WriteQueryResponse(_structure, _results, typeof(T));
        }
    }

    public abstract class QueryActionResult : ActionResult
    {
        protected readonly IQueryStructure _structure;
        protected readonly IReadOnlyDictionary<RootTerm, object> _results;

        internal QueryActionResult(IQueryStructure structure, IReadOnlyDictionary<RootTerm, object> results)
        {
            _structure = structure;
            _results = results;
        }

        public static QueryActionResult<T> From<T>(Query query, T source)
        {
            var results = new Dictionary<RootTerm, object>();

            foreach (var lambda in query.Structure.GetAll().Where(q => q != null))
                results[lambda] = query.Compiller.Compile<T>(lambda)(source);

            return new QueryActionResult<T>(query.Structure, results);
        }
    }
}
