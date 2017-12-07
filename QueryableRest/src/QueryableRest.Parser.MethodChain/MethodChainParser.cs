using QRest.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using QRest.Core.Terms;
using Sprache;
using Read = Sprache.Parse;
using System.Linq;

namespace QRest.Semantics.MethodChain
{
    public class MethodChainParser : IQuerySemanticsProvider
    {
        private readonly Parser<ITerm> _parser;

        public MethodChainParser()
        {
            _parser = CallChain;
        }

        public ITerm Parse(IReadOnlyDictionary<string, string[]> queryParts)
        {
            return _parser.Parse(queryParts.First().Value[0]);
        }

        public bool QuerySelector(string queryparam, string modelname)
        {
            throw new NotImplementedException();
        }

        private static Parser<ITerm> CallChain => Read.Ref(() =>
            from root in CallChainRoot
            from chunks in CallChainChunk.Many()
            select chunks.Aggregate(root, (c1, c2) => { c1.GetLatestCall().Next = c2; return c1; })
            );

        private static Parser<ITerm> CallChainRoot => Read.Ref(() =>
            from chunk in Property.XOr(Constant).XOr(Method)
            select chunk
        );

        private static Parser<ITerm> CallChainChunk => Read.Ref(() =>
            from chunk in Property.XOr(Method)
            select chunk
        );

        private static Parser<ITerm> Method => Read.Ref(() =>
            from semic in Read.Char('-').XOr(Read.Char(':'))
            from method in Read.LetterOrDigit.Many().Text()
            from parameters in Read.Optional(Read.Contained(Read.XDelimitedBy(CallChain, Read.Char(',')), Read.Char('('), Read.Char(')')))
            select semic == '-'?
                new MethodTerm { Method = method, Arguments = parameters.IsEmpty ? new List<ITerm>() : parameters.GetOrDefault().Where(r => r != null).ToList() }
                : new LambdaTerm { Method = method, Arguments = parameters.IsEmpty ? new List<ITerm>() : parameters.GetOrDefault().Where(r => r != null).ToList() }
            );

        private static Parser<ITerm> Property =>
            from nav in Read.Optional(Read.Char('.'))
            from str1 in Read.Letter.Once().Text()
            from str2 in Read.LetterOrDigit.Many().Text()
            select new PropertyTerm { PropertyName = str1 + str2 };


        private static Parser<ConstantTerm> Constant =>
            new List<Parser<ConstantTerm>> {
                from str in Read.Contained(Read.LetterOrDigit.Many().Text(), Read.Char('\''), Read.Char('\''))
                select new ConstantTerm { Value = str },

                from str in Read.Digit.AtLeastOnce().Text()
                select new ConstantTerm { Value = Int32.Parse(str) },

                from str in Read.Contained(Read.LetterOrDigit.Many().Text(), Read.Char('{'), Read.Char('}'))
                select new ConstantTerm { Value = Guid.Parse(str) }
            }.Aggregate((p1, p2) => p1.XOr(p2));

    }
}
