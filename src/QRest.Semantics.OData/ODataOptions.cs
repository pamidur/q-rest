using Microsoft.AspNetCore.Http;
using QRest.Semantics.OData.Parsing;

namespace QRest.Semantics.OData
{
    public class ODataOptions
    {
        public static ODataOptions Default { get; } = new ODataOptions();

        public PathString ServiceRoot { get; set; } = "/";
        public string Namespace { get; set; } = "OData";
        public ODataOperationMap Operations { get; set; } = new ODataOperationMap();
    }
}
