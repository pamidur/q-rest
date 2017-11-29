using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace QueryableRest.Semantics.Terms
{
    public class CallTerm : ITerm
    {
        public string Method { get; set; }
        public List<ITerm> Arguments { get; set; } = new List<ITerm>();
        public ITerm Next { get; set; }

        public bool IsPerEachCall { get; set; }

        public Expression CreateExpression(Expression context, Expression root, Registry registry)
        {
            var op = registry.Operations[Method];

            Expression exp = null;

            if (IsPerEachCall)
            {
                var newroot = GetArgumentsRoot(context);
                var args = Arguments.Select(a => a.CreateExpression(newroot, newroot, registry)).ToList();
                exp = op.CreateExpression(context, newroot, args);
            }
            else
            {
                var args = Arguments.Select(a => a.CreateExpression(root, root, registry)).ToList();
                exp = op.CreateExpression(context, root, args);
            }

            return Next?.CreateExpression(exp, root, registry) ?? exp;
        }

        public override string ToString()
        {
            var args = string.Join(",", Arguments.Select(a => a.ToString().TrimStart('.')));

            var argsLiteral = args.Length > 0 ? $"({args})" : "";

            return $"{(IsPerEachCall ? ":" : "-")}{Method}{argsLiteral}{Next?.ToString()}";
        }

        public Expression GetArgumentsRoot(Expression context)
        {
            return Expression.Parameter(GetQueryElementType(context));
        }

        protected Type GetQueryElementType(Expression query)
        {
            var typeInfo = query.Type.GetTypeInfo();

            if ($"{typeInfo.Namespace}.{typeInfo.Name}" != "System.Linq.IQueryable`1")
            {
                typeInfo = typeInfo.GetInterface("System.Linq.IQueryable`1").GetTypeInfo();
            }

            return typeInfo.GetGenericArguments()[0];
        }
    }
}
