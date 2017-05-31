//using System.Linq;
//using System.Linq.Expressions;

//namespace QueryableRest.Query.Semantics.SortOperations
//{
//    public class OrderByDescendingOperation<TResource> : Operation<TResource>
//    {
//        public override string[] Monikers { get; } = new[] { "-" };

//        public override Expression CreateExpression(Expression query, Field<TResource> field)
//        {
//            var param = Expression.Parameter(typeof(TResource), "r");
//            var lambda = Expression.Lambda(field.GetAccesor(param), param);

//            var orderByAscendingCall = Expression.Call(typeof(Queryable), "OrderByDescending", new[] { typeof(TResource), lambda.ReturnType }, query, lambda);

//            return orderByAscendingCall;
//        }
//    }
//}