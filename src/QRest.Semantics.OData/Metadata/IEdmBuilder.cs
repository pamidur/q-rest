using Microsoft.AspNetCore.Mvc.Abstractions;

namespace QRest.Semantics.OData.Metadata
{
    public interface IEdmBuilder
    {
        ODataModel Build();
        string GetEdmName(ActionDescriptor actionDescriptor);
    }
}
