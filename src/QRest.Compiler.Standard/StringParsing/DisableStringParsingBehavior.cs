using System;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.StringParsing
{
    public class DisableStringParsingBehavior : IStringParsingBehavior
    {
        public Expression Parse(Expression expression, Type target)
        {
            return null;
        }
    }
}
