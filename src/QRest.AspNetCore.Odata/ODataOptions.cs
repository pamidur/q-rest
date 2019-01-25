using Microsoft.AspNetCore.Http;

namespace QRest.AspNetCore.OData
{
    public class ODataOptions
    {
        public PathString ServiceRoot { get; set; } = "/";
        public string Namespace { get; set; } = "OData";

        //public IReadOnlyList<PathString> ApiPaths { get; set; }
    }
}
