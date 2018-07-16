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
using QRest.Core.Operations.Query.OrderDirectionOperations;

namespace QRest.Semantics.MethodChain
{
    public class MethodChainParser : IQuerySemanticsProvider
    {
        internal static readonly Parser<char> StringDelimiter = Read.Char('`');
        internal static readonly Parser<char> ArgumentDelimiter = Read.Char(',');

        internal static readonly Parser<char> CallOpenBracket = Read.Char('(');
        internal static readonly Parser<char> CallCloseBracket = Read.Char(')');

        internal static readonly Parser<char> ArrayOpenBracket = Read.Char('[');
        internal static readonly Parser<char> ArrayCloseBracket = Read.Char(']');

        internal static readonly Parser<char> MethodIndicator = Read.Char('-');
        internal static readonly Parser<char> LambdaIndicator = Read.Char(':');

        internal static readonly Parser<char> PropertyNavigator = Read.Char('.');

        internal static readonly Parser<string> TrueConstantString = Read.String("true").Text();
        internal static readonly Parser<string> FalseConstantString = Read.String("false").Text();

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
                case "get": return new SelectOperation() { UseStaticTerminatingQuery = true };
                case "oneof": return new OneOfOperation();
                case "every": return new EveryOperation();
                case "first": return new FirstOperation();
                case "count": return new CountOperation();
                case "with": return new WithOperation();
                case "it": return new ItOperation();
                case "sum": return new SumOperation();
                case "has": return new ContainsOperation();
                case "skip": return new SkipOperation();
                case "take": return new TakeOperation();
                case "order": return new OrderOperation();
                case "asc": return new AscendingOperation();
                case "desc": return new DescendingOperation();
                default: throw new Exception();
            }
        }

        internal static Parser<ITerm> CallChain { get; } = Read.Ref(() =>
            from root in CallChainRoot
            from chunks in CallChainChunk.Many()
                //from name in Read.Optional(Name)
            select chunks//.Concat(new[] { name.GetOrDefault() }).Where(c => c != null)
            .Aggregate(root, (c1, c2) => { c1.GetLatestCall().Next = c2; return c1; })
            );

        internal static Parser<ITerm> CallChainRoot { get; } = Read.Ref(() =>
            from chunk in Property.XOr(Constant).XOr(Call)
            select chunk
        );

        internal static Parser<ITerm> CallChainChunk { get; } = Read.Ref(() =>
            from chunk in SubProperty.XOr(Call).XOr(Name)
            select chunk
        );        

        internal static Parser<ITerm> Call { get; } = Read.Ref(() => Lambda.XOr(Method));

        internal static Parser<List<ITerm>> CallArguments { get; } = Read.Ref(() =>
            from parameters in Read.Contained(Read.XDelimitedBy(CallChain, ArgumentDelimiter).Optional(), CallOpenBracket, CallCloseBracket).Optional()
            select parameters.GetOrDefault()?.GetOrDefault()?.Where(r => r != null)?.ToList() ?? new List<ITerm>()
        );

        internal static Parser<ITerm> Lambda { get; } = Read.Ref(() =>
            from semic in LambdaIndicator
            from method in MemberName
            from arguments in CallArguments
            select new LambdaTerm { Operation = SelectOperation(method), Arguments = arguments }
            );

        internal static Parser<ITerm> Method { get; } = Read.Ref(() =>
            from semic in MethodIndicator
            from method in MemberName
            from arguments in CallArguments
            select new MethodTerm { Operation = SelectOperation(method), Arguments = arguments }
            );

        internal static Parser<ITerm> SubProperty { get; } = Read.Ref(() =>
             from nav in PropertyNavigator
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
                BoolConstant,
                Array
            }.Aggregate((p1, p2) => p1.XOr(p2))
        );

        internal static Parser<ConstantTerm> Array { get; } = Read.Ref(() =>
            from constants in Read.Contained(Read.XDelimitedBy(Constant, ArgumentDelimiter).Optional(), ArrayOpenBracket, ArrayCloseBracket)
            select new ConstantTerm { Value = constants.GetOrDefault()?.Where(r => r != null)?.Select(c => c.Value)?.ToArray().AsQueryable() ?? new object[] { }.AsQueryable() }
        );

        internal static Parser<ConstantTerm> NumberConstant { get; } = Read.Ref(() =>
            from str in Read.Digit.XAtLeastOnce().Text()
            select new ConstantTerm { Value = Int32.Parse(str) }
        );        

        internal static Parser<ConstantTerm> StringConstant { get; } = Read.Ref(() =>
            from str in Read.Contained(Read.AnyChar.Except(StringDelimiter).Many().Text().Optional(), StringDelimiter, StringDelimiter)
            select new ConstantTerm { Value = str.GetOrDefault() ?? "" }
        );

        internal static Parser<ConstantTerm> BoolConstant { get; } = Read.Ref(() => TrueConstant.XOr(FalseConstant) );

        internal static Parser<ConstantTerm> TrueConstant { get; } = Read.Ref(() =>
            from str in TrueConstantString
            select new ConstantTerm { Value = true }
        );

        internal static Parser<ConstantTerm> FalseConstant { get; } = Read.Ref(() =>
            from str in FalseConstantString
            select new ConstantTerm { Value = false }
        );

        internal static Parser<NameTerm> Name { get; } = Read.Ref(() =>
            from at in Read.Char('@')
            from name in MemberName
            select new NameTerm { Name = name }
        );

        internal static Parser<string> MemberName { get; } =
            from str1 in Read.Letter.Once().Text()
            from str2 in Read.LetterOrDigit.XMany().Text()
            select str1 + str2;



    }
}
