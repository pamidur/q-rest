using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ODataSamples.Contexts;
using QRest.AspNetCore;
using SQLitePCL;
using System;
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
        public DateTime DatetimeUnspec { get; set; } = new DateTime(new Random().Next(1970, 2018), new Random().Next(1, 12), new Random().Next(1, 28));
        public DateTime DatetimeLocal { get; set; } = DateTime.Now;
        public DateTime DatetimeUtc { get; set; } = DateTime.UtcNow;
        public DateTimeOffset Datetimeoffset { get; set; } = DateTimeOffset.Now;

        public SubEntity Sub { get; set; }

        public string[] Emails { get; set; } = new string[] { };

        public IEnumerable<SubEntity> Contacts { get; set; } = new List<SubEntity> { };


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
        private readonly DataContext _context;
        private static List<Entity> _data = new List<Entity>
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
                new Entity {  Number = 133, Text = "EEE", Emails = new[]{"test1@gmail.com", "test2@gmail.com" } },
                new Entity {  Number = 134, Text = "EEE", Contacts= new List<SubEntity>{ new SubEntity { Text = "qwerty" } , new SubEntity { Text = "qwerty22" }} },

            };



        public ValuesController(DataContext ctx)
        {
            var client = new MongoClient("mongodb://test:test@server:27017/test");
            collection = client.GetDatabase("test").GetCollection<Entity>("entities");
            //collection.InsertMany(_data);
            _source = collection.AsQueryable();
            _context = ctx;
            _context.Database.EnsureCreated();
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

        class Test1
        {
            public Test1 Test { get; set; }
            public int[] id { get; set; }

        }

        [HttpGet("fromef/{query?}")]
        public ActionResult<IQueryable<Entity>> FromEf(Query query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var temp = _context.Customers.Select(c => new { c.Id });

            var aresult = query.ToActionResult(_context.Customers.Include(o => o.Orders));
            return aresult;
        }

        [HttpGet("testdate/{dateTime}")]
        public IActionResult TestDate(DateTime? dateTime)
        {
            return Ok(dateTime?.Kind.ToString());
        }
    }
}
