using QRest.Core.Contracts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class TermSequence : ITermSequence, IEnumerable<ITerm>
    {
        private readonly LinkedList<ITerm> _sequence = new LinkedList<ITerm>();

        public ITerm Root => _sequence.First.Value;
        public ITerm Last => _sequence.Last.Value;
        public bool IsEmpty => !_sequence.Any();

        public string DebugView => "";

        public void Add(ITerm term)
        {
            _sequence.AddLast(term);
        }

        public ITermSequence Clone()
        {
            throw new System.NotImplementedException();
            //return new TermSequence { Root = Root.Clone() };
        }

        public IEnumerator<ITerm> GetEnumerator()
        {
            return _sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sequence.GetEnumerator();
        }

        public override string ToString()
        {
            var result = "";
            foreach (var term in _sequence)
                result = $"{result}{term.ToString()}";

            return result;
        }

        public Expression CreateExpression(ICompilationContext compiler, Expression prev, ParameterExpression root)
        {
            var result = prev;

            foreach (var term in _sequence)
                result = term.CreateExpression(compiler, result, root);

            return result;
        }

        ITerm ITerm.Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
