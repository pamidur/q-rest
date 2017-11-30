using System;
using System.Collections.Generic;
using System.Reflection;

namespace QueryableRest.Semantics.Containers
{
    class PropertiesContainer : Dictionary<string, object>
    {
        public static readonly MethodInfo AddMethod = ((Action<string, object>)new Dictionary<string, object>().Add).Method;
        public static readonly Type Type = typeof(PropertiesContainer);

        public PropertiesContainer() { }

        public PropertiesContainer(IDictionary<string, object> initial) : base(initial)
        {

        }
    }
}
