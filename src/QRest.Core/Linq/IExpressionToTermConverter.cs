using QRest.Core.Terms;
using System.Linq.Expressions;

namespace QRest.Core.Linq
{
    public interface IExpressionToTermConverter
    {
        ITerm Convert<TResult>(Expression expression);
    }
}
