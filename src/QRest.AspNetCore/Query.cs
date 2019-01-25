using Microsoft.AspNetCore.Mvc;
using QRest.AspNetCore.Contracts;
using QRest.Core.Contracts;

namespace QRest.AspNetCore
{
    [ModelBinder(typeof(QueryModelBinder))]
    public class Query
    {
        public IQueryStructure Structure { get; }
        public ICompiler Compiller { get; }      

        public Query(IQueryStructure structure, ICompiler compiller)
        {
            Structure = structure;
            Compiller = compiller;
        }

        public object Apply<T>(T source)
        {
            var lambda = Compiller.Compile<T>(Structure.Data);
            var result = lambda(source);
            return result;
        }
    }
}
