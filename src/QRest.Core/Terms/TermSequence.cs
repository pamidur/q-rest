using QRest.Core.Contracts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class TermSequence : TermBase, ITermSequence
    {
        private readonly LinkedList<ITerm> _sequence = new LinkedList<ITerm>();

        public ITerm Root => _sequence.First.Value;
        public ITerm Last => _sequence.Last.Value;
        public bool IsEmpty => !_sequence.Any();

        public override string SharedView => string.Join("", _sequence.Select(t => t.SharedView));
        public override string DebugView => $"#{string.Join("", _sequence.Select(t => t.DebugView))}";

        public void Add(ITerm term)
        {
            _sequence.AddLast(term);
        }

        public IEnumerator<ITerm> GetEnumerator()
        {
            return _sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sequence.GetEnumerator();
        }

        public override Expression CreateExpression(ICompilationContext compiler, Expression prev, ParameterExpression root)
        {
            var result = prev;

            foreach (var term in _sequence)
                result = term.CreateExpression(compiler, result, root);

            return result;
        }

        public override ITerm Clone()
        {
            return new TermSequence { Root.Clone() };
        }
    }
}
