using System.Collections.Generic;
using QRest.AspNetCore.Contracts;
using QRest.Core.Terms;

namespace QRest.AspNetCore.OData
{
    public class ODataQueryStructure : IQueryStructure
    {
        public RootTerm Data { get; set; }
        public RootTerm Count { get; set; }

        public IReadOnlyList<RootTerm> GetAll() => new[] { Data, Count };
    }
}
