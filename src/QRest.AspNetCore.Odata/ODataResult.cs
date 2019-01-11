using Microsoft.AspNetCore.Mvc;

namespace QRest.AspNetCore.OData
{
    public class ODataActionResult : OkObjectResult
    {
        public ODataActionResult(object value) : base(value)
        {
        }
    }
}
