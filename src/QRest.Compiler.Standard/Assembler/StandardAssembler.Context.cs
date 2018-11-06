using QRest.Core;
using QRest.Core.Contracts;
using QRest.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.Assembler
{
    public partial class StandardAssembler : IAssemblerContext
    {
        public IReadOnlyDictionary<string, Expression> GetInitializers(IReadOnlyList<Expression> arguments)
        {
            var fields = new Dictionary<string, Expression>();
            foreach (var arg in arguments)
            {
                var name = GetName(arg);
                if (string.IsNullOrEmpty(name) || fields.ContainsKey(name))
                    name = CreateUniquePropName(fields);
                fields.Add(name, arg);
            }

            return fields;
        }

        public Expression CreateContainer(IReadOnlyDictionary<string, Expression> properties)
        {
            return DynamicContainer.CreateContainer(properties);
        }

        public Expression CreateContainer(IReadOnlyList<Expression> arguments)
        {
            var initializers = GetInitializers(arguments);
            return CreateContainer(initializers);
        }

        public string GetName(Expression arg)
        {
            string name = null;

            if (arg.NodeType == NamedExpression.NamedExpressionType)
            {
                name = ((NamedExpression)arg).Name;
            }
            else if (arg.NodeType == ExpressionType.MemberAccess)
            {
                name = ((MemberExpression)arg).Member.Name;
            }
            else if (arg.NodeType == ExpressionType.Parameter)
            {
                name = ((ParameterExpression)arg).Name;
            }
            else if (arg.NodeType == ExpressionType.Index)
            {
                var index = (IndexExpression)arg;
                var indexArg = index.Arguments.FirstOrDefault();
                if (indexArg == null)
                    throw new ExpressionCreationException();

                if (indexArg.NodeType != ExpressionType.Constant)
                    throw new ExpressionCreationException();

                name = ((ConstantExpression)indexArg).Value.ToString();
            }
            else if (arg.NodeType == ExpressionType.Call)
            {
                var call = (MethodCallExpression)arg;
                name = call.Method.Name;
            }
            else if (arg.NodeType == ExpressionType.Dynamic)
            {
                var dyn = (DynamicExpression)arg;

                if (dyn.Binder is GetMemberBinder)
                    name = ((GetMemberBinder)dyn.Binder).Name;
                else
                    throw new ExpressionCreationException();
            }
            else if (DynamicContainer.IsContainerType(arg.Type))
            {
                throw new ExpressionCreationException();
            }

            return name;
        }

        private string CreateUniquePropName(Dictionary<string, Expression> initializers)
        {
            var namePrefix = "Data";
            var ind = 0;

            var name = "";

            do
            {
                name = $"{namePrefix}{ind++}";
            } while (initializers.ContainsKey(name));

            return name;
        }

        public virtual (Expression Left, Expression Right) Convert(Expression left, Expression right)
        {
            var leftType = /*left.NodeType == ExpressionType.Lambda ? ((LambdaExpression)left).ReturnType : */left.Type;
            var rightType = /*right.NodeType == ExpressionType.Lambda ? ((LambdaExpression)right).ReturnType :*/ right.Type;

            if (TryConvert(right, leftType, out var newright))
                return (left, newright);
            else if (TryConvert(left, rightType, out var newleft))
                return (newleft, right);
            else
                throw new ExpressionCreationException($"Cannot cast {leftType.Name} and {rightType.Name} either way.");
        }

        public virtual bool TryConvert(Expression expression, Type target, out Expression result)
        {
            if (expression.Type == target)
            {
                result = expression;
                return true;
            }
            else if (target.IsAssignableFrom(expression.Type))
            {
                result = Expression.Convert(expression, target);
                return true;
            }
            else if (expression.Type == typeof(string))
            {
                result = _options.StringParsing.Parse(expression, target);
                return result != null;
            }

            result = null;
            return false;
        }

        public Expression SetName(Expression expression, string name = null)
        {
            if(name == null)            
                name = typeof(IQueryable).IsAssignableFrom(expression.Type) ? "Entities" : "Entity";
            

            return NamedExpression.Create(expression, name);
        }
    }
}
