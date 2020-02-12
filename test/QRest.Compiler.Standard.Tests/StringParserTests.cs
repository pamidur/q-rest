using QRest.Core.Compilation.TypeConverters;
using System;
using System.Globalization;
using System.Linq.Expressions;
using Xunit;

namespace QRest.Compiler.Standard.Tests
{
    public enum SimpleEnum
    {
        First,
        Second
    }

    public enum IntEnum : int
    {
        First = 10,
        Second = 20
    }


    public class StringParserTests
    {
        private readonly DefaultTypeConverter _converter;

        public StringParserTests()
        {
            _converter = new DefaultTypeConverter(CultureInfo.InvariantCulture, true);
        }

        [Fact(DisplayName = "Can parse string enum")]
        public void Can_Parse_String_Enum()
        {
            var param = Expression.Parameter(typeof(string));
            Assert.True(_converter.TryConvert(param, typeof(SimpleEnum), out var parser));

            var lambda = Expression.Lambda<Func<string, SimpleEnum>>(parser, param);
            var result = lambda.Compile()(SimpleEnum.Second.ToString());

            Assert.Equal(SimpleEnum.Second, result);
        }

        [Fact(DisplayName = "Can parse int enum")]
        public void Can_Parse_Int_Enum()
        {
            var param = Expression.Parameter(typeof(int));
            Assert.True(_converter.TryConvert(param, typeof(IntEnum), out var parser));

            var lambda = Expression.Lambda<Func<int, IntEnum>>(parser, param);
            var result = lambda.Compile()((int)IntEnum.Second);

            Assert.Equal(IntEnum.Second, result);
        }

        [Fact(DisplayName = "Can parse stringly-int enum")]
        public void Can_Parse_Stringly_Int_Enum()
        {
            var param = Expression.Parameter(typeof(string));
            Assert.True(_converter.TryConvert(param, typeof(IntEnum), out var parser));

            var lambda = Expression.Lambda<Func<string, IntEnum>>(parser, param);
            var result = lambda.Compile()(((int)IntEnum.Second).ToString());

            Assert.Equal(IntEnum.Second, result);
        }
    }
}