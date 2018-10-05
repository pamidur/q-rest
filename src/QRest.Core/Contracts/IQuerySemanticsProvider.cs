namespace QRest.Core.Contracts
{
    public interface IQuerySemanticsProvider
    {
        ITermSequence Parse(IRequestModel model);
    }
}
