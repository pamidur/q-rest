using QRest.Core.Contracts;
using System.Collections.Generic;
using QRest.Core.Terms;
using Sprache;
using System.Linq;
using QRest.Core.Operations;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations.Query;
using QRest.Core.Operations.Aggregations;
using QRest.Core.Operations.Query.OrderDirectionOperations;
using QRest.Core;
using System;
using System.Globalization;
using QRest.Core.Exceptions;

namespace QRest.Semantics.MethodChain
{
    public partial class MethodChainSemantics : IQuerySemanticsProvider
    {
        private readonly Dictionary<string, IOperation> _operationMap = new Dictionary<string, IOperation>();

        private Lazy<Parser<ITerm>> Parser { get; }

        public MethodChainSemantics()
        {
            Parser = new Lazy<Parser<ITerm>>(() => PrepareParser());
        }

        private Parser<ITerm> PrepareParser()
        {
            AddDefaultOperations();

            foreach (var customOp in CustomOperations)
                AddOperation(customOp.Key, customOp.Value);

            return CallChain(this).End();
        }

        protected internal void AddDefaultOperations()
        {
            AddOperation("ne", new NotEqualOperation());
            AddOperation("eq", new EqualOperation());
            AddOperation("not", new NotOperation());
            AddOperation("where", new WhereOperation());
            AddOperation("get", new SelectOperation { UseStaticTerminatingQuery = UseStaticQueryTerminator });
            AddOperation("oneof", new OneOfOperation());
            AddOperation("every", new EveryOperation());
            AddOperation("first", new FirstOperation());
            AddOperation("count", new CountOperation());
            AddOperation("sum", new SumOperation());
            AddOperation("has", new ContainsOperation());
            AddOperation("skip", new SkipOperation());
            AddOperation("take", new TakeOperation());
            AddOperation("order", new OrderOperation());
            AddOperation("asc", new AscendingOperation());
            AddOperation("desc", new DescendingOperation());


            AddOperation("with", new WithOperation());
            AddOperation("it", new ItOperation());
        }

        protected void AddOperation(string name, IOperation operation)
        {
            if (operation is CompareOperationBase && UseDefferedConstantParsing >= DefferedConstantParsing.Strings)
            {
                var compareOp = (CompareOperationBase)operation;

                compareOp.TryParseFromStrings = true;

                foreach (var parser in CustomConstantParsers)
                    compareOp.Parsers.Add(parser.Key, parser.Value);
            }

            _operationMap.Add(name, operation);
        }

        public ITerm Parse(IReadOnlyDictionary<string, string[]> queryParts)
        {
            var query = queryParts.FirstOrDefault().Value.FirstOrDefault();

            if (string.IsNullOrEmpty(query))
                return null;

            var result = Parser.Value.TryParse(query);            

            return result.Value;
        }

        public string[] QuerySelector(string modelname)
        {
            return new[] { modelname };
        }

        protected object ParseConstant<T>(string source)
        {
            var type = typeof(T);

            if (UseDefferedConstantParsing == DefferedConstantParsing.All || type == typeof(string))
                return source;

            if (type == typeof(bool))
                return bool.Parse(source);

            if (UseDefferedConstantParsing == DefferedConstantParsing.StringsAndNumbers)
                return source;

            if (type == typeof(int))
                return int.Parse(source, CultureInfo.InvariantCulture);

            if (type == typeof(float))
                return float.Parse(source, CultureInfo.InvariantCulture);

            if (UseDefferedConstantParsing == DefferedConstantParsing.Strings)
                return source;

            throw new NotSupportedException($"Cannot parse type of {type.ToString()}");
        }

        protected IOperation SelectOperation(string opName)
        {
            if (!_operationMap.TryGetValue(opName, out var op))
                throw new UnknownOperationException(opName);

            return op;
        }
    }
}
