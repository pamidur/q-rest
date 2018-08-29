using QRest.Core.Containers;
using QRest.Core.Contracts;
using QRest.Core.Conventions;
using QRest.Core.Operations;
using QRest.Core.Operations.Aggregations;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations.Query;
using System.Collections.Generic;

namespace QRest.Core
{
    public class Registry
    {
        private readonly Dictionary<string, IOperation> _operations = new Dictionary<string, IOperation>();
        public IReadOnlyDictionary<string, IOperation> Operations => _operations;

        public void RegisterOperation<T>(string name = null)
            where T : IOperation, new()
        {
            name = name ?? typeof(T).Name.ToLowerInvariant().Replace("operation", "");
            _operations.Add(name, new T());
        }

        public INameConvention PropertyNameConvention { get; } = new PascalCaseConvention();
        public INameConvention MethodNameConvention { get; } = new PascalCaseConvention();        


        public static void RegisterDefaultOperations(Registry registry)
        {
            registry.RegisterOperation<NotEqualOperation>("ne");
            registry.RegisterOperation<EqualOperation>("eq");
            registry.RegisterOperation<NotOperation>();
            registry.RegisterOperation<WhereOperation>();
            registry.RegisterOperation<SelectOperation>();
            registry.RegisterOperation<OneOfOperation>();
            registry.RegisterOperation<EveryOperation>();
            registry.RegisterOperation<FirstOperation>();
            registry.RegisterOperation<CountOperation>();
            registry.RegisterOperation<ItOperation>();
            registry.RegisterOperation<SumOperation>();
            registry.RegisterOperation<WithOperation>();
        }

    }
}
