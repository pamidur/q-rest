namespace QRest.Core.Terms
{
    public interface ITerm
    {
        ITerm Clone();
        string ViewDebug { get; }
        string ViewQuery { get; }
        string ViewKey { get; }
    }
}