using QRest.Core.Contracts;

namespace QRest.Core.Extensions
{
    static class OperationExtensions
    {
        public static string GetName(this IOperation operation)
        {
            return operation.GetType().Name.ToLowerInvariant().Replace("operation", "");
        }
    }
}
