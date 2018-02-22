using Antlr4.Runtime;
using QRest.Core.Terms;
using Xunit;

namespace QRest.OData.Tests
{
    public class ODataParserTest
    {
        [Theory]
        [InlineData(":where(-it.param1-equal(`L72`))", @"$filter =   param1 eq 'L72'")]
        [InlineData(":where(`L72`-equal(-it.param1))", @"$filter =  'L72' eq param1 ")]
        [InlineData(":where(-every(-it.param1-equal(`L72`),-oneof(-it.param2-equal(`qwerty`),-it.param3-equal(`asdf`))))",
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
            Assert.Equal(":where(-it.param-contains(`b`)-not)", exp.ToString());

        }

        [Theory]
        [InlineData(":where(-it.a-equal(-it.b)):count", @"$filter = a eq b&$count=true")]
        [InlineData(":where(-it.a-equal(-it.b))", @"$filter = a eq b&$count=false")]
        public void ShouldParseCount(string expected, string input)
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
