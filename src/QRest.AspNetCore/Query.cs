using Microsoft.AspNetCore.Mvc;
using QRest.AspNetCore.Contracts;
using QRest.Core.Contracts;
using System;

namespace QRest.AspNetCore
{
    public abstract class Query
    {
        public IQueryStructure Structure { get; }
        public ICompiler Compiller { get; }

        protected Query(IQueryStructure structure, ICompiler compiller)
        {
            Structure = structure;
            Compiller = compiller;
        }
    }

    public abstract class TypedQueryBase : Query
    {
        protected TypedQueryBase(IQueryStructure structure, ICompiler compiller) : base(structure, compiller)
        {
        }

        public abstract Type SourceType { get; }        
    }

    [ModelBinder(typeof(QueryModelBinder))]
    public class Query<T> : TypedQueryBase
    {
        private readonly Type _sourceType;

        public Query(IQueryStructure structure, ICompiler compiller) : base(structure, compiller)
        {
            _sourceType = typeof(T);
        }

        public override Type SourceType => _sourceType;

        public object Apply(T source)
        {
            var lambda = Compiller.Compile<T>(Structure.Data);
            var result = lambda(source);
            return result;
        }
    }
}
