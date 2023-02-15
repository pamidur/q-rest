using QRest.Core.Terms;
using System.Threading.Tasks;

namespace QRest.Core.Linq
{
    public interface ITermExecutor
    {
        Task<TResult> Execute<TResult>(ITerm term);
    }
}
