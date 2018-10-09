using QRest.Core.Contracts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Core.Terms
{
    public class TermSequence : ITermSequence
    {
        private readonly LinkedList<ITerm> _sequence = new LinkedList<ITerm>();

        public ITerm Root => _sequence.First.Value;
        public ITerm Last => _sequence.Last.Value;
        public bool IsEmpty => !_sequence.Any();

        public string SharedView => string.Join("", _sequence.Select(t => t?.SharedView ?? "Shit happend"));
        public string KeyView => string.Join("", _sequence.Select(t => t.KeyView));
        public string DebugView => $"#{string.Join("", _sequence.Select(t => t.DebugView))}";

        public void Add(ITerm term)
        {
            _sequence.AddLast(term);
        }

        public void Add(ITermSequence terms)
        {
            foreach (var term in terms)
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

        public ITermSequence Clone()
        {
            return new TermSequence { Root.Clone() };
        }
    }
}
