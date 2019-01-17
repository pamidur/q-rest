using System.Collections.Generic;
using QRest.AspNetCore.Contracts;
using QRest.Core.Terms;

namespace QRest.AspNetCore.OData
{
    public class ODataQueryStructure : IQueryStructure
    {
        public ODataQueryStructure(string host)
        {
            Host = host;
        }

        public string Host { get; set; }

        public RootTerm Data { get; set; }
        public RootTerm Count { get; set; }

        public IReadOnlyList<RootTerm> GetAll() => new[] { Data, Count };
    }
}
