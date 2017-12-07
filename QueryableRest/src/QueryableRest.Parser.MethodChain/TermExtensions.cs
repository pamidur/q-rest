using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QRest.Semantics.MethodChain
{
    internal static class TermExtensions
    {
        public static ITerm GetLatestCall(this ITerm root)
        {
            var res = root;
            while (res.Next != null) res = res.Next;

            return res;
        }
    }
}
