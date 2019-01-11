using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using QRest.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestWebApp.Controllers
{
    [BsonIgnoreExtraElements]
    public class Entity
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //[JsonConverter(typeof(ObjectIdConverter))]
        //public ObjectId Id { get; set; }
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
                new Entity { Number = 1, Text = "CCC", /*Sub = new SubEntity { Text = "SubText" } */},
                new Entity { Number = 2, Text = "AAA",/* Sub = new SubEntity { Text = "SubText2" }*/ },
            }.AsQueryable();



        public ValuesController()
        {
            var client = new MongoClient(new MongoClientSettings() { Server = new MongoServerAddress("localhost", 27017) });
            collection = client.GetDatabase("test").GetCollection<Entity>("entities");
            //collection.InsertOne(new Entity { Number = 1, Text = "dateTime tests", Datetime=DateTime.Now, Datetimeoffset = DateTime.Now });

            _source = collection.AsQueryable();
        }

        // GET api/values
        [HttpGet("{query?}")]
        public ActionResult Get(Query<IQueryable<Entity>> query)
        {
            //var result = query.Apply(_source);

            var aresult = query.ToActionResult(_source);
            return aresult;
        }
    }
}
