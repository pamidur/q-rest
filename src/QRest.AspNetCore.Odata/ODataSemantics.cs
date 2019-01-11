using Antlr4.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRest.AspNetCore.Contracts;
using QRest.Core;
using QRest.Core.Terms;
using QRest.OData;
using System.Collections.Generic;
using System.Linq;

namespace QRest.AspNetCore.OData
{
    public class ODataSemantics : ISemantics
    {
        public IQueryStructure ReadQueryStructure(IReadOnlyList<string> values, HttpRequest request)
        {
            var clauses = new[] { "$filter", "$select", "$orderby", "$count", "$top", "$skip" };

            var strings = new List<string>();

            foreach (var c in clauses)            
                if (request.Query.TryGetValue(c, out var strs))
                    strings.Add($"{c}={strs.First()}");    

            ICharStream stream = CharStreams.fromstring(string.Join("&", strings));
            ITokenSource lexer = new ODataGrammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);

            var parser = new ODataGrammarParser(tokens);
            var context = parser.parse();

            var vis = new ODataVisitor();
            var exp = vis.Visit(context);

            var lambda = new LambdaTerm(BuiltIn.Roots.OriginalRoot, exp);

            return new ODataQueryStructure { Data = lambda };
        }

        public ActionResult WriteQueryResponse(IQueryStructure query, IReadOnlyDictionary<LambdaTerm, object> results)
        {
            var result = results[query.Data];
            return new ODataActionResult(result);
        }
    }
}
