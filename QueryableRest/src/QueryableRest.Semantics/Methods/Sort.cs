using QueryableRest.Semantics.Operations;
using QueryableRest.Semantics.SortOperations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QueryableRest.Semantics.Methods
{
    public class Sort<TTarget> : Method<TTarget, SortOperation<TTarget>>
    {
        private static readonly Collection<SortOperation<TTarget>> sortOperations = new Collection<SortOperation<TTarget>>();
        public override ICollection<SortOperation<TTarget>> Operations => sortOperations;

        public IQueryable ApplyTo(IQueryable resources)
        {
            var data = resources;

            foreach (var operation in Operations)
            {
                var methodCall = (MethodCallExpression)operation.CreateExpression(data.Expression);
                data = data.Provider.CreateQuery(methodCall);
            }

            return data;
        }       

        public static Sort<TTarget> From(string data)
        {
            var sort = new Sort<TTarget>();
            GetBlocks(data).ToList().ForEach(b => sort.Operations.Add(SortOperation<TTarget>.From(b)));

            return sort;
        }
    }
}
