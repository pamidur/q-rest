﻿using QRest.Core.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public sealed class SkipOperation : QueryOperationBase
    {
        internal SkipOperation() { }

        public override string Key { get; } = "skip";        

        protected override Expression CreateExpression(Expression context, Type element, IReadOnlyList<Expression> arguments, IAssembler assembler)
        {
            if (arguments.Count != 1)
                throw new CompilationException("Expected 1 parameter");

            if (!assembler.TypeConverter.TryConvert(arguments[0], typeof(int), out var argument))
                throw new CompilationException($"Cannot cast {arguments[0].Type} to Int32");

            var exp = Expression.Call(QueryableType, nameof(Queryable.Skip), new Type[] { element }, context, argument);

            return assembler.SetName(exp, "data");
        }
    }
}
