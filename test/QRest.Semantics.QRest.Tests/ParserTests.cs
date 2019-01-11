using Moq;
using QRest.AspNetCore.Native;
using QRest.Core.Contracts;
using QRest.Core.Terms;
using Sprache;
using System;
using System.Collections.Generic;
using Xunit;

namespace QRest.Semantics.QRest.Tests
{
    public class ParserTests
    {
        private readonly Mock<IOperation> _opration;
        private readonly QRestParserBuilder _parser;

        private static readonly string _testUniversalOperationName = "test";

        public ParserTests()
        {
            _opration = new Mock<IOperation>();
            _opration.Setup(m => m.Key).Returns(_testUniversalOperationName);

            _parser = new QRestParserBuilder(DefferedConstantParsing.Disabled, new Dictionary<string, Func<SequenceTerm[], MethodTerm>> {
                { _testUniversalOperationName, s=>new MethodTerm(_opration.Object) }  
            });
            _parser.Build();
        }

        [Fact(DisplayName = "Correct String Constant Is Parsed")]
        public void ValidStringConstant()
        {
            var expected = "Test123-:=/\\+()!@#$%$String\r\n\t!";
            var actual = _parser.StringConstant.TryParse($"`{expected}`");

            Assert.True(actual.Remainder.AtEnd);
            Assert.Empty(actual.Remainder.Memos);

            Assert.True(actual.WasSuccessful);
            Assert.Empty(actual.Expectations);

            Assert.Equal(expected, actual.Value.Value);
        }

        [Fact(DisplayName = "Correct Empty String Constant Is Parsed")]
        public void ValidEmptyStringConstant()
        {
            var expected = "";
            var actual = _parser.StringConstant.TryParse($"`{expected}`");

            Assert.True(actual.WasSuccessful);
            Assert.Empty(actual.Expectations);

            Assert.Equal(expected, actual.Value.Value);
        }

        [Fact(DisplayName = "Incorrect String Constant Shows Error")]
        public void InValidStringConstant1()
        {
            var actual = _parser.StringConstant.TryParse($"`text");

            Assert.NotEmpty(actual.Expectations);
            Assert.False(actual.WasSuccessful);
        }

        [Fact(DisplayName = "Correct Float Constant Is Parsed")]
        public void FloatParseTest()
        {
            var actual = _parser.NumberConstant.TryParse($"1.12");

            Assert.Empty(actual.Expectations);
            Assert.True(actual.WasSuccessful);
            Assert.Equal(1.12f, (float)actual.Value.Value);
        }

        [Fact(DisplayName = "Correct Int Constant Is Parsed")]
        public void IntParseTest()
        {
            var actual = _parser.NumberConstant.TryParse($"567");

            Assert.Empty(actual.Expectations);
            Assert.True(actual.WasSuccessful);
            Assert.Equal(567, (int)actual.Value.Value);
        }

        [Fact(DisplayName = "Parsed Method Without Params and Brakets")]
        public void MethodWithoutParamsAndBrakets()
        {
            var actual = _parser.Call.TryParse($"-{_testUniversalOperationName}");

            Assert.Empty(actual.Expectations);
            Assert.True(actual.WasSuccessful);
            Assert.Equal(_testUniversalOperationName, actual.Value.Operation.Key);
            Assert.Empty(actual.Value.Arguments);
        }

        [Fact(DisplayName = "Parsed Method Without Params")]
        public void MethodWithoutParams()
        {
            var actual = _parser.Call.TryParse($"-{_testUniversalOperationName}()");

            Assert.Empty(actual.Expectations);
            Assert.True(actual.WasSuccessful);
            Assert.Equal(_testUniversalOperationName, actual.Value.Operation.Key);
            Assert.Empty(actual.Value.Arguments);
        }
    }
}
