using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace QRest.AspNetCore.OData
{
    public class ODataOptions
    {
        public PathString MetadataPath { get; set; }
        public string Namespace { get; set; }

        //public IReadOnlyList<PathString> ApiPaths { get; set; }
    }
}
