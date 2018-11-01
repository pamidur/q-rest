using QRest.Core.Contracts;

namespace QRest.Core.RootProviders
{
    public static class BuiltInRootProviders
    {
        public static IRootProvider Root = new SameRootProvider();
        public static IRootProvider Context = new SameContextRootProvider();
        public static IRootProvider ContextElement = new ElementRootProvider(Context);
        public static IRootProvider RootElement = new ElementRootProvider(Root);
    }
}
