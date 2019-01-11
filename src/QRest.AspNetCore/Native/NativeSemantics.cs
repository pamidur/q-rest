using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRest.AspNetCore.Contracts;
using QRest.Core;
using QRest.Core.Operations;
using QRest.Core.Terms;
using Sprache;
using System;
using System.Collections.Generic;

namespace QRest.AspNetCore.Native
{
    public partial class NativeSemantics : ISemantics
    {
        private static readonly NativeQueryStructure _default = new NativeQueryStructure { Data = new RootTerm(new MethodTerm(new RootOperation()).AsSequence()) };

        private Lazy<Parser<RootTerm>> Parser { get; }

        public NativeSemantics()
        {
            Parser = new Lazy<Parser<RootTerm>>(() => PrepareParser());
        }

        private Parser<RootTerm> PrepareParser()
        {
            return new TermParser(UseDefferedConstantParsing, OperationsMap.GetRegisteredOperationNames(), OperationsMap.LookupOperation).Build();
        }

        public ActionResult WriteQueryResponse(IQueryStructure query, IReadOnlyDictionary<RootTerm, object> results)
        {
            var result = results[query.Data];
            return new OkObjectResult(result);
        }

        public IQueryStructure ReadQueryStructure(IReadOnlyList<string> values, HttpRequest request)
        {
            if (values.Count == 0)
                return _default;

            var result = Parser.Value.TryParse(values[0]);

            return new NativeQueryStructure { Data = result.Value };
        }
    }
}
