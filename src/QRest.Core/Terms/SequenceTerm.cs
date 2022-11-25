using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace QRest.Core.Terms
{
    public sealed class SequenceTerm : IReadOnlyList<ITerm>, ITerm
    {
        private readonly ImmutableList<ITerm> _sequence;
        public ITerm Root => _sequence[0];
        public ITerm Last => _sequence[^1];

        public SequenceTerm(params ITerm[] terms)
           : this(terms == null || terms.Length == 0 ? 
                 throw new InvalidOperationException($"Sequence MUST contain at least one element.")
                 : BuildSequence(terms))
        {
        }

        private SequenceTerm(ImmutableList<ITerm> terms)
        {
            _sequence = terms;
            ViewQuery = FormatView(_sequence, t => t.ViewQuery);
            ViewKey = FormatView(_sequence, t => t.ViewKey);
            ViewDebug = $"#{string.Join("", _sequence.Select(t => t.ViewDebug))}";
        }        

        public string ViewQuery { get; private set; }
        public string ViewKey { get; private set; }
        public string ViewDebug { get; private set; }
      

        public int Count => _sequence.Count;
        public ITerm this[int index] => _sequence.ElementAt(index);

        public IEnumerator<ITerm> GetEnumerator()
        {
            return _sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sequence.GetEnumerator();
        }

        public override string ToString() => ViewQuery;

        private static string FormatView(IReadOnlyCollection<ITerm> sequence, Func<ITerm, string> selector)
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

        private static ImmutableList<ITerm> BuildSequence(ITerm[] terms)
        {
            var builder = ImmutableList.CreateBuilder<ITerm>();

            foreach (var term in terms)
            {
                if (term is SequenceTerm s)
                    builder.AddRange(s._sequence);
                else if (term != null)
                {
                    if ((term is ConstantTerm || term is LambdaTerm) && builder.Count != 0)
                        throw new InvalidOperationException($"Cannot chain '{term.GetType().Name}' is the middle of sequence.");

                    if ((term is NameTerm) && builder.Count == 0)
                        throw new InvalidOperationException($"Cannot start sequence with '{term.GetType().Name}'.");

                    if ((term is ContextTerm) && builder.Count > 0)
                        throw new InvalidOperationException($"Context cannot be changed in the mid of the sequence.");

                    builder.Add(term);
                }
            }

            return builder.ToImmutable();
        }
    }
}
