﻿using QRest.Core.Extensions;
using QRest.Core.Operations;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.Expressions
{
    public class TerminationExpression : ProxyExpression
    {
        public static readonly ExpressionType ExpressionNodeType = (ExpressionType)2100;
        private readonly Type _element;
        private readonly Type _type;

        private TerminationExpression(Expression expression, Type element) : base(expression, ExpressionNodeType)
        {
            _element = element;
            _type = typeof(IQueryable<>).MakeGenericType(_element);
        }

        public override Type Type => _type;
        public override Expression Reduce()
        {
            return
                Call(typeof(Queryable), nameof(Queryable.AsQueryable), new[] { _element }
                , Call(typeof(Enumerable), nameof(Enumerable.ToArray), new[] { _element }, OriginalExpression));
        }

        public override string ToString() => $"{base.ToString()}.Terminate()";

        public static Expression Create(Expression expression)
        {
            if (expression.NodeType == ExpressionNodeType)
                return expression;

            if (expression.Type.TryGetCollectionElement(out var element) && element.queryable)
                return new TerminationExpression(expression, element.type);

            return expression;
        }
    }
}
