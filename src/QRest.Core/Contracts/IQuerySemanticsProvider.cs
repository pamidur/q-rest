using QRest.Core.Terms;

namespace QRest.Core.Contracts
{
    public interface IQuerySemanticsProvider
    {
        TermSequence Parse(IRequestModel model);
    }
}
