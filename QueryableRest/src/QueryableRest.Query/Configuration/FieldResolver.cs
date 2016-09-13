using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueryableRest.Query.Configuration
{
    public class DefaultFieldResolver
    {
        public virtual Func<string, Type, string> FieldNameResolver { get; set; } = (field, type) =>
        {
            return field;
        };
    }
}