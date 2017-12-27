using QRest.Core.Contracts;
using System;
using System.Collections.Generic;
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
            var query = queryParts.FirstOrDefault().Value.FirstOrDefault();

            if (string.IsNullOrEmpty(query))
                return null;

            return _parser.Parse(queryParts.First().Value[0]);
        }

        public string[] QuerySelector(string modelname)
        {
            return new[] { modelname };
        }

        private static Parser<ITerm> CallChain { get; } = Read.Ref(() =>
            from root in CallChainRoot
            from chunks in CallChainChunk.Many()
            from name in Read.Optional(Name)
            select chunks.Concat(new[] { name.GetOrDefault() }).Where(c => c != null)
            .Aggregate(root, (c1, c2) => { c1.GetLatestCall().Next = c2; return c1; })
            );

        private static Parser<ITerm> CallChainRoot { get; } = Read.Ref(() =>
            from chunk in Property.XOr(Constant).XOr(Method)
            select chunk
        );

        private static Parser<ITerm> CallChainChunk { get; } = Read.Ref(() =>
            from chunk in Property.XOr(Method)
            select chunk
        );

        private static Parser<ITerm> Method { get; } = Read.Ref(() =>
            from semic in Read.Char('-').XOr(Read.Char(':'))
            from method in Read.LetterOrDigit.Many().Text()
            from parameters in Read.Optional(Read.Contained(Read.Optional(Read.XDelimitedBy(CallChain, Read.Char(','))), Read.Char('('), Read.Char(')')))
            select semic == '-' ?
                new MethodTerm { Method = method, Arguments = ReadCallParameters(parameters) }
                : new LambdaTerm { Method = method, Arguments = ReadCallParameters(parameters) }
            );

        private static Parser<ITerm> Property { get; } =
            from nav in Read.Optional(Read.Char('.'))
            from str1 in Read.Letter.Once().Text()
            from str2 in Read.LetterOrDigit.Many().Text()
            select new PropertyTerm { PropertyName = str1 + str2 };


        private static Parser<ConstantTerm> Constant { get; } =
            new List<Parser<ConstantTerm>> {
                from str in Read.Contained(Read.LetterOrDigit.Many().Text(), Read.Char('`'), Read.Char('`'))
                select new ConstantTerm { Value = str },

                from str in Read.Digit.AtLeastOnce().Text()
                select new ConstantTerm { Value = Int32.Parse(str) },

                from str in Read.Contained(Read.LetterOrDigit.Many().Text(), Read.Char('{'), Read.Char('}'))
                select new ConstantTerm { Value = Guid.Parse(str) }
            }.Aggregate((p1, p2) => p1.XOr(p2));

        private static Parser<ConstantTerm> Primitives { get; } =
            new List<Parser<ConstantTerm>> {
                from str in Read.Contained(Read.LetterOrDigit.Many().Text(), Read.Char('`'), Read.Char('`'))
                select new ConstantTerm { Value = str },

                from str in Read.Digit.AtLeastOnce().Text()
                select new ConstantTerm { Value = Int32.Parse(str) },

                from str in Read.Contained(Read.LetterOrDigit.Many().Text(), Read.Char('{'), Read.Char('}'))
                select new ConstantTerm { Value = Guid.Parse(str) }
            }.Aggregate((p1, p2) => p1.XOr(p2));


        private static Parser<NameTerm> Name { get; } =
            from at in Read.Char('@')
            from str1 in Read.Letter.Once().Text()
            from str2 in Read.LetterOrDigit.Many().Text()
            select new NameTerm { Name = str1 + str2 };

        private static List<ITerm> ReadCallParameters(IOption<IOption<IEnumerable<ITerm>>> data)
        {
            var parameters = data.GetOrDefault()?.GetOrDefault();
            if (parameters == null)
                return new List<ITerm>();

            return parameters.Where(r => r != null).ToList();
        }

    }
}
