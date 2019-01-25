using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using QRest.AspNetCore;
using System.Collections.Generic;
using System.Linq;

namespace TestWebApp.Controllers
{
    [BsonIgnoreExtraElements]
    public class Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        //[JsonConverter(typeof(ObjectIdConverter))]
        public string Id { get; set; }

        public string Text { get; set; }
        public int Number { get; set; }
        //public DateTime Datetime { get; set; }
        //public DateTimeOffset Datetimeoffset { get; set; }

        //public SubEntity Sub { get; set; }

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
        private readonly IMongoCollection<Entity> collection;
        private readonly IMongoQueryable<Entity> _source;
        private IQueryable<Entity> _data = new List<Entity>
            {
                new Entity { Number = 21, Text = "CCC", /*Sub = new SubEntity { Text = "SubText" } */},
                new Entity { Number = 32, Text = "AAA",/* Sub = new SubEntity { Text = "SubText2" }*/ },
                new Entity { Number = 43, Text = "AAA",/* Sub = new SubEntity { Text = "SubText2" }*/ },
                new Entity { Number = 54, Text = "AAA",/* Sub = new SubEntity { Text = "SubText2" }*/ },
                new Entity { Number = 65, Text = "AAA",/* Sub = new SubEntity { Text = "SubText2" }*/ },
                new Entity { Number = 76, Text = "AAA",/* Sub = new SubEntity { Text = "SubText2" }*/ },
                new Entity { Number = 87, Text = "AAA",/* Sub = new SubEntity { Text = "SubText2" }*/ },
                new Entity { Number = 98, Text = "AAA",/* Sub = new SubEntity { Text = "SubText2" }*/ },
                new Entity { Number = 109, Text = "AAA",/* Sub = new SubEntity { Text = "SubText2" }*/ },
                new Entity { Number = 110, Text = "AAA",/* Sub = new SubEntity { Text = "SubText2" }*/ },
                new Entity {  Number = 121, Text = "AAA",/* Sub = new SubEntity { Text = "SubText2" }*/ },
                new Entity {  Number = 132, Text = "AAA",/* Sub = new SubEntity { Text = "SubText2" }*/ },
            }.AsQueryable();



        public ValuesController()
        {
            var client = new MongoClient(new MongoClientSettings() { Server = new MongoServerAddress("localhost", 27017) });
            collection = client.GetDatabase("test").GetCollection<Entity>("entities");
            _source = collection.AsQueryable();
        }

        [HttpGet("{query?}")]
        public ActionResult<IQueryable<Entity>> FromMemory(Query query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var aresult = query.ToActionResult(_data);
            return aresult;
        }

        [HttpGet("fromdb/{query?}")]
        public ActionResult<IQueryable<Entity>> FromDb(Query query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var aresult = query.ToActionResult(_source);
            return aresult;
        }
    }
}
