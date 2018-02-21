using QRest.Core.Terms;

namespace QRest.Core
{

    public static class TermExtensions
    {
        public static ITerm GetLatestCall(this ITerm root)
        {
            var res = root;
            while (res.Next != null) res = res.Next;

            return res;
        }
    }
}
