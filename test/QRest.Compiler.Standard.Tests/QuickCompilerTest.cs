using QRest.Core.Compilation;
using QRest.Core.Parsing;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace QRest.Compiler.Standard.Tests
{
    public class Entity
    {
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
    public class QuickCompilerTest
    {
        private static IQueryable<Entity> _data = new List<Entity>
            {
                new Entity { Number = 21, Text = "CCC", Sub = new SubEntity { Text = "SubText" }  },
                new Entity { Number = 32, Text = "AAA",  Sub = new SubEntity { Text = "ret" } },
                new Entity { Number = 43, Text = "AAA",  Sub = new SubEntity { Text = "cc" } },
                new Entity { Number = 54, Text = "AAA",  Sub = new SubEntity { Text = "xx" } },
                new Entity { Number = 65, Text = "AAA",  Sub = new SubEntity { Text = "rte" } },
                new Entity { Number = 76, Text = "AAA",  Sub = new SubEntity { Text = "zx" } },
                new Entity { Number = 87, Text = "AAA",  Sub = new SubEntity { Text = "cc" } },
                new Entity { Number = 98, Text = "AAA",  Sub = new SubEntity { Text = "SubrrText2" } },
                new Entity { Number = 109, Text = "AAA",  Sub = new SubEntity { Text = "rr" } },
                new Entity { Number = 110, Text = "AAA",  Sub = new SubEntity { Text = "66" } },
                new Entity {  Number = 121, Text = "AAA", Sub = new SubEntity { Text = "cc" } },
                new Entity {  Number = 132, Text = "AAA", Sub = new SubEntity { Text = "66" } },
                new Entity {  Number = 133, Text = "EEE", Sub = new SubEntity { Text = "66" }, Emails = new[]{"test1@gmail.com", "test2@gmail.com" } },
                new Entity {  Number = 134, Text = "EEE", Sub = new SubEntity { Text = "66" }, Contacts= new List<SubEntity>{ new SubEntity { Text = "qwerty" } , new SubEntity { Text = "qwerty22" }} },

            }.AsQueryable();

        [Fact]
        public void TestProperty()
        {
            var term = TermParser.Default.Parse("-where(:Number-eq(121))");
            var func = TermCompiler.Default.Compile<IQueryable<Entity>>(term);

            var result = (func(_data) as IQueryable<Entity>).ToArray();

            Assert.Contains(result, e => e.Number == 121);
            Assert.Single(result);
        }

        [Fact]
        public void TestSubProperty()
        {
            var term = TermParser.Default.Parse("-where(:Sub.Text-eq('SubrrText2'))");
            var temp = TermCompiler.Default.Compile<IQueryable<Entity>>(term)(_data);

            var result = (temp as IEnumerable<Entity>).ToArray();

            Assert.Contains(result, e => e.Sub?.Text == "SubrrText2");
            Assert.Single(result);
        }

        [Fact]
        public void TestOwnMethod()
        {
            var term = TermParser.Default.Parse("-where(:Contacts-any(:Text-eq('qwerty')))");
            var temp = TermCompiler.Default.Compile<IQueryable<Entity>>(term)(_data);

            var result = (temp as IEnumerable<Entity>).ToArray();

            Assert.Contains(result, e => e.Contacts.Any(s => s.Text == "qwerty"));
            Assert.Single(result);
        }

        [Fact]
        public void TestTakeSkipMethod()
        {
            var term = TermParser.Default.Parse("-where(:Text-eq('AAA'))-skip(1)-take(2)");
            var temp = TermCompiler.Default.Compile<IQueryable<Entity>>(term)(_data);

            var result = (temp as IEnumerable<Entity>).ToArray();

            Assert.Contains(result, e => e.Number == 43);
            Assert.Contains(result, e => e.Number == 54);
            Assert.Equal(2, result.Length);
        }
    }
}
