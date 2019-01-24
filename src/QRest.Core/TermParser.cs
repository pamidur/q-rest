using QRest.Core.Contracts;
using QRest.Core.Operations;
using QRest.Core.Terms;
using Sprache;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Read = Sprache.Parse;

namespace QRest.Core
{
    public enum DefferedConstantParsing : byte
    {
        /// <summary>
        /// Don't use deffered parsing.
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// Use only for string values. Allows '{Guid}' and '{DateTime}' to be parsed.
        /// </summary>
        Strings = 1,
        /// <summary>
        /// Use for strings and number values. Useful for cases when need long int suport and additional precision for decimals and doubles
        /// </summary>
        StringsAndNumbers = 3,
        /// <summary>
        /// Use for all values
        /// </summary>
        All = 4,
    }

    public class TermParser
    {
        public static Parser<RootTerm> Default { get; }

        static TermParser() => Default = new TermParser(DefferedConstantParsing.StringsAndNumbers, OperationsMap.GetRegisteredOperationNames(), OperationsMap.LookupOperation).Build();

        internal static readonly Parser<char> StringDelimiter = Read.Char('\'');
        internal static readonly Parser<char> ArgumentDelimiter = Read.Char(',');

        internal static readonly Parser<char> CallOpenBracket = Read.Char('(');
        internal static readonly Parser<char> CallCloseBracket = Read.Char(')');

        internal static readonly Parser<char> ArrayOpenBracket = Read.Char('[');
        internal static readonly Parser<char> ArrayCloseBracket = Read.Char(']');

        internal static readonly Parser<char> MethodIndicator = Read.Char('-');
        internal static readonly Parser<char> LambdaIndicator = Read.Char(':');

        internal static readonly Parser<char> PropertyNavigator = Read.Char('.');

        internal static readonly Parser<char> NameIndicator = Read.Char('@');


        internal static readonly Parser<IEnumerable<char>> TrueConstantString = Read.String("true").Token();
        internal static readonly Parser<IEnumerable<char>> FalseConstantString = Read.String("false").Token();


        private readonly DefferedConstantParsing _defferedParsing;
        private readonly IReadOnlyList<string> _operations;
        private readonly Func<string, IOperation> _selector;

        internal Parser<SequenceTerm> CallChain;
        internal Parser<RootTerm> Root;
        internal Parser<MethodTerm> Call;
        internal Parser<List<SequenceTerm>> CallArguments;
        internal Parser<ITerm> SubProperty;
        internal Parser<ITerm> Property;
        internal Parser<LambdaTerm> Lambda;
        internal Parser<SequenceTerm> RootProperty;
        internal Parser<ITerm> ChainRoot;
        internal Parser<ConstantTerm> Constant;
        internal Parser<ConstantTerm> ArrayConstant;
        internal Parser<ConstantTerm> NumberConstant;
        internal Parser<ConstantTerm> StringConstant;
        internal Parser<ConstantTerm> BoolConstant;
        internal Parser<NameTerm> Name;
        internal Parser<string> MemberName;

        public TermParser(
            DefferedConstantParsing defferedParsing,
            IReadOnlyList<string> operations,
            Func<string, IOperation> selector)
        {
            _defferedParsing = defferedParsing;
            _operations = operations;
            _selector = selector;
        }

        public Parser<RootTerm> Build()
        {
            MemberName = BuildMemberNameParser().Named("Member Name");

            BoolConstant = BuildBoolConstantParser().Named("Boolean Constant");
            StringConstant = BuildStringConstantParser().Named("String Constant");
            NumberConstant = BuildNumberConstantParser().Named("Number Constant");
            ArrayConstant = BuildArrayConstantParser().Named("Array Constant");
            Constant = BuildConstantParser().Named("Constant");


            Property = BuildPropertyParser().Named("Property");
            Lambda = BuildLambdaParser().Named("Lambda");
            SubProperty = BuildSubPropertyParser().Named("SubProperty");
            RootProperty = BuildRootPropertyParser().Named("Property");

            Call = BuildMethodParser(BuildOperationParsers()).Named("Call");

            Name = BuildNameTermParser().Named("Name");

            ChainRoot = BuildChainRootParser();
            CallArguments = BuildCallArgumentsParser().Named("Call Arguments");
            CallChain = BuildCallChainParser();

            Root = BuildTopLambdaParser().Named("Root");

            return Root.End();
        }

        internal Parser<RootTerm> BuildTopLambdaParser() =>
          from seq in CallChain
          select new RootTerm(seq);

        internal Parser<ITerm> BuildChainRootParser() =>
          from root in Call.XOr<ITerm>(Lambda).XOr(Constant)
          select root.AsSequence();

        internal Parser<SequenceTerm> BuildCallChainParser() =>
          from root in ChainRoot.XOr(RootProperty)
          from chunks in SubProperty.XOr(Call).XOr(Name).XMany()
          select chunks.Aggregate(new List<ITerm> { root }, (c1, c2) => { c1.Add(c2); return c1; }, acc => new SequenceTerm(acc.ToArray()));

        internal Parser<List<SequenceTerm>> BuildCallArgumentsParser() => Read.Ref(() =>
            from parameters in Read.Contained(Read.XDelimitedBy(CallChain, ArgumentDelimiter).XOptional(), CallOpenBracket, CallCloseBracket)
            select parameters.GetOrDefault()?.Where(r => r != null)?.ToList() ?? new List<SequenceTerm>()
            );

        internal Parser<LambdaTerm> BuildLambdaParser() =>
            from semic in LambdaIndicator
            from seq in CallChain
            select new LambdaTerm(seq);

        internal Parser<MethodTerm> BuildMethodParser(Parser<IOperation> operationFactoryParser) =>
             from semic in MethodIndicator
             from operation in operationFactoryParser
             from arguments in CallArguments.XOptional()
             select new MethodTerm(operation, arguments.GetOrDefault()?.ToArray() ?? new SequenceTerm[] { });

        internal Parser<IOperation> BuildOperationParsers()
        {
            if (_operations.Count == 0)
                throw new NotSupportedException("Must supply at least one opration.");

            Parser<IOperation> parser = null;

            foreach (var opname in _operations)
            {
                var next = BuildOperationFactory(opname).Named($"'{opname}'");
                parser = parser == null ? next : parser.Or(next);
            }

            return parser;
        }

        internal Parser<IOperation> BuildOperationFactory(string name) =>
            from method in Read.String(name)
            select _selector(string.Join("", method));


        internal Parser<ITerm> BuildSubPropertyParser() =>
             from nav in PropertyNavigator
             from prop in Property
             select prop;

        internal Parser<SequenceTerm> BuildRootPropertyParser() =>
             from prop in Property
             select new SequenceTerm(new[] { new MethodTerm(new RootOperation()), prop });

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
            from at in NameIndicator
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

