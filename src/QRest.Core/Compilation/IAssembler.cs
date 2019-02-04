using QRest.Core.Compilation.Containers;
using QRest.Core.Compilation.TypeConverters;
using System.Linq.Expressions;

namespace QRest.Core.Compilation
{
    public interface IAssembler
    {
        IContainerFactory ContainerFactory { get; }
        ITypeConverter TypeConverter { get; }

        string GetName(Expression expression);
        Expression SetName(Expression expression, string name);
    }
}
