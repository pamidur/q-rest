using Microsoft.AspNetCore.Mvc;
using ODataSamples.Contexts;
using QRest.AspNetCore;
using System.Linq;

namespace ODataSamples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataBaseController : ControllerBase
    {
        private readonly DataContext _ctx;

        public DataBaseController(DataContext ctx)
        {
            _ctx = ctx;
        }

        //http://localhost:58337/api/database?$filter=Id%20le%204&$orderby=OrderDate%20desc&$select=CustomerFirstName,Title&$top=2&$skip=1
        //SELECT[c].[Id], [c.Customer].[FirstName] AS[CustomerFirstName], [c.Customer].[Discount], [c].[OrderDate], [c].[Title]
        //FROM[Orders] AS[c]
        //INNER JOIN[Customers] AS[c.Customer] ON[c].[CustomerId] = [c.Customer].[Id]
        //WHERE[c].[Id] <= @__Value_0
        //ORDER BY[c].[OrderDate]
        //DESC
        //OFFSET @__p_1 ROWS FETCH NEXT @__p_2 ROWS ONLY

        [HttpGet("{query?}")]
        public ActionResult<IQueryable<Domain.Order>> Get(Query query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var t1 = _ctx.Orders
            //    .Select(c => new Domain.Order
            //    {
            //        Id = c.Id,
            //        CustomerFirstName = c.Customer.FirstName,
            //        Discount = c.Customer.Discount,
            //        OrderDate = c.OrderDate,
            //        Title = c.Title
            //    })
            //    .Where(x => x.Id <= 4)
            //    .OrderByDescending(o => o.OrderDate)
            //    .Skip(1)
            //    .Take(2)
            //    .Select(s=> new { s.CustomerFirstName, s.Title})
            //    ;

            //var t2 = t1.ToArray();

            var data = _ctx.Orders.Select(c => new Domain.Order
            {
                Id = c.Id,
                CustomerFirstName = c.Customer.FirstName,
                Discount = c.Customer.Discount,
                OrderDate = c.OrderDate,
                Title = c.Title
            }).Where(z=> true);

            var result = query.ToActionResult(data);

            return result;
        }

    }
}
