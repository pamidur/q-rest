using QRest.Core.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Core.Terms
{
    public sealed class SequenceTerm : IEnumerable<ITerm>, ITerm
    {
        private readonly LinkedList<ITerm> _sequence = new LinkedList<ITerm>();

        public SequenceTerm(params ITerm[] terms)
        {
            AddTerms(terms);            
        }

        public ITerm Root => _sequence.First.Value;
        public ITerm Last => _sequence.Last.Value;
        public bool IsEmpty => !_sequence.Any();

        public string ViewQuery { get; private set; }
        public string ViewKey { get; private set; }
        public string ViewDebug { get; private set; }

        public SequenceTerm Append(params ITerm[] terms)
        {
            var clone = (SequenceTerm)Clone();
            clone.AddTerms(terms);

            return clone;
        }

        private void AddTerms(IEnumerable<ITerm> terms)
        {
            foreach (var term in terms)
            {
                if (term is SequenceTerm s)
                    AddTerms(s);
                else if (term != null)
                    CheckAndAddLast(term);
            }
        }

        private void CheckAndAddLast(ITerm term)
        {
            if ((term is ConstantTerm || term is LambdaTerm) && _sequence.Count != 0)
                throw new InvalidOperationException($"Cannot chain '{term.GetType().Name}' is the middle of sequence.");

            if ((term is NameTerm) && _sequence.Count == 0)
                throw new InvalidOperationException($"Cannot start sequence with '{term.GetType().Name}'.");

            if ((term is ContextTerm) && _sequence.Count > 0)
                throw new InvalidOperationException($"Context cannot be changed in the mid of the sequence.");

            _sequence.AddLast(term);

            ViewQuery = FormatView(_sequence, t => t.ViewQuery);
            ViewKey = FormatView(_sequence, t => t.ViewKey);
            ViewDebug = $"#{string.Join("", _sequence.Select(t => t.ViewDebug))}";
        }

        public IEnumerator<ITerm> GetEnumerator()
        {
            return _sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sequence.GetEnumerator();
        }

        public ITerm Clone() => new SequenceTerm(_sequence.Select(t => t.Clone()).ToArray());

        public override string ToString() => ViewQuery;

        private static string FormatView(ICollection<ITerm> sequence, Func<ITerm, string> selector)
        {
            var terms = sequence.ToArray();

            if (terms.Length != 1 && terms[0] is ContextTerm ct && ct.IsRoot)
                terms = terms.Skip(1).ToArray();

            var data = terms.Select(selector).ToArray();

            //remove leading dot for root property
            if (terms.Length > 0 && terms[0] is PropertyTerm)
                data[0] = data[0].Substring(1);

            return $"{string.Join("", data)}";
        }
    }
}
