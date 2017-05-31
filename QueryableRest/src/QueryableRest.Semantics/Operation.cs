using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace QueryableRest.Semantics
{
    public abstract class Operation<TTarget>
    {
        public Operation<TTarget> Operand { get; protected set; }
        public abstract Expression CreateExpression(Expression parent);
        public abstract string Serialize();

        protected Type GetQueryElementType(Expression query)
        {
            var typeInfo = query.Type.GetTypeInfo();

            if (typeInfo.FullName != "System.Linq.IQueryable`1")
            {
                typeInfo = typeInfo.ImplementedInterfaces.First(i => i.Namespace == "System.Linq" && i.Name == "IQueryable`1").GetTypeInfo();
            }

            return typeInfo.GenericTypeArguments[0];
        }

        public override string ToString()
        {
            return Serialize();
        }
    }


    //    public abstract class Operation
    //{
    //    public abstract string[] Monikers { get; }

    //    public virtual bool IsFinal { get; } = false;

    //    public abstract Expression CreateExpression(Expression query, Argument argument);

    //    protected Type GetQueryElementType(Expression query)
    //    {
    //        Expression.Property()
    //        var typeInfo = query.Type.GetTypeInfo();

    //        if (typeInfo.FullName != "System.Linq.IQueryable`1")
    //        {
    //            typeInfo = typeInfo.GetInterface("System.Linq.IQueryable`1").GetTypeInfo();
    //        }

    //        return typeInfo.GetGenericArguments()[0];
    //    }
    //}
}