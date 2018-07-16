using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using QRest.AspNetCore;

namespace TestWebApp.Controllers
{
    [BsonIgnoreExtraElements]
    public class Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId Id { get; set; }
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
        private readonly IMongoCollection<Entity> collection;
        IQueryable<Entity> _data = new List<Entity>
            {
                new Entity { Number = 1, Text = "CCC", Sub = new SubEntity { Text = "SubText" } },
                new Entity { Number = 2, Text = "AAA", Sub = new SubEntity { Text = "SubText2" } },
            }.AsQueryable();

        public ValuesController()
        {
            var client = new MongoClient(new MongoClientSettings() { Server = new MongoServerAddress("localhost", 27017) });
            collection = client.GetDatabase("test").GetCollection<Entity>("entities");
            //collection.InsertOne(new Entity { Number = 3, Text = "Olo" });
        }

        // GET api/values
        [HttpGet("{query?}")]
        public ActionResult Get(Query query)
        {
            var cr = Expression.Call(typeof(Int32), "Parse", new Type[] { }, Expression.Constant("1"));

            var data = collection.AsQueryable();

            var r = data.Where(e => e.Number == int.Parse("1"));
            
            var rr = r.ToArray();

            var result = query.Apply(data);          

            return Ok(result);
        }        
    }
}
