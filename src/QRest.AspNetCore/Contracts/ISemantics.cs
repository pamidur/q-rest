using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRest.Core.Contracts;
using System;
using System.Collections.Generic;

namespace QRest.AspNetCore.Contracts
{
    public interface ISemantics
    {
        ActionResult WriteQueryResponse(IQueryStructure query, IReadOnlyDictionary<ITerm, object> results, Type source);
        IQueryStructure ReadQueryStructure(IReadOnlyList<string> values, HttpRequest request);
    }
}
