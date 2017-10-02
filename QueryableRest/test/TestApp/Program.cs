using QueryableRest.Semantics.Arguments;
using QueryableRest.Semantics.Methods;
using QueryableRest.Semantics.Operations;
using QueryableRest.Semantics.SortOperations;
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
            var prop = new PropertyTerm { PropertyName = "Text" };

            var conts = new ConstantTerm { Value = "AAA" };

            var call = new CallTerm { Method = "eq" };
            call.Arguments.Add(conts);


            var data = Expression.Constant(new Entity { Number = 1, Text = "CCC" });

            call.CreateExpression(data, null);
            

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