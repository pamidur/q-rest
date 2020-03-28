using QRest.Core.Compilation;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public sealed class NewOperation : OperationBase
    {
        internal NewOperation() { }

        public override string Key { get; } = "new";

        public override Expression CreateExpression(Expression context, IReadOnlyList<Expression> arguments, IAssembler assembler)
        {
            var expression = arguments.Any() ? CreateContainer(assembler, arguments) : context;
            return expression;
        }        

        private static Expression CreateContainer(IAssembler assembler, IReadOnlyList<Expression> arguments)
        {
            var fields = new Dictionary<string, Expression>();
            foreach (var arg in arguments)
            {
                var name = assembler.GetName(arg);
                if (fields.ContainsKey(name)) name = CreateUniquePropName(fields, name);
                fields.Add(name, arg);
            }

            return assembler.ContainerFactory.CreateContainer(fields);
        }

        private static string CreateUniquePropName(Dictionary<string, Expression> initializers, string initialName)
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
