using Microsoft.AspNetCore.Mvc.Abstractions;

namespace QRest.AspNetCore.OData.Metadata
{
    public interface IModelBuilder
    {
        ODataModel Build();
        string GetEdmName(ActionDescriptor actionDescriptor);
    }
}
