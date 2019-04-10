using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ODataSamples.Contexts;
using QRest.AspNetCore;

namespace ODataSamples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemoryController : ControllerBase
    {
        [HttpGet("{query?}")]
        public ActionResult<IQueryable<Domain.Order>> Get(Query query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = InMemoryCtx._customers.Join(InMemoryCtx._orders, o => o.Id, i => i.CustomerId, (c, o) =>
                new Domain.Order
                {
                    Id = o.Id,
                    CustomerFirstName = c.FirstName,
                    Discount = c.Discount,
                    OrderDate = o.OrderDate,
                    Title = o.Title
                });

            var result = query.ToActionResult(data);

            return result;
        }

       
    }
}
