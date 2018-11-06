using QRest.Core.Contracts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Core.Terms
{
    public class SequenceTerm : IEnumerable<ITerm>, ITerm
    {
        private readonly LinkedList<ITerm> _sequence = new LinkedList<ITerm>();

        public SequenceTerm(params ITerm[] terms) => Add(terms);
        public SequenceTerm(SequenceTerm sequence) => Add((IEnumerable<ITerm>)sequence);

        public ITerm Root => _sequence.First.Value;
        public ITerm Last => _sequence.Last.Value;
        public bool IsEmpty => !_sequence.Any();

        public virtual string SharedView => $"{string.Join("", _sequence.Select(t => t.SharedView))}";
        public virtual string KeyView => string.Join("", _sequence.Select(t => t.KeyView));
        public virtual string DebugView => $"#{string.Join("", _sequence.Select(t => t.DebugView))}";

        protected void Add(ITerm term)
        {
            if (!(term is LambdaTerm) && term is SequenceTerm s)
                Add((IEnumerable<ITerm>)s);
            else if (term != null)
                _sequence.AddLast(term);
        }

        protected void Add(IEnumerable<ITerm> terms)
        {
            foreach (var term in terms)
                Add(term);
        }

        public IEnumerator<ITerm> GetEnumerator()
        {
            return _sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sequence.GetEnumerator();
        }

        public virtual ITerm Clone() => new SequenceTerm(_sequence.Select(t => t.Clone()).ToArray());

        public override string ToString() => SharedView;
    }
}
