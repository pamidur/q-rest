using System;
using System.Collections.Generic;
using QRest.Core.Terms;
using Sprache;
using Read = Sprache.Parse;
using System.Linq;

namespace QRest.Semantics.MethodChain
{
    public partial class MethodChainSemantics
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

        internal Parser<ITerm> CallChain() =>
           from root in CallChainRoot()
           from chunks in CallChainChunk().Many()
               //from name in Read.Optional(Name)
           select chunks//.Concat(new[] { name.GetOrDefault() }).Where(c => c != null)
          .Aggregate(root, (c1, c2) => { c1.GetLatestCall().Next = c2; return c1; });

        internal Parser<ITerm> CallChainRoot() =>
            from chunk in Property().XOr(Constant()).XOr(Call())
            select chunk;

        internal Parser<ITerm> CallChainChunk() =>
            from chunk in SubProperty().XOr(Call()).XOr(Name())
            select chunk;

        internal Parser<ITerm> Call() => Lambda().XOr(Method()).Named("Call");

        internal Parser<List<ITerm>> CallArguments() => Read.Ref(() =>
            from parameters in Read.Contained(Read.XDelimitedBy(CallChain(), ArgumentDelimiter).Optional(), CallOpenBracket, CallCloseBracket).Optional()
            select parameters.GetOrDefault()?.GetOrDefault()?.Where(r => r != null)?.ToList() ?? new List<ITerm>()
            );

        internal Parser<LambdaTerm> Lambda() =>
            (from semic in LambdaIndicator
             from method in MemberName
             from arguments in CallArguments()
             select new LambdaTerm { Operation = SelectOperation(method), Arguments = arguments }
            ).Named("Query");

        internal Parser<MethodTerm> Method() =>
            (from semic in MethodIndicator
             from method in MemberName
             from arguments in CallArguments()
             select new MethodTerm { Operation = SelectOperation(method), Arguments = arguments }
            ).Named("Method");

        internal Parser<ITerm> SubProperty() =>
             from nav in PropertyNavigator
             from prop in Property()
             select prop;

        internal Parser<ITerm> Property() =>
             from name in MemberName
             select new PropertyTerm { PropertyName = name };

        internal Parser<ConstantTerm> Constant() =>
            new Parser<ConstantTerm>[] {
                StringConstant(),
                NumberConstant(),
                BoolConstant(),
                Array()
            }.Aggregate((p1, p2) => p1.XOr(p2));

        internal Parser<ConstantTerm> Array() => Read.Ref(() =>
            from constants in Read.Contained(Read.XDelimitedBy(Constant(), ArgumentDelimiter).Optional(), ArrayOpenBracket, ArrayCloseBracket)
            select new ConstantTerm { Value = constants.GetOrDefault()?.Where(r => r != null)?.Select(c => c.Value)?.ToArray().AsQueryable() ?? new object[] { }.AsQueryable() }
            );

        internal Parser<ConstantTerm> NumberConstant() =>
            from strInt in Read.Digit.XAtLeastOnce().Text()
            from srtDecim in Read.Char('.').Then(c => Read.Digit.XAtLeastOnce().Text()).Optional()
            select new ConstantTerm { Value = srtDecim.IsDefined ? ParseConstant<float>($"{strInt}.{srtDecim.GetOrDefault()}") : ParseConstant<Int32>(strInt) };

        internal Parser<ConstantTerm> StringConstant() =>
            from str in Read.Contained(Read.AnyChar.Except(StringDelimiter).Many().Text().Optional(), StringDelimiter, StringDelimiter)
            select new ConstantTerm { Value = ParseConstant<string>(str.GetOrDefault() ?? "") };


        internal Parser<ConstantTerm> BoolConstant() =>
            from str in TrueConstantString.XOr(FalseConstantString)
            select new ConstantTerm { Value = ParseConstant<bool>(str) };


        internal Parser<NameTerm> Name() =>
            from at in Read.Char('@')
            from name in MemberName
            select new NameTerm { Name = name };


        internal Parser<string> MemberName { get; } =
            (from str1 in Read.Letter.Once().Text()
             from str2 in Read.LetterOrDigit.XMany().Text().Optional()
             select str1 + (str2.GetOrDefault() ?? "")).Named("Member");
    }
}
