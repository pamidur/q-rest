using QRest.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Core.Terms
{
    public class MethodTerm : TermBase
    {
        public MethodTerm()
        {
            Arguments = new List<ITermSequence>();
        }

        public new IOperation Operation { get => base.Operation; set => base.Operation = value; }
        public new List<ITermSequence> Arguments { get => (List<ITermSequence>) base.Arguments; set => base.Arguments = value; }

        protected virtual string GetView(Func<ITermSequence, string> viewSelector)
        {
            var args = string.Join(",", Arguments.Select(viewSelector));
            var argsLiteral = args.Length > 0 ? $"({args})" : "";
            return $"-{Operation.GetType().Name.ToLowerInvariant().Replace("operation", "")}{argsLiteral}";
        }

        public override string SharedView => GetView(t => t.SharedView);
        public override string DebugView => GetView(t => t.DebugView);
        public override string KeyView => GetView(t => t.KeyView);

        public override ITerm Clone() => new MethodTerm { Operation = Operation, Arguments = Arguments.Select(a => (ITermSequence)a.Clone()).ToList() };
    }
}
