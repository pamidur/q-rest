using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace QRest.Core.Compilation.Containers
{
    public class EmitContainerFactory : IContainerFactory
    {
        private static readonly ModuleBuilder _module;
        private static readonly ConcurrentDictionary<int, TypeInfo> _containerCache = new ConcurrentDictionary<int, TypeInfo>();

        static EmitContainerFactory()
        {
            var name = new AssemblyName("$qrest-containers");
            var ab = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            _module = ab.DefineDynamicModule(name.Name);
        }

        public Expression CreateContainer(IReadOnlyDictionary<string, Expression> properties)
        {
            properties = properties.ToImmutableSortedDictionary();

            var key = GetKey(properties);

            if (!_containerCache.TryGetValue(key, out var type))
            {
                lock (_module)
                {
                    if (!_containerCache.TryGetValue(key, out type))
                    {

                        var tb = _module.DefineType($"$qc_{key}", TypeAttributes.Public | TypeAttributes.Sealed);
                        var gparams = tb.DefineGenericParameters(properties.Keys.ToArray());

                        var ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, gparams);

                        var il = ctor.GetILGenerator();

                        for (int i = 0; i < gparams.Length; i++)
                        {
                            var param = gparams[i];

                            var field = tb.DefineField($"_{param.Name}", param, FieldAttributes.Private);
                            var prop = tb.DefineProperty(param.Name, PropertyAttributes.None, param, new Type[0]);
                            var getter = tb.DefineMethod($"get_{param.Name}", MethodAttributes.Public | MethodAttributes.SpecialName, CallingConventions.HasThis, param, new Type[0]);
                            var ilm = getter.GetILGenerator();

                            ilm.Emit(OpCodes.Ldarg_0);
                            ilm.Emit(OpCodes.Ldfld, field);
                            ilm.Emit(OpCodes.Ret);

                            prop.SetGetMethod(getter);

                            ctor.DefineParameter(i + 1, ParameterAttributes.None, param.Name);

                            il.Emit(OpCodes.Ldarg, 0);
                            il.Emit(OpCodes.Ldarg, i + 1);
                            il.Emit(OpCodes.Stfld, field);
                        }

                        il.Emit(OpCodes.Ret);

                        _containerCache[key] = type = tb.CreateTypeInfo();
                    }
                }
            }

            var argTypes = properties.Values.Select(p => p.Type).ToArray();
            var gtype = type.MakeGenericType(argTypes);

            return Expression.New(gtype.GetConstructors()[0], properties.Values);
        }

        private int GetKey(IReadOnlyDictionary<string, Expression> properties)
        {
            return properties.Keys.Aggregate(0, (a, v) => unchecked(a + v.GetHashCode()));
        }

        public Expression CreateReadProperty(Expression context, string name)
        {
            throw new NotSupportedException();
        }

        public bool IsContainerExpression(Expression expression)
        {
            return false;
        }
    }
}
