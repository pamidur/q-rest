using QRest.Core.Contracts;
using QRest.Core.Operations;
using QRest.Core.Operations.Query;
using QRest.Core.RootProviders;

namespace QRest.Core
{
    public static class BuiltIn
    {
        public static class Roots
        {
            public static readonly IRootProvider OriginalRoot = new SameRootProvider();
            public static readonly IRootProvider ContextElement = new ElementRootProvider(new SameContextProvider());
        }

        public static class Operations
        {
            public static readonly IOperation New = new NewOperation();
            public static readonly IOperation Select = new SelectOperation();
        }
    }
}
