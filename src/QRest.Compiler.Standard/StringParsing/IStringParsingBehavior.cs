using System;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.StringParsing
{
    public interface IStringParsingBehavior
    {
        Expression Parse(Expression expression, Type target);
    }
}
