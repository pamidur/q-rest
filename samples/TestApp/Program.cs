using Newtonsoft.Json;
using QRest.Compiller.Standard;
using QRest.Core;
using QRest.Core.Contracts;
using QRest.Core.Operations;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations.Query;
using QRest.Core.Terms;
using QRest.Semantics.MethodChain;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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

    class FakeRequest : IRequestModel
    {
        public string ModelName { get; set; }
        public string Query { get; set; }

        public Stream GetBody()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyMemory<string> GetNamedQueryPart(string name)
        {
            return new[] { Query }.AsMemory();
        }
    }

    //sort=-text@trim

    public class Program
    {
        public static void Main(string[] args)
        {
            var data = new List<Entity>
            {
                new Entity { Number = 1, Text = "CCC", Sub = new SubEntity { Text = "SubText" } },
                new Entity { Number = 2, Text = "AAA", Sub = new SubEntity { Text = "SubText2" } },
            }.AsQueryable();

            var parser = new MethodChainSemantics();
            var tree = parser.Parse(new FakeRequest { Query = ":where(-every(Sub.Text-eq(`SubText`),Text-ne(-it.Sub.Text), Status-eq({true}))):select(Number@num,Sub.Text):where(Number-eq(1)):select(Text)" });
                     
            //:where()-select(:count,:select)
            //:where()-select(:count,:order(-it.f1-asc):top(1):skip(2):select(-it.f1,it.f2))    
            //-select(:count,:order(-it.f1-asc):top(1):skip(2):select(-it.f1,it.f2))
            //:where(Sub.Text-eq(`SubText`))
            //:where(`SubText`-eq(-it.Sub.Text)



            //tree = new LambdaTerm
            //{
            //    Operation = new WhereOperation(),
            //    Arguments = new List<ITerm> {
            //        new MethodTerm{
            //            Operation = new EveryOperation(),
            //            Arguments = new List<ITerm>{
            //                new PropertyTerm{
            //                    PropertyName = "Text",
            //                    Next = new MethodTerm{
            //                        Operation = new EqualOperation(),
            //                        Arguments = new List<ITerm>{
            //                            new ConstantTerm {
            //                                Value = "dadsa"
            //                            }
            //                        }
            //                    }
            //                },
            //                 new PropertyTerm{
            //                    PropertyName = "Number",
            //                    Next = new MethodTerm{
            //                        Operation = new NotEqualOperation(),
            //                        Arguments = new List<ITerm>{
            //                            new ConstantTerm {
            //                                Value = 2
            //                            }
            //                        }
            //                    }
            //                },

            //            }
            //        },


            //    }
            //};


            // :with(1@v):where(price-ne($v)):select(text,price@totalprice,$v)

            // -with([1,2,3,4,5]@range):where-every(price-in($range),cost-in($range))

            // :where(-every(price-gt(1),price-lg(5)))

            // :where(price-gt(1)-and(price-lg(5),price-ne(3)))            

            // :where(price-every(-gt(1),-lg(5))):sort-desc(price)

            // :where(-gt(price,1))     


            var e = new StandardCompiler().Compile<IQueryable<Entity>>(tree);


            var r = e(data);            
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