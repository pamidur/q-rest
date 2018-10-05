using Antlr4.Runtime;
using QRest.Core.Contracts;
using QRest.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QRest.Semantics.OData
{
    public class ODataSemantics : IQuerySemanticsProvider
    {

        public ITerm Parse(IRequestModel model)
        {
            var clauses = new[] { "$filter", "$select", "$orderby", "$count", "$top", "$skip" };

            var strings = clauses.Where(c => model.GetNamedQueryPart(c).Length > 0)
                .Select(c => $"{c}={model.GetNamedQueryPart(c).ToArray().First()}");

            ICharStream stream = CharStreams.fromstring(string.Join("&",strings));
            ITokenSource lexer = new ODataGrammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);

            var parser = new ODataGrammarParser(tokens);
            var context = parser.parse();

            var vis = new ODataVisitor();
            var exp = vis.Visit(context);

            return exp;
        }
    }
}
