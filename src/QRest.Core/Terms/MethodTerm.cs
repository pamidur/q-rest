using QRest.Core.Operations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Core.Terms
{
    public sealed class MethodTerm : ITerm
    {
        public IOperation Operation { get; }
        public IReadOnlyList<ITerm> Arguments { get; }

        public MethodTerm(IOperation operation, params ITerm[] terms)
        {
            Operation = operation;
            Arguments = terms;

            ViewQuery = GetView(t => t.ViewQuery);
            ViewDebug = GetView(t => t.ViewDebug);
            ViewKey = GetView(t => t.ViewKey);
        }

        private string GetView(Func<ITerm, string> viewSelector)
        {
            var args = string.Join(",", Arguments.Select(viewSelector));
            var argsLiteral = args.Length > 0 ? $"({args})" : "";
            return $"-{Operation.Key}{argsLiteral}";
        }

        public string ViewQuery { get; }
        public string ViewDebug { get; }
        public string ViewKey { get; }

        public ITerm Clone() => new MethodTerm(Operation, Arguments.Select(a => a.Clone()).ToArray());
    }
}
