using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleApp4
{
    //class Query
    //{
    //    public List<Call> CallChain { get; set; }
    //    public List<Variable> Variables { get; set; }
    //}


    //class Array : Argument
    //{

    //}   

    class Constant : ITerm
    {
        public object Value { get; set; }

        public ITerm Parent { get; set; }

        public Expression GetExpression()
        {
            return Expression.Constant(Value, Value.GetType());
        }
    }

    class Property : ITerm
    {
        public string PropertyName { get; set; }

        public ITerm Parent { get; set; }

        public Expression GetExpression()
        {
            return Expression.PropertyOrField(Parent.GetExpression(),PropertyName);
        }
    }

    class Context : ITerm
    {
        public ITerm Parent { get; set; }

        public Expression GetExpression()
        {
            throw new NotImplementedException();
        }
    }

    //class Variable : Argument
    //{

    //}

    class Call : ITerm
    {
        public string Method { get; set; }
        public List<ITerm> Arguments { get; set; }

        public ITerm Parent { get; set; }

        public Expression GetExpression()
        {
            throw new NotImplementedException();
        }
    }

    interface ITerm
    {
        ITerm Parent { get; set; }
        Expression GetExpression();
    }


    class Program
    {

        public static IEnumerable<Parser<Constant>> GetConstantParsers()
        {
            return new List<Parser<Constant>> {

                from str in Parse.Contained(Parse.LetterOrDigit.Many().Text(), Parse.Char('\''), Parse.Char('\''))
                select new Constant { Value = str },

                from str in Parse.Digit.AtLeastOnce().Text()
                select new Constant { Value = Int32.Parse(str) },
            };
        }


        static void Main(string[] args)
        {
            var Root =
                from root in Parse.Char('$')
                select new Context();

            var Property =
             from str1 in Parse.Letter.Once().Text()
             from str2 in Parse.LetterOrDigit.Many().Text()
             select new Property { Parent = new Context(), PropertyName = str1 + str2 };

            var NestedProperty =
                from nav in Parse.Char('.')
                from prop in Property
                select prop;

            Parser<Call> Call = null;

            var Argument =
             from arg in GetConstantParsers().Aggregate((p1,p2)=>p1.XOr(p2)).XOr<ITerm>(Property).XOr(Root)
             from calls in Parse.Ref(() => Call).XOr<ITerm>(NestedProperty).Many()
             select new ITerm[] { arg }.Concat(calls).Aggregate((c1, c2) => { c2.Parent = c1; return c2; });

            Call =
             from semic in Parse.Char(':')
             from method in Parse.LetterOrDigit.Many().Text()
             from parameters in Parse.Optional(Parse.Contained(Parse.XDelimitedBy(Parse.Optional(Argument), Parse.Char(',')), Parse.Char('('), Parse.Char(')')))
             select new Call { Method = method, Arguments = parameters.IsEmpty ? new List<ITerm>() : parameters.GetOrDefault().Select(p => p.GetOrDefault()).Where(r => r != null).ToList<ITerm>() };


            var a = Argument.Parse("$:filter(len:eq(12),text:contains('qwerty')):select(id,text,data:select(id,value),blocks:filter(id:eq(1)):select(internal:select)):sort(blocks:max(id)):skip(3):take(10)");

            Console.WriteLine("Hello World!");
        }
    }
}
