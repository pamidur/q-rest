using QRest.Core.Contracts;
using QRest.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Core.Terms
{
    public class MethodTerm : TermBase
    {
        public MethodTerm(IOperation operation, IReadOnlyList<ITermSequence> terms = null)
        {
            Operation = operation;
            Arguments = terms ?? new List<ITermSequence>();
        }

        protected virtual string GetView(Func<ITermSequence, string> viewSelector)
        {
            var args = string.Join(",", Arguments.Select(viewSelector));
            var argsLiteral = args.Length > 0 ? $"({args})" : "";
            return $"-{Operation.GetName()}{argsLiteral}";
        }

        public override string SharedView => GetView(t => t.SharedView);
        public override string DebugView => GetView(t => t.DebugView);
        public override string KeyView => GetView(t => t.KeyView);

        public override ITerm Clone() => new MethodTerm(Operation, Arguments.Select(a => a.Clone()).ToList());
    }
}
