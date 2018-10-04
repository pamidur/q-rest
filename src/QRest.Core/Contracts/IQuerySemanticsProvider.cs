namespace QRest.Core.Contracts
{
    public interface IQuerySemanticsProvider
    {
        ITerm Parse(IRequestModel model);
    }
}
