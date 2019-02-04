namespace QRest.Core.Terms
{
    public interface ITerm
    {
        ITerm Clone();
        string DebugView { get; }
        string SharedView { get; }
        string KeyView { get; }
    }
}