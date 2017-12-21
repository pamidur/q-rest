using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TestWebApp.Controllers
{
    public class Entity
    {
        public string Text { get; set; }
        public int Number { get; set; }

        public SubEntity Sub { get; set; }

        public override string ToString()
        {
            return $"{Text} {Number}";
        }
    }

    public class SubEntity
    {
        public string Text { get; set; }
        public string Text2 { get; set; }
        public int Number { get; set; }

        public override string ToString()
        {
            return $"{Text} {Number}";
        }
    }




    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        IQueryable<Entity> _data = new List<Entity>
            {
                new Entity { Number = 1, Text = "CCC", Sub = new SubEntity { Text = "SubText" } },
                new Entity { Number = 2, Text = "AAA", Sub = new SubEntity { Text = "SubText2" } },
            }.AsQueryable();



        // GET api/values
        [HttpGet("{query?}")]
        public ActionResult Get(Query query)
        {
            var result = query.Apply(_data);
            return Ok(result);
        }        
    }
}
