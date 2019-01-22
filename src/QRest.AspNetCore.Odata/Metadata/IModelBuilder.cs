using System;
using System.Collections.Generic;
using System.Text;

namespace QRest.AspNetCore.OData.Metadata
{
    public interface IModelBuilder
    {
        ODataModel Build();
    }
}
