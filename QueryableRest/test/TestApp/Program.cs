using Newtonsoft.Json;
using QRest.Core;
using QRest.Core.Operations;
using QRest.Core.Terms;
using QRest.Semantics.MethodChain;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TestApp
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

    //sort=-text@trim

    public class Program
    {
        public static void Main(string[] args)
        {            
            var data = Expression.Constant(new List<Entity>
            {
                new Entity { Number = 1, Text = "CCC", Sub = new SubEntity { Text = "SubText" } },
                new Entity { Number = 2, Text = "AAA", Sub = new SubEntity { Text = "SubText2" } },
            }.AsQueryable());

            var parser = new MethodChainParser();
            var tree = parser.Parse(new Dictionary<string, string[]> { { "", new[] 

            { ":where(-every(Sub.Text-eq('SubText'),Text-ne(Sub.Text))):select(Number,Sub.Text):where(Number-eq(1)):select(Text)" }

                } });


            // :with(1@v):where(price-ne($v)):select(text,price@totalprice,$v)

            // -with([1,2,3,4,5]@range):where-every(price-in($range),cost-in($range))

            // :where(-every(price-gt(1),price-lg(5)))

            // :where(price-gt(1)-and(price-lg(5),price-ne(3)))            

            // :where(price-every(-gt(1),-lg(5))):sort-desc(price)

            // :where(-gt(price,1))     

            var dataParam = Expression.Parameter(data.Type);

            var registry = new Registry();
            Registry.RegisterDefaultOperations(registry);

            var e = tree.CreateExpression(dataParam, dataParam, registry);

            var l = Expression.Lambda(e, dataParam);

            var r = l.Compile().DynamicInvoke(data.Value);

            var brakepoint = 0;   
        }
    }

    class SelectOperationContainer : Dictionary<string, object>
    {
        public SelectOperationContainer() { }

        public SelectOperationContainer(IDictionary<string, object> initial) : base(initial)
        {

        }
    }
}