using QRest.Core.Terms;

namespace QRest.Core.Contracts
{
    public interface IQuerySemanticsProvider
    {
        SequenceTerm Parse(IRequestModel model);
    }
}
