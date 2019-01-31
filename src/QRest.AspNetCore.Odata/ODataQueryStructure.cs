using System.Collections.Generic;
using QRest.AspNetCore.Contracts;
using QRest.Core.Contracts;

namespace QRest.AspNetCore.OData
{
    public class ODataQueryStructure : IQueryStructure
    {
        public ITerm Data { get; set; }
        public ITerm Count { get; set; }

        public IReadOnlyList<ITerm> GetAll() => new[] { Data, Count };
    }
}
