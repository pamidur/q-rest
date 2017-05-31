using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace QueryableRest.Query.Semantics
{
    public abstract class Operation
    {
        public abstract string[] Monikers { get; }

        public virtual bool IsFinal { get; } = false;

        public abstract Expression CreateExpression(Expression query, Argument argument);

        protected Type GetQueryElementType(Expression query)
        {
            var typeInfo = query.Type.GetTypeInfo();

            if (typeInfo.FullName != "System.Linq.IQueryable`1")
            {
                typeInfo = typeInfo.GetInterface("System.Linq.IQueryable`1").GetTypeInfo();
            }

            return typeInfo.GetGenericArguments()[0];
        }
    }
}