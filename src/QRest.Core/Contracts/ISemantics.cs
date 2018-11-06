using QRest.Core.Terms;

namespace QRest.Core.Contracts
{
    public interface ISemantics
    {
        LambdaTerm Parse(IRequestModel model);
    }
}
