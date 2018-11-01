using QRest.Core.Terms;

namespace QRest.Core.Contracts
{
    public interface IQuerySemanticsProvider
    {
        LambdaTerm Parse(IRequestModel model);
    }
}
