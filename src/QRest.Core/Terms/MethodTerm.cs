using QRest.Core.Contracts;
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

            SharedView = GetView(t => t.SharedView);
            DebugView = GetView(t => t.DebugView);
            KeyView = GetView(t => t.KeyView);
        }

        protected string GetView(Func<SequenceTerm, string> viewSelector)
        {
            var args = string.Join(",", Arguments.Select(viewSelector));
            var argsLiteral = args.Length > 0 ? $"({args})" : "";
            return $"-{Operation.Key}{argsLiteral}";
        }

        public string SharedView { get; }
        public string DebugView { get; }
        public string KeyView { get; }

        public ITerm Clone() => new MethodTerm(Operation, Arguments.Select(a => (SequenceTerm)a.Clone()).ToList());
    }
}
