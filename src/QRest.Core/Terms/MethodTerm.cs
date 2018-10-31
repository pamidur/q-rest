using QRest.Core.Contracts;
using QRest.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Core.Terms
{
    public class MethodTerm : ITerm
    {
        public IOperation Operation { get; }
        public IReadOnlyList<SequenceTerm> Arguments { get; }

        public MethodTerm(IOperation operation, IReadOnlyList<SequenceTerm> terms = null)
        {
            Operation = operation;
            Arguments = terms ?? new List<SequenceTerm>();
        }

        protected virtual string GetView(Func<SequenceTerm, string> viewSelector)
        {
            var args = string.Join(",", Arguments.Select(viewSelector));
            var argsLiteral = args.Length > 0 ? $"({args})" : "";
            return $"-{Operation.GetName()}{argsLiteral}";
        }

        public string SharedView => GetView(t => t.SharedView);
        public string DebugView => GetView(t => t.DebugView);
        public string KeyView => GetView(t => t.KeyView);

        public ITerm Clone => new MethodTerm(Operation, Arguments.Select(a => (SequenceTerm)a.Clone).ToList());
    }
}
