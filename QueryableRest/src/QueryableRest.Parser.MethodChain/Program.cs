//using QRest.Core.Terms;
//using Sprache;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;

//namespace ConsoleApp4
//{
//    class Program
//    {

//        public static IEnumerable<Parser<ConstantTerm>> GetConstantParsers()
//        {
//            return new List<Parser<ConstantTerm>> {

//                from str in Parse.Contained(Parse.LetterOrDigit.Many().Text(), Parse.Char('\''), Parse.Char('\''))
//                select new ConstantTerm { Value = str },

//                from str in Parse.Digit.AtLeastOnce().Text()
//                select new ConstantTerm { Value = Int32.Parse(str) },

//                from str in Parse.Contained(Parse.LetterOrDigit.Many().Text(), Parse.Char('{'), Parse.Char('}'))
//                select new ConstantTerm { Value = Guid.Parse(str) }
//            };
//        }


//        static void Main(string[] args)
//        {
  

//            Parser<Call> Call = null;

//            var Argument =
//             from arg in GetConstantParsers().Aggregate((p1,p2)=>p1.XOr(p2)).XOr<ITerm>(Property).XOr(Root)
//             from calls in Parse.Ref(() => Call).XOr<ITerm>(NestedProperty).Many()
//             select new ITerm[] { arg }.Concat(calls).Aggregate((c1, c2) => { c2.Parent = c1; return c2; });

//            Call =
//             from semic in Parse.Char(':')
//             from method in Parse.LetterOrDigit.Many().Text()
//             from parameters in Parse.Optional(Parse.Contained(Parse.XDelimitedBy(Parse.Optional(Argument), Parse.Char(',')), Parse.Char('('), Parse.Char(')')))
//             select new Call { Method = method, Arguments = parameters.IsEmpty ? new List<ITerm>() : parameters.GetOrDefault().Select(p => p.GetOrDefault()).Where(r => r != null).ToList<ITerm>() };


//            var a = Argument.Parse("$:filter(len:eq(12),text:contains('qwerty')):select(id,text,data:select(id,value),blocks:filter(id:eq(1)):select(internal:select)):sort(blocks:max(id)):skip(3):take(10)");

//            Console.WriteLine("Hello World!");
//        }
//    }
//}
