using Newtonsoft.Json;
using QueryableRest.Semantics.Operations;
using QueryableRest.Semantics.Terms;
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
            var o = new ExpandoObject();
            o.TryAdd("Test", 1);

            //var site = CallSite<Func<CallSite, object, object>>.Create();


            var ed = Expression.Dynamic(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, "Test", o.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }),
                typeof(object), Expression.Constant(o));

            var ep = Expression.Property(Expression.Convert(Expression.Constant(o), typeof(IDictionary<string, object>)), "Item", Expression.Constant("Test"));

            var exp = Expression.Lambda<Func<object>>(ed);

            var ao = exp.Compile()();

            var os = JsonConvert.SerializeObject((IDictionary<string, object>)o);

            var txt = new StringBuilder("asdas").ToString();

            Expression<Func<SelectOperationContainer>> a = () => new SelectOperationContainer { { txt, new object() } };


            var data = Expression.Constant(new List<Entity>
            {
                new Entity { Number = 1, Text = "CCC", Sub = new SubEntity { Text = "SubText" } },
                new Entity { Number = 2, Text = "AAA", Sub = new SubEntity { Text = "SubText2" } },
            }.AsQueryable());

            // filter:(sub.text:eq('SubText'):not)



            var tree = new CallTerm
            {
                Method = WhereOperation.DefaultMoniker,
                IsPerEachCall = true,
                Arguments = new List<ITerm>
                {
                    new CallTerm
                    {
                        Method = EveryOperation.DefaultMoniker,
                        Arguments = new List<ITerm>
                        {
                            new PropertyTerm
                    {
                        PropertyName = "Sub",

                        Next = new PropertyTerm
                        {
                            PropertyName = "Text",

                            Next = new CallTerm
                            {
                                Method = EqualOperation.DefaultMoniker,
                                Arguments = new List<ITerm>
                                {
                                    new ConstantTerm
                                    {
                                        Value = "SubText"
                                    }
                                }
                            }
                        }
                    },


                        new PropertyTerm
                        {
                            PropertyName = "Text",

                            Next = new CallTerm
                            {
                                Method = NotEqualOperation.DefaultMoniker,
                                Arguments = new List<ITerm>
                                {
                                    new PropertyTerm
                                    {
                                        PropertyName = "Sub",
                                        Next = new PropertyTerm
                                        {
                                            PropertyName = "Text",
                                        }
                                    }

                                }
                            }

                    }
                        }
                    }
                },
                Next = new CallTerm
                {
                    Method = SelectOperation.DefaultMoniker,
                    IsPerEachCall = true,
                    Arguments = new List<ITerm> {
                        new PropertyTerm
                        {
                            PropertyName = "Number",
                        },
                        new PropertyTerm
                                    {
                                        PropertyName = "Sub",
                                        Next = new PropertyTerm
                                        {
                                            PropertyName = "Text",
                                        }
                                    }
                    }
                    ,
                    Next = new CallTerm
                    {
                        Method = WhereOperation.DefaultMoniker,
                        IsPerEachCall = true,
                        Arguments = new List<ITerm> {
                            new PropertyTerm
                            {
                                PropertyName = "Number",
                                Next = new CallTerm
                                {
                                    Method = EqualOperation.DefaultMoniker,
                                    Arguments = new List<ITerm>{
                                        new ConstantTerm{ Value = 1}
                                    }
                                }
                            },
                        },

                        Next = new CallTerm
                        {
                            Method = SelectOperation.DefaultMoniker,
                            IsPerEachCall = true,
                            Arguments = new List<ITerm> {
                            new PropertyTerm
                            {
                                PropertyName = "Text",
                            }
                        }
                        }
                    }
                }
            };


            // :with(1@v):where(price-ne($v)):select(text,price@totalprice,$v)

            // -with([1,2,3,4,5]@range):where-every(price-in($range),cost-in($range))

            // :where(-every(price-eq(cost))):select(id,text)
             
                      
            var data2 = new List<Entity>
            {
                new Entity { Number = 1, Text = "CCC"},
                new Entity { Number = 2, Text = "AAA"},
                new Entity { Number = 3, Text = "BBB"},
            }.AsQueryable();


            var results = from store in data2
                          let AveragePrice = store.Number * 2
                          where AveragePrice < 500 && AveragePrice > 250
                          select store;



            var dataParam = Expression.Parameter(data.Type);

            var e = tree.CreateExpression(dataParam, dataParam, new QueryableRest.Semantics.Registry());

            var l = Expression.Lambda(e, dataParam);

            var r = l.Compile().DynamicInvoke(data.Value);

            var aaaa = 0;



            ////var sortQuery = new Sort<Entity>();
            ////sortQuery.Operations.Add(SortOperation<Entity>.ByAscending(e => e.Text));
            ////sortQuery.Operations.Add(SortOperation<Entity>.ByDescending(e => e.Number));

            //var sortQuery = Sort<Entity>.From("Text;-Number");
            ////sortQuery.Fields.Add(new Tuple<Argument, Operation>(new FieldValueArgument { FieldName = "Number" }, new OrderByAscendingOperation()));

            ////var result = sortQuery.ApplyTo(data).Cast<object>().ToList();

            ////var b = a.GetTypeInfo().;

            ////var query = new SortQuery<Entity>();
            ////query.Fields.Add(new KeyValuePair<Field<Entity>, Operation<Entity>>(new Field<Entity>() { Name = "Number" }, new OrderByAscendingOperation<Entity>()));

            //var result = sortQuery.ApplyTo(data.AsQueryable()).Cast<object>().ToList();

            //var query_test = data.AsQueryable().OrderBy(e => e.Text).OrderByDescending(e => e.Number);
            //var result_test = query_test.ToList();

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