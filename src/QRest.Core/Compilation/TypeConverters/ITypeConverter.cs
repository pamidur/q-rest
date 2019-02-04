using System;
using System.Linq.Expressions;

namespace QRest.Core.Compilation.TypeConverters
{
    public interface ITypeConverter
    {
        bool TryConvert(Expression expression, Type target, out Expression result);
    }
}
