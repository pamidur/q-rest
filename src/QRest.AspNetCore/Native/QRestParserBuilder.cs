using QRest.Core;
using QRest.Core.Contracts;
using QRest.Core.Operations;
using QRest.Core.Terms;
using Sprache;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Read = Sprache.Parse;

namespace QRest.AspNetCore.Native
{
    public class QRestParserBuilder
    {
        internal static readonly Parser<char> StringDelimiter = Read.Char('`');
        internal static readonly Parser<char> ArgumentDelimiter = Read.Char(',');

        internal static readonly Parser<char> CallOpenBracket = Read.Char('(');
        internal static readonly Parser<char> CallCloseBracket = Read.Char(')');

        internal static readonly Parser<char> ArrayOpenBracket = Read.Char('[');
        internal static readonly Parser<char> ArrayCloseBracket = Read.Char(']');

        internal static readonly Parser<char> MethodIndicator = Read.Char('-');

        internal static readonly Parser<char> PropertyNavigator = Read.Char('.');

        internal static readonly Parser<IEnumerable<char>> TrueConstantString = Read.String("true").Token();
        internal static readonly Parser<IEnumerable<char>> FalseConstantString = Read.String("false").Token();


        private readonly DefferedConstantParsing _defferedParsing;
        private readonly Dictionary<string, Func<SequenceTerm[], MethodTerm>> _callMap;
        internal Parser<SequenceTerm> CallChain;
        internal Parser<LambdaTerm> TopLambda;
        internal Parser<MethodTerm> Call;
        internal Parser<List<SequenceTerm>> CallArguments;
        internal Parser<ITerm> SubProperty;
        internal Parser<ITerm> Property;
        internal Parser<SequenceTerm> RootProperty;
        internal Parser<ITerm> ChainRoot;
        internal Parser<ConstantTerm> Constant;
        internal Parser<ConstantTerm> ArrayConstant;
        internal Parser<ConstantTerm> NumberConstant;
        internal Parser<ConstantTerm> StringConstant;
        internal Parser<ConstantTerm> BoolConstant;
        internal Parser<NameTerm> Name;
        internal Parser<string> MemberName;

        public QRestParserBuilder(
            DefferedConstantParsing defferedParsing,
            Dictionary<string, Func<SequenceTerm[], MethodTerm>> callMap)
        {
            _defferedParsing = defferedParsing;
            _callMap = callMap;
        }

        public Parser<LambdaTerm> Build()
        {
            MemberName = BuildMemberNameParser().Named("Member Name");

            BoolConstant = BuildBoolConstantParser().Named("Boolean Constant");
            StringConstant = BuildStringConstantParser().Named("String Constant");
            NumberConstant = BuildNumberConstantParser().Named("Number Constant");
            ArrayConstant = BuildArrayConstantParser().Named("Array Constant");
            Constant = BuildConstantParser().Named("Constant");


            Property = BuildPropertyParser().Named("Property");
            SubProperty = BuildSubPropertyParser().Named("SubProperty");
            RootProperty = BuildRootPropertyParser().Named("Property");

            Call = BuildMethodParser(BuildOperationParsers(_callMap)).Named("Call");

            Name = BuildNameTermParser().Named("Name");

            ChainRoot = BuildChainRootParser().Named("Call or Constant");
            CallArguments = BuildCallArgumentsParser().Named("Call Arguments");
            CallChain = BuildCallChainParser().Named("Expression");

            TopLambda = BuildTopLambdaParser().Named("Lambda");

            return TopLambda.End();
        }

        internal Parser<LambdaTerm> BuildTopLambdaParser() =>
          from seq in CallChain
          select new LambdaTerm(BuiltIn.Roots.OriginalRoot, seq);

        internal Parser<ITerm> BuildChainRootParser() =>
          from root in Call.Or<ITerm>(Constant)
          select root.AsSequence();

        internal Parser<SequenceTerm> BuildCallChainParser() =>
          from root in ChainRoot.Or(RootProperty)
          from chunks in SubProperty.Or(Call).Or(Name).Many()
          select chunks.Aggregate(new List<ITerm> { root }, (c1, c2) => { c1.Add(c2); return c1; }, acc=> new SequenceTerm(acc.ToArray()));

        internal Parser<List<SequenceTerm>> BuildCallArgumentsParser() => Read.Ref(() =>
            from parameters in Read.Contained(Read.DelimitedBy(CallChain, ArgumentDelimiter), CallOpenBracket, CallCloseBracket)
            select parameters.Where(r => r != null)?.ToList() ?? new List<SequenceTerm>()
            );

