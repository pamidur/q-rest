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
        private static readonly NativeQueryStructure _default = new NativeQueryStructure { Data = new LambdaTerm(BuiltIn.Roots.OriginalRoot, new MethodTerm(new ItOperation()).AsSequence()) };

        private Lazy<Parser<LambdaTerm>> Parser { get; }

        public NativeSemantics()
        {
            Parser = new Lazy<Parser<LambdaTerm>>(() => PrepareParser());
        }

        private Parser<LambdaTerm> PrepareParser()
        {
            return new QRestParserBuilder(UseDefferedConstantParsing, _callMap).Build();
        }

        public ActionResult WriteQueryResponse(IQueryStructure query, IReadOnlyDictionary<LambdaTerm, object> results)
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
