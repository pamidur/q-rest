using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace QRest.AspNetCore.OData.Metadata
{
    public class ConventionalModelBuilder : IModelBuilder
    {
        private readonly IApiDescriptionGroupCollectionProvider _provider;
        private readonly IOptions<ODataOptions> _options;

        public ConventionalModelBuilder(IApiDescriptionGroupCollectionProvider provider, IOptions<ODataOptions> options)
        {
            _provider = provider;
            _options = options;
        }

        public ODataModel Build()
        {
            throw new NotImplementedException();
        }
    }
}
