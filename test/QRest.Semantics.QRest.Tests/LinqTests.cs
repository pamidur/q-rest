using QRest.Core.Linq;
using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace QRest.Semantics.QRest.Tests
{
    public class LinqTests
    {
        private class TestClass
        {
            public string Data { get; set; }
        }

        [Fact]
        public void Can_Convert_Simple_Constant()
        {
            var converter = new DefaultExpressionToTermConverter();
            var term = converter.Convert<string>(Expression.Constant("ololo"));

            Assert.True(term is ConstantTerm);
            Assert.Equal("ololo", (term as ConstantTerm).Value);
        }

        [Fact]
        public void Can_Convert_Simple_Query()
        {
            var converter = new DefaultExpressionToTermConverter();

            var exp = new QRestQueryProvider(null,null).CreateQuery<TestClass>().Where(t => t.Data == "ololo").Expression;


            var term = converter.Convert<string>(exp);

            Assert.True(term is ConstantTerm);
            Assert.Equal("ololo", (term as ConstantTerm).Value);
        }
    }
}
