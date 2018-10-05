using Antlr4.Runtime;
using QRest.Core.Contracts;
using QRest.Core.Terms;
using Xunit;

namespace QRest.OData.Tests
{
    public class ODataParserTest
    {
        [Theory]
        [InlineData(":where(-it.param1-equal(`L72`))-select(:select)", @"$filter =   param1 eq 'L72'")]
        [InlineData(":where(`L72`-equal(-it.param1))-select(:select)", @"$filter =  'L72' eq param1 ")]
        [InlineData(":where(-every(-it.param1-equal(`L72`),-oneof(-it.param2-equal(`qwerty`),-it.param3-equal(`asdf`))))-select(:select)",
            @"$filter = param1 eq 'L72' AND (param2 eq 'qwerty' OR param3 eq 'asdf') ")]
        public void ShouldParseFilterQueryOption(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, ((LambdaTerm)exp).ToString()); // "Debug" property is protected
        }

        [Fact]
        public void ShouldParseFuncAndNOT()
        {
            var input = @"$filter = not contains(param,'b')";
            ITerm exp = Parse(input);
            Assert.Equal(":where(-it.param-contains(`b`)-not)-select(:select)", exp.ToString());

        }

        [Theory]
        [InlineData(":where(-it.a-equal(-it.b))-select(:count,:select)", @"$filter = a eq b&$count=true")]
        [InlineData(":where(-it.a-equal(-it.b))-select(:select)", @"$filter = a eq b&$count=false")]
        public void ShouldParseCount(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.ToString());
        }


        [Fact]
        public void ShouldParseEmptyString()
        {
            ITerm exp = Parse(string.Empty);
            Assert.Equal("-select", exp.ToString());
        }

        [Theory]
        [InlineData(":where(-it.a-equal(-it.b))-select(:select(-it.f1,-it.f2))", @"$filter = a eq b&$count=false&$select=f1,f2")]
        [InlineData(":where(-it.a-equal(-it.b))-select(:count,:select(-it.f1,-it.f2))", @"$filter = a eq b&$count=true&$select=f1,f2")]
        public void ShouldParseSelect(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.ToString());
        }

        [Theory]
        [InlineData("-select(:select(-it.f1,-it.f2))", @"$count=false&$select=f1,f2&$skip=1&$top=1")]
        public void ShouldParseTopSkip(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.ToString());
        }


        [Theory]
        [InlineData("-select(:order(-it.f1-ascending,-it.f2-ascending):select)", @"$orderby=f1,f2")]
        [InlineData("-select(:order(-it.f1-ascending,-it.f2-descending):select)", @"$orderby=f1 asc,f2 desc" )]
        [InlineData("-select(:count,:order(-it.f1-ascending,-it.f2-descending):select)", @"$orderby=f1 asc,f2 desc&$count=true")]
        public void ShouldParseOrderBy(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.ToString());
        }


        private static ITerm Parse(string input)
        {
            ICharStream stream = CharStreams.fromstring(input);
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
