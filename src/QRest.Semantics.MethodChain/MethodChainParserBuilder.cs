using QRest.Core.Contracts;
using QRest.Core.Terms;
using Sprache;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Read = Sprache.Parse;

namespace QRest.Semantics.MethodChain
{
    public class MethodChainParserBuilder
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

        internal static readonly Parser<IEnumerable<char>> TrueConstantString = Read.String("true").Token();
        internal static readonly Parser<IEnumerable<char>> FalseConstantString = Read.String("false").Token();


        private readonly DefferedConstantParsing _defferedParsing;
        private readonly Dictionary<string, IOperation> _operationMap;


        internal Parser<ITermSequence> CallChain;
        internal Parser<ITerm> Call;
        internal Parser<List<ITermSequence>> CallArguments;
        internal Parser<LambdaTerm> Lambda;
        internal Parser<MethodTerm> Method;
        internal Parser<ITerm> SubProperty;
        internal Parser<ITerm> Property;
        internal Parser<ConstantTerm> Constant;
        internal Parser<ConstantTerm> ArrayConstant;
        internal Parser<ConstantTerm> NumberConstant;
        internal Parser<ConstantTerm> StringConstant;
        internal Parser<ConstantTerm> BoolConstant;
        internal Parser<NameTerm> Name;
        internal Parser<string> MemberName;

        public MethodChainParserBuilder(DefferedConstantParsing defferedParsing, Dictionary<string, IOperation> operationMap)
        {
            _defferedParsing = defferedParsing;
            _operationMap = operationMap;
        }

        public Parser<ITermSequence> Build()
        {
            MemberName = BuildMemberNameParser().Named("Member Name");

            BoolConstant = BuildBoolConstantParser().Named("Boolean Constant");
            StringConstant = BuildStringConstantParser().Named("String Constant");
            NumberConstant = BuildNumberConstantParser().Named("Number Constant");
            ArrayConstant = BuildArrayConstantParser().Named("Array Constant");
            Constant = BuildConstantParser().Named("Constant");

            Property = BuildPropertyParser().Named("Property");
            SubProperty = BuildSubPropertyParser().Named("SubProperty");

            Method = BuildMethodParser(BuildOperationParsers(_operationMap.Where(m => m.Value.SupportsCall).ToArray())).Named("Method Call");
            Lambda = BuildLambdaParser(BuildOperationParsers(_operationMap.Where(m => m.Value.SupportsQuery).ToArray())).Named("Query Call");
            Call = Lambda.XOr(Method).Named("Call");

            Name = BuildNameTermParser().Named("Name");

            CallArguments = BuildCallArgumentsParser().Named("Call Arguments");
            CallChain = BuildCallChainParser().Named("Expression");

            return CallChain.End();
        }

        internal Parser<ITermSequence> BuildCallChainParser() =>
          from root in Call.Or(Property).Or(Constant)
          from chunks in SubProperty.Or(Call).Or(Name).Many()
          select chunks.Aggregate(new TermSequence() { root }, (c1, c2) => { c1.Add(c2); return c1; });

        internal Parser<List<ITermSequence>> BuildCallArgumentsParser() => Read.Ref(() =>
            from parameters in Read.Contained(Read.DelimitedBy(CallChain, ArgumentDelimiter), CallOpenBracket, CallCloseBracket)
            select parameters.Where(r => r != null)?.ToList() ?? new List<ITermSequence>()
            );

        internal Parser<LambdaTerm> BuildLambdaParser(Parser<IOperation> operationParser) =>
             from semic in LambdaIndicator
             from operation in operationParser
             from arguments in CallArguments.Optional()
             select new LambdaTerm { Operation = operation, Arguments = arguments.GetOrDefault() ?? new List<ITermSequence>() };

        internal Parser<MethodTerm> BuildMethodParser(Parser<IOperation> operationParser) =>
             from semic in MethodIndicator
             from operation in operationParser
             from arguments in CallArguments.Optional()
             select new MethodTerm { Operation = operation, Arguments = arguments.GetOrDefault() ?? new List<ITermSequence>() };

        internal Parser<IOperation> BuildOperationParsers(KeyValuePair<string, IOperation>[] operationMap)
        {
            if (operationMap.Length == 0)
                throw new NotSupportedException("Must supply at least one opration.");

            Parser<IOperation> parser = null;

            foreach (var map in operationMap)
            {
                var next = BuildOperationParser(map.Key, map.Value).Named($"'{map.Key}'");
                parser = parser == null ? next : parser.Or(next);
            }

            return parser;
        }

        internal Parser<IOperation> BuildOperationParser(string name, IOperation operation) =>
            from method in Read.String(name).Token()
            select operation;


        internal Parser<ITerm> BuildSubPropertyParser() =>
             from nav in PropertyNavigator
             from prop in Property
             select prop;

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
            select new ConstantTerm { Value = constants.Where(r => r != null).Select(c => c.Value).ToArray().AsQueryable() }
            );

        internal Parser<ConstantTerm> BuildNumberConstantParser() =>
            from strInt in Read.Digit.XAtLeastOnce().Text()
            from srtDecim in Read.Char('.').Then(c => Read.Digit.XAtLeastOnce().Text()).Optional()
            select new ConstantTerm { Value = srtDecim.IsDefined ? ParseConstant<float>($"{strInt}.{srtDecim.GetOrDefault()}") : ParseConstant<Int32>(strInt) };

        internal Parser<ConstantTerm> BuildStringConstantParser() =>
            from str in Read.Contained(Read.AnyChar.Except(StringDelimiter).Many().Text().Optional(), StringDelimiter, StringDelimiter)
            select new ConstantTerm { Value = ParseConstant<string>(str.GetOrDefault() ?? "") };

        internal Parser<ConstantTerm> BuildBoolConstantParser() =>
            from str in TrueConstantString.XOr(FalseConstantString).Text()
            select new ConstantTerm { Value = ParseConstant<bool>(str) };

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

