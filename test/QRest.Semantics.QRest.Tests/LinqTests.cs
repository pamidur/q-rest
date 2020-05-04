using QRest.Core.Linq;
using QRest.Core.Operations;
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
        private DefaultExpressionToTermConverter _converter;
        private IQueryable<TestClass> _testQuery;

        public LinqTests()
        {
            _converter = new DefaultExpressionToTermConverter();
            _testQuery = new QRestQueryProvider(null, null).CreateQuery<TestClass>();
        }
        private class TestClass
        {
            public string Data { get; set; }
        }

        [Fact]
        public void Can_Convert_Simple_Constant()
        {
            var term = _converter.Convert<string>(Expression.Constant("ololo"));

            Assert.Equal("'ololo'", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Simple_Where_Query()
        {
            var exp = _testQuery.Where(t => t.Data == "ololo").Expression;
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-where(:#$$.Data-eq('ololo'))", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Simple_WhereBoolean_Query()
        {
            var exp = _testQuery.Where(t => t.Data == "ololo" || t.Data != "123").Expression;
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-where(:-oneof(#$$.Data-eq('ololo'),#$$.Data-ne('123')))", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Simple_Select_Query()
        {
            var exp = _testQuery.Select(t => t.Data).Expression;
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-map(:#$$.Data)", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Simple_Skip_Query()
        {
            var exp = _testQuery.Skip(10).Expression;
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-skip('10')", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Simple_Take_Query()
        {
            var exp = _testQuery.Take(10).Expression;
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-take('10')", term.ViewDebug);
        }        
    }
}
