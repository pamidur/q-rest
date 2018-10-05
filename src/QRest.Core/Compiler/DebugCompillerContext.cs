using QRest.Core.Compiler.Debug;
using QRest.Core.Contracts;
using System.Collections.Generic;
using System.Linq.Expressions;

//namespace QRest.Core.Compiler
//{
//    internal class DebugCompillerContext : DefaultCompilerContext
//    {
//        private readonly List<DebugTermView> _views;

//        public DebugCompillerContext(ITerm root, bool finalize) : base(finalize)
//        {
//            _views = new List<DebugTermView>();
//            DebugResult = new DebugResult { DebugView = _views, Root = root };
//        }

//        public DebugResult DebugResult { get; internal set; }      

//        protected override Expression CompileTerm(ITerm term, Expression context, ParameterExpression root)
//        {
//            var exp = base.CompileTerm(term, context, root);
//            var view = new DebugTermView { Term = term };
//            _views.Add(view);

//            exp = Expression.Block(Expression.Call(Expression.Constant(view), nameof(DebugTermView.SetDebugResult), new[] { exp.Type }, exp), exp);
//            return exp;
//        }
//    }
//}
