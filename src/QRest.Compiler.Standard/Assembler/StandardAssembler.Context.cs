using QRest.Compiler.Standard.Expressions;
using QRest.Core;
using QRest.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.Assembler
{
    public partial class StandardAssembler : IAssemblerContext
    {
        public Expression CreateContainer(IReadOnlyDictionary<string, Expression> properties)
        {
            return DynamicContainer.CreateContainer(properties);
        }

        public Expression CreateContainer(IReadOnlyList<Expression> arguments)
        {
            var initializers = GetInitializers(arguments);
            return CreateContainer(initializers);
        }

        public Expression SetName(Expression expression, string name)
        {
            return NamedExpression.Create(expression, name);
        }

        public string GetName(Expression arg)
        {
            if (arg is NamedExpression named) return named.Name;
            if (arg is UnaryExpression unary) return GetName(unary.Operand);
            if (arg is MemberExpression member) return member.Member.Name;
            if (arg is ParameterExpression parameter) return parameter.Name;
            if (arg is IndexExpression index && index.Arguments.Count == 1 && index.Arguments[0] is ConstantExpression constant) return constant.Value.ToString();
            if (arg is DynamicExpression dynamic && dynamic.Binder is GetMemberBinder binder) return binder.Name;
            if (arg.CanReduce) return GetName(arg.Reduce());
            if (arg is MethodCallExpression call)
            {
                if (call.Object != null) return GetName(call.Object);
                else if (call.Arguments.Count != 0) return GetName(call.Arguments[0]);
            }
            return typeof(IQueryable).IsAssignableFrom(arg.Type) ? "Entities" : "Entity";
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
                result = _stringParsingBehavior.Parse(expression, target);
                return result != null;
            }

            result = null;
            return false;
        }

        protected IReadOnlyDictionary<string, Expression> GetInitializers(IReadOnlyList<Expression> arguments)
        {
            var fields = new Dictionary<string, Expression>();
            foreach (var arg in arguments)
            {
                var name = GetName(arg);
                if (fields.ContainsKey(name)) name = CreateUniquePropName(fields, name);
                fields.Add(name, arg);
            }

            return fields;
        }

        private string CreateUniquePropName(Dictionary<string, Expression> initializers, string initialName)
        {
            var namePrefix = initialName;
            var ind = 0;

            var name = "";

            do
            {
                name = $"{namePrefix}{ind++}";
            } while (initializers.ContainsKey(name));

            return name;
        }
    }
}
