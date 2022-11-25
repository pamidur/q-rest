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
            public NestedTestClass Nested { get; set; }
        }

        public class NestedTestClass
        {
            public int Number { get; set; }
        }

        [Fact]
        public void Can_Convert_Simple_Constant()
        {
            var term = _converter.Convert<string>(Expression.Constant("ololo"));

            Assert.Equal("'ololo'", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Complex_Count_Query()
        {
            var exp = Express(q => q.Count(t => t.Data == "ololo"));
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-where(:#$$.Data-eq('ololo'))-count", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Simple_Count_Query()
        {
            var exp = Express(q => q.Count());
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-count", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Simple_Where_Query()
        {
            var exp = Express(q => q.Where(t => t.Data == "ololo"));
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-where(:#$$.Data-eq('ololo'))", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Complex_Where_Query()
        {
            var exp = Express(q => q.Where(t => t.Data == "ololo" || (t.Data != "123" && t.Nested.Number > 3)));
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-where(:-any(#$$.Data-eq('ololo'),-all(#$$.Data-ne('123'),#$$.Nested.Number-gt('3'))))", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Complex_Select_Query()
        {
            var exp = Express(q => q.Select(t => new { Text = t.Data, Value = t.Nested.Number }));
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-map(:-new(#$$.Data@Text,#$$.Nested.Number@Value))", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Simple_Select_Query()
        {
            var exp = Express(q => q.Select(t => t.Data));
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-map(:#$$.Data)", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Simple_Skip_Query()
        {
            var exp = Express(q => q.Skip(10));
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-skip('10')", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Simple_Take_Query()
        {
            var exp = Express(q => q.Take(10));
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-take('10')", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Simple_Order_Query()
        {
            var exp = Express(q => q.OrderBy(t => t.Data));
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-order(:#$$.Data)", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Simple_OrderDesc_Query()
        {
            var exp = Express(q => q.OrderByDescending(t => t.Data));
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-order(:#$$.Data-desc)", term.ViewDebug);
        }

        [Fact]
        public void Can_Convert_Complex_Order_Query()
        {
            var exp = Express(q => q.OrderBy(t => t.Data).ThenByDescending(t => t.Nested.Number).ThenBy(t => t.Nested));
            var term = _converter.Convert<string>(exp);

            Assert.Equal("#$$-order(:#$$.Data,:#$$.Nested.Number-desc,:#$$.Nested)", term.ViewDebug);
        }

        private static Expression Express(Expression<Action<IQueryable<TestClass>>> action)
        {
            return action.Body;
        }
    }
}
