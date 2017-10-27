using QueryableRest.Semantics.Operations;
using QueryableRest.Semantics.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

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

            // sub.text:eq('SubText'):not



            var tree = new CallTerm
            {
                Method = FilterOperation.DefaultMoniker,
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
                                },

                                Next = new CallTerm
                                {
                                    Method = NotOperation.DefaultMoniker
                                }
                            }
                        }
                    }                                                                                           
                }
            };

            var dataParam = Expression.Parameter(data.Type);

            var e = tree.CreateExpression(dataParam, new QueryableRest.Semantics.Registry());

            var l = Expression.Lambda(e, dataParam);
            
            var r = l.Compile().DynamicInvoke(data.Value);

            var aaaa = 0;

            //var data = new List<Entity>
            //{
            //    new Entity { Number = 1, Text = "CCC"},
            //    new Entity { Number = 2, Text = "AAA"},
            //    new Entity { Number = 3, Text = "BBB"},
            //}.AsQueryable();

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
}