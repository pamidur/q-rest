using QRest.Core.Terms;
using System.Collections.Generic;

namespace QRest.AspNetCore.Contracts
{
    public interface IQueryStructure
    {
        RootTerm Data { get; }
        IReadOnlyList<RootTerm> GetAll();
    }
}