        internal Parser<MethodTerm> BuildMethodParser(Parser<Func<SequenceTerm[], MethodTerm>> operationFactoryParser) =>
             from semic in MethodIndicator
             from operation in operationFactoryParser
             from arguments in CallArguments.Optional()
             select operation(arguments.GetOrDefault()?.ToArray() ?? new SequenceTerm[] { });

        internal Parser<Func<SequenceTerm[], MethodTerm>> BuildOperationParsers(Dictionary<string, Func<SequenceTerm[], MethodTerm>> operationMap)
        {
            if (operationMap.Count == 0)
                throw new NotSupportedException("Must supply at least one opration.");

            Parser<Func<SequenceTerm[], MethodTerm>> parser = null;

            foreach (var map in operationMap)
            {
                var next = BuildOperationFactory(map.Key, map.Value).Named($"'{map.Key}'");
                parser = parser == null ? next : parser.Or(next);
            }

            return parser;
        }

        internal Parser<Func<SequenceTerm[], MethodTerm>> BuildOperationFactory(string name, Func<SequenceTerm[], MethodTerm> factory) =>
            from method in Read.String(name).Token()
            select factory;


        internal Parser<ITerm> BuildSubPropertyParser() =>
             from nav in PropertyNavigator
             from prop in Property
             select prop;

        internal Parser<SequenceTerm> BuildRootPropertyParser() =>
             from prop in Property
             select new SequenceTerm(new[] { new MethodTerm(new ItOperation()), prop });

        internal Parser<ITerm> BuildPropertyParser() =>
             from name in MemberName
             select new PropertyTerm(name);

        internal Parser<ConstantTerm> BuildConstantParser() =>
            new Parser<ConstantTerm>[] {
                StringConstant,
                NumberConstant,
                BoolConstant,
                ArrayConstant
            }.Aggregate((p1, p2) => p1.Or(p2));

        internal Parser<ConstantTerm> BuildArrayConstantParser() => Read.Ref(() =>
            from constants in Read.Contained(Read.DelimitedBy(Constant, ArgumentDelimiter), ArrayOpenBracket, ArrayCloseBracket)
            select new ConstantTerm(constants.Where(r => r != null).Select(c => c.Value).ToArray().AsQueryable())
            );

        internal Parser<ConstantTerm> BuildNumberConstantParser() =>
            from strInt in Read.Digit.XAtLeastOnce().Text()
            from srtDecim in Read.Char('.').Then(c => Read.Digit.XAtLeastOnce().Text()).Optional()
            select new ConstantTerm(srtDecim.IsDefined ? ParseConstant<float>($"{strInt}.{srtDecim.GetOrDefault()}") : ParseConstant<Int32>(strInt));

        internal Parser<ConstantTerm> BuildStringConstantParser() =>
            from str in Read.Contained(Read.AnyChar.Except(StringDelimiter).Many().Text().Optional(), StringDelimiter, StringDelimiter)
            select new ConstantTerm(ParseConstant<string>(str.GetOrDefault() ?? ""));

        internal Parser<ConstantTerm> BuildBoolConstantParser() =>
            from str in TrueConstantString.XOr(FalseConstantString).Text()
            select new ConstantTerm(ParseConstant<bool>(str));

        protected Parser<NameTerm> BuildNameTermParser() =>
            from at in Read.Char('@')
            from name in MemberName
            select new NameTerm(name);

        protected Parser<string> BuildMemberNameParser() =>
            from str1 in Read.Letter.Once().Text()
            from str2 in Read.LetterOrDigit.XMany().Text().Optional()
            select str1 + (str2.GetOrDefault() ?? "");


        protected object ParseConstant<T>(string source)
        {
            var type = typeof(T);

            if (_defferedParsing == DefferedConstantParsing.All || type == typeof(string))
                return source;

            if (type == typeof(bool))
                return bool.Parse(source);

            if (_defferedParsing == DefferedConstantParsing.StringsAndNumbers)
                return source;

            if (type == typeof(int))
                return int.Parse(source, CultureInfo.InvariantCulture);

            if (type == typeof(float))
                return float.Parse(source, CultureInfo.InvariantCulture);

            if (_defferedParsing == DefferedConstantParsing.Strings)
                return source;

            throw new NotSupportedException($"Cannot parse type of {type.ToString()}");
        }
    }
}

