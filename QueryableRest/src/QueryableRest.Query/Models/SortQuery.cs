using QueryableRest.Query.Semantics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QueryableRest.Query.Models
{
    public class SortQuery<TResource>
    {
        public IQueryable ApplyTo(IQueryable resources)
        {
            var data = resources;

            foreach (var field in Fields)
            {
                var methodCall = (MethodCallExpression)field.Item2.CreateExpression(data.Expression, field.Item1);
                data = data.Provider.CreateQuery(methodCall);
            }

            return data;
        }

        public SortQuery()
        {
            Fields = new List<Tuple<Argument, Operation>>();
        }

        public List<Tuple<Argument, Operation>> Fields { get; }
    }
}