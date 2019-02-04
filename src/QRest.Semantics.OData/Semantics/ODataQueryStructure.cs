using System.Collections.Generic;
using QRest.AspNetCore.Contracts;
using QRest.Core.Terms;

namespace QRest.Semantics.OData.Semantics
{
    public class ODataQueryStructure : IQueryStructure
    {
        public ITerm Data { get; set; }
        public ITerm Count { get; set; }

        public IReadOnlyList<ITerm> GetAll() => new[] { Data, Count };
    }
}
