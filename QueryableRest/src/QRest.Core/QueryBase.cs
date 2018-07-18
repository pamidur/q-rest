using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using QRest.Core.Extensions;
using QRest.Core.Operations;
using QRest.Core.Contracts;
using QRest.Core.Compiler;

namespace QRest.Core
{
    public abstract class QueryBase
    {
        public ITerm RootTerm { get; set; } = new MethodTerm { Operation = new ItOperation() };

        private static readonly TermTreeCompiler _compiler = new TermTreeCompiler();

        public object Apply<T>(IQueryable<T> target, bool finalize = true)
        {
            var lambda = _compiler.Compile<IQueryable<T>>(RootTerm);

            var result = lambda.Compile()(target);

            //var debug = _compiler.CompileDebug<IQueryable<T>>(RootTerm);
            //debug.Func.Compile()(target);

            return result;
        }
    }
}
