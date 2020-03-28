using Antlr4.Runtime;
using QRest.Core.Terms;
using QRest.Semantics.OData.Parsing;
using Xunit;

namespace QRest.OData.Tests
{
    public class ODataParserTest
    {
        [Theory]
        [InlineData("value=-where(:param1-eq('[L72]'))", @"$filter =   param1 eq '[L72]'")]
        [InlineData("value=-where(:'L72'-eq(param1))", @"$filter =  'L72' eq param1 ")]
        [InlineData("value=-where(:-every(param1-eq('L72'),-oneof(param2-eq('qwerty'),param3-eq('asdf'))))",
            @"$filter = param1 eq 'L72' AND (param2 eq 'qwerty' OR param3 eq 'asdf') ")]
        public void ShouldParseFilterQueryOption(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.ViewQuery);
        }

        [Fact]
        public void ShouldParseFuncAndNOT()
        {
            var input = @"$filter = not contains(param,'b')";
            ITerm exp = Parse(input);
            Assert.Equal("value=-where(:param-has('b')-not)", exp.ViewQuery);

        }

        [Theory]
        [InlineData("value=-where(:a-eq(b));count=-where(:a-eq(b))-count", @"$filter = a eq b&$count=true")]
        [InlineData("value=-where(:a-eq(b))", @"$filter = a eq b&$count=false")]
        public void ShouldParseCount(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.ViewQuery);
        }


        [Fact]
        public void ShouldParseEmptyString()
        {
            ITerm exp = Parse(string.Empty);
            Assert.Equal("value=$$", exp.ViewQuery);
        }

        [Theory]
        [InlineData("value=-where(:a-eq('2019-12-22T21:02:00.3434Z'))", @"$filter = a eq 2019-12-22T21:02:00.3434Z")]
        [InlineData("value=-where(:a-eq('2019-12-22T21:02:00.34346767+02:00'))", @"$filter = a eq 2019-12-22T21:02:00.34346767+02:00")]
        [InlineData("value=-where(:a-eq('2019-12-22T21:02:00-03:00'))", @"$filter = a eq 2019-12-22T21:02:00-03:00")]
        public void ShouldParseDateTimeOffset(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.ViewQuery);
        }

        [Theory]
        [InlineData("value=-where(:a-ne('2019-12-22T21:02:00.3434Z'))", @"$filter = a ne 2019-12-22T21:02:00.3434Z")]
        public void ShouldParseNotEqual(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.ViewQuery);
        }

        [Theory]
        [InlineData("value=-where(:a-eq('AF41F4AE-4FD2-4505-8FEF-CD7612C948D7'))", @"$filter = a eq {AF41F4AE-4FD2-4505-8FEF-CD7612C948D7}")]
        [InlineData("value=-where(:a-eq('AF41F4AE-4FD2-4505-8FEF-CD7612C948D7'))", @"$filter = a eq AF41F4AE-4FD2-4505-8FEF-CD7612C948D7")]
        [InlineData("value=-where(:a-eq('AF41F4AE-4FD2-4505-8FEF-CD7612C948D7'))", @"$filter = a eq (AF41F4AE-4FD2-4505-8FEF-CD7612C948D7)")]
        public void ShouldParseGuid(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.ViewQuery);
        }

        [Theory]
        [InlineData("value=-where(:a-eq(b))-map(:-new(f1,f2))", @"$filter = a eq b&$count=false&$select=f1,f2")]
        [InlineData("value=-where(:a-eq(b))-map(:-new(f1,f2));count=-where(:a-eq(b))-count", @"$filter = a eq b&$count=true&$select=f1,f2")]
        public void ShouldParseEach(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.ViewQuery);
        }

        [Theory]
        [InlineData("value=-skip('1')-take('1')-map(:-new(f1,f2))", @"$count=false&$select=f1,f2&$skip=1&$top=1")]
        public void ShouldParseTopSkip(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.ViewQuery);
        }

        [Theory]
        [InlineData("value=-where(:Contacts-any(:-every(Email-eq('test@gmail.com'),-eq('test1@gmail.com'))))", @"$filter = Contacts/any(c: c/Email eq 'test@gmail.com' and c eq 'test1@gmail.com')")]
        [InlineData("value=-where(:Contacts-all(:-oneof(Email-eq('test@gmail.com'),-eq('test1@gmail.com'))))", @"$filter = Contacts/all(c: c/Email eq 'test@gmail.com' or c eq 'test1@gmail.com')")]
        public void ShouldParseLambda(string expected, string input)
        {
            ITerm exp = Parse(input); 
            Assert.Equal(expected, exp.ViewQuery);
        }


        [Theory]
        [InlineData("value=-order(:f1)", @"$orderby=f1 asc")]
        [InlineData("value=-order(:f1,:f2)", @"$orderby=f1,f2")]
        [InlineData("value=-order(:f1,:f2,:f3)", @"$orderby=f1,f2,f3")]
        [InlineData("value=-order(:f1,:f2-desc)", @"$orderby=f1 asc,f2 desc")]
        [InlineData("value=-order(:f1,:f2-desc);count=-count", @"$orderby=f1 asc,f2 desc&$count=true")]
        [InlineData("value=-order(:f1);count=-count", @"$orderby=f1 asc&$count=true")]
        public void ShouldParseOrderBy(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.ViewQuery);
        }

        [Fact]
        public void ShouldParseBool()
        {
            ITerm exp = Parse(@"$filter = a eq false");
            Assert.Equal("value=-where(:a-eq(false))", exp.ViewQuery);
        }

        [Fact]
        public void ShouldParseNull()
        {
            ITerm exp = Parse(@"$filter = a eq null");
            Assert.Equal("value=-where(:a-eq(null))", exp.ViewQuery);
        }

        private static ITerm Parse(string input)
        {
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new ODataGrammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);

            var parser = new ODataGrammarParser(tokens);
            parser.AddErrorListener(new ODataParserErrorListener());

            var context = parser.parse();

            var vis = new ODataVisitor(new ODataOperationMap());
            var exp = vis.Visit(context);
            return exp;
        }
    }
}
