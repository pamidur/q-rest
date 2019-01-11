using QRest.Core;
using QRest.Core.Contracts;
using QRest.Core.Operations;
using QRest.Core.Operations.Aggregations;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations.Query;
using QRest.Core.Operations.Selectors;
using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QRest.AspNetCore.Native
{
    public partial class NativeSemantics
    {
        private readonly Dictionary<string, Func<SequenceTerm[], MethodTerm>> _callMap = new Dictionary<string, Func<SequenceTerm[], MethodTerm>>
        {
            {"ne",      s=> DefaultMethod( new NotEqualOperation(),s) },
            {"eq",      s=> DefaultMethod( new EqualOperation(),s)},
            {"gt",      s=> DefaultMethod( new GreaterThanOperation(),s)},
            {"gte",     s=> DefaultMethod( new GreaterThanOrEqualOperation(),s)},
            {"lt",      s=> DefaultMethod( new LessThanOperation(),s)},
            {"lte",     s=> DefaultMethod( new LessThanOrEqualOperation(),s)},
            {"not",     s=> DefaultMethod( new NotOperation(),s)},
            {"oneof",   s=> DefaultMethod( new OneOfOperation(),s) },
            {"every",   s=> DefaultMethod( new EveryOperation() ,s)},
            {"has",     s=> DefaultMethod( new ContainsOperation() ,s)},
            {"get",     s=> DefaultMethod( new NewOperation() ,s)},

            {"skip",    s=> DefaultMethod( new SkipOperation(),s)},
            {"take",    s=> DefaultMethod( new TakeOperation(),s)},

            {"asc",     s=> DefaultMethod( new ContextOperation(),s)},
            {"desc",    s=> DefaultMethod( new ReverseOrderOperation(),s)},

            {"$$",      s=> DefaultMethod( new ItOperation(),s)},
            {"$",     s=> DefaultMethod( new ContextOperation(),s)},

            {"first",   s=> DefaultMethod( new FirstOperation(),s)},
            {"count",   s=> DefaultMethod( new CountOperation(),s)},
            {"where",   s=> DefaultLambda( new WhereOperation(),s)},
            {"any",     s=> DefaultLambda( new AnyOperation(),s)},
            {"map",     s=> SelectLambda(s)},
            {"sum",     s=> DefaultLambda( new SumOperation(),s)},
            {"order",   s=> DefaultLambda( new OrderOperation(),s)},
        };

        private static MethodTerm DefaultLambda(IOperation operation, SequenceTerm[] args)
        {
            return new MethodTerm(operation, args.Select(s => new LambdaTerm(BuiltIn.Roots.ContextElement, s)).ToList());
        }

        private static MethodTerm SelectLambda(SequenceTerm[] args)
        {
            return new MethodTerm(BuiltIn.Operations.Select, new[] { new LambdaTerm(BuiltIn.Roots.ContextElement, new MethodTerm(BuiltIn.Operations.New, args).AsSequence()) });
        }

        private static MethodTerm DefaultMethod(IOperation operation, SequenceTerm[] args)
        {
            return new MethodTerm(operation, args);
        }
    }
}
