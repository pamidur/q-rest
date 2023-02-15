using QRest.Core.Parsing;
using Sprache;
using Xunit;

namespace QRest.Semantics.QRest.Tests
{
    public class ComplexParserTests
    {

        [Theory]
        [InlineData("-where(:a-eq(b))", "-where(:#$$.a-eq(#$$.b))")] //implicit root reference
        [InlineData("-new($,$-count,$my,$$)", "-new($,#$-count,$my,$$)")] //context is parsed
        public void ComplexTests(string input, string expected)
        {
            var term = TermParser.Default.Parse(input);
            Assert.Equal(expected, term.ViewDebug);
            Assert.Equal(input, term.ViewQuery);
        }
    }
}
