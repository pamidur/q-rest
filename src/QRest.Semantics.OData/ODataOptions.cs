using Microsoft.AspNetCore.Http;

namespace QRest.Semantics.OData
{
    public class ODataOptions
    {
        public PathString ServiceRoot { get; set; } = "/";
        public string Namespace { get; set; } = "OData";
    }
}
