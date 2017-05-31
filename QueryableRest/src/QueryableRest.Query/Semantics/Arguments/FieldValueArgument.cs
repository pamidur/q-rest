using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QueryableRest.Query.Semantics.Arguments
{
    public class FieldValueArgument : Argument
    {
        public override string[] Monikers { get; } = new[] { "" };

        public string FieldName { get; set; }

        public override Expression CreateExpression(ParameterExpression parameter)
        {
            return Expression.PropertyOrField(parameter, FieldName);
        }
    }
}