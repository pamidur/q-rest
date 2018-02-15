using QRest.Core.Contracts;
using System;
using System.Collections.Generic;
using QRest.Core.Terms;
using Sprache;
using Read = Sprache.Parse;
using System.Linq;
using QRest.Core.Operations;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations.Query;
using QRest.Core.Operations.Aggregations;

namespace QRest.Semantics.MethodChain
{
    public class MethodChainParser : IQuerySemanticsProvider
    {
        internal static readonly Parser<char> StringDelimiter = Read.Char('`');

        internal readonly Parser<ITerm> _parser;

        public MethodChainParser()
        {
            _parser = CallChain;
        }

        public ITerm Parse(IReadOnlyDictionary<string, string[]> queryParts)
        {
            var query = queryParts.FirstOrDefault().Value.FirstOrDefault();

            if (string.IsNullOrEmpty(query))
                return null;

            var result = _parser.TryParse(queryParts.First().Value[0]);

            return result.Value;
        }

        public string[] QuerySelector(string modelname)
        {
            return new[] { modelname };
        }

        internal static IOperation SelectOperation(string opName)
        {
            switch (opName)
            {
                case "ne": return new NotEqualOperation();
                case "eq": return new EqualOperation();
                case "not": return new NotOperation();
                case "where": return new WhereOperation();
                case "select": return new SelectOperation() { UseStaticTerminatingQuery = true };
                case "oneof": return new OneOfOperation();
                case "every": return new EveryOperation();
                case "first": return new FirstOperation();
                case "count": return new CountOperation();
                case "with": return new WithOperation();
                case "it": return new ItOperation();
                case "sum": return new SumOperation();
                case "contains": return new ContainsOperation();
                case "skip": return new SkipOperation();
                case "take": return new TakeOperation();
                default: throw new Exception();
            }
        }

        internal static Parser<ITerm> CallChain { get; } = Read.Ref(() =>
            from root in CallChainRoot
            from chunks in CallChainChunk.Many()
            from name in Read.Optional(Name)
            select chunks.Concat(new[] { name.GetOrDefault() }).Where(c => c != null)
            .Aggregate(root, (c1, c2) => { c1.GetLatestCall().Next = c2; return c1; })
            );

        internal static Parser<ITerm> CallChainRoot { get; } = Read.Ref(() =>
            from chunk in Property.XOr(Constant).XOr(Method)
            select chunk
        );

        internal static Parser<ITerm> CallChainChunk { get; } = Read.Ref(() =>
            from chunk in SubProperty.XOr(Method)
            select chunk
        );

        internal static Parser<ITerm> Method { get; } = Read.Ref(() =>
            from semic in Read.Char('-').XOr(Read.Char(':'))
            from method in MemberName
            from parameters in Read.Contained(Read.XDelimitedBy(CallChain, Read.Char(',')).Optional(), Read.Char('('), Read.Char(')')).Optional()
            select semic == '-' ?
                new MethodTerm { Operation = SelectOperation(method), Arguments = ReadCallParameters(parameters) }
                : new LambdaTerm { Operation = SelectOperation(method), Arguments = ReadCallParameters(parameters) }
            );

        internal static Parser<ITerm> SubProperty { get; } = Read.Ref(() =>
             from nav in Read.Char('.')
             from prop in Property
             select prop
            );

        internal static Parser<ITerm> Property { get; } = Read.Ref(() =>
             from name in MemberName
             select new PropertyTerm { PropertyName = name }
            );

        internal static Parser<ConstantTerm> Constant { get; } = Read.Ref(() =>
            new Parser<ConstantTerm>[] {
                StringConstant,
                NumberConstant,
                GuidConstant
            }.Aggregate((p1, p2) => p1.XOr(p2)).Select(o => new ConstantTerm { Value = o })
        );

        internal static Parser<ConstantTerm> NumberConstant { get; } = Read.Ref(() =>
            from str in Read.Digit.AtLeastOnce().Text()
            select new ConstantTerm { Value = Int32.Parse(str) }
        );

        internal static Parser<ConstantTerm> GuidConstant { get; } = Read.Ref(() =>
            from str in Read.Contained(Read.LetterOrDigit.Many().Text(), Read.Char('{'), Read.Char('}'))
            select new ConstantTerm { Value = Guid.Parse(str) }
        );

        internal static Parser<ConstantTerm> StringConstant { get; } = Read.Ref(() =>
            from str in Read.Contained(Read.AnyChar.Except(StringDelimiter).Many().Text().Optional(), StringDelimiter, StringDelimiter)
            select new ConstantTerm { Value = str.GetOrDefault() ?? "" }
        );

        internal static Parser<NameTerm> Name { get; } = Read.Ref(() =>
            from at in Read.Char('@')
            from name in MemberName
            select new NameTerm { Name = name }
        );

        internal static Parser<string> MemberName { get; } =
            from str1 in Read.Letter.Once().Text()
            from str2 in Read.LetterOrDigit.Many().Text()
            select str1 + str2;

        internal static List<ITerm> ReadCallParameters(IOption<IOption<IEnumerable<ITerm>>> data)
        {
            var parameters = data.GetOrDefault()?.GetOrDefault();
            if (parameters == null)
                return new List<ITerm>();

            return parameters.Where(r => r != null).ToList();
        }

    }
}
