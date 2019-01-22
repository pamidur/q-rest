using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QRest.AspNetCore.OData.Metadata
{
    public class ConventionalModelBuilder : IModelBuilder
    {
        private readonly IApiDescriptionGroupCollectionProvider _provider;
        private readonly ODataOptions _options;

        public ConventionalModelBuilder(IApiDescriptionGroupCollectionProvider provider, IOptions<ODataOptions> options)
        {
            _provider = provider;
            _options = options.Value;
        }

        public ODataModel Build()
        {
            var model = ODataModel.New(_options.Namespace);
            foreach (var api in _provider.ApiDescriptionGroups.Items.SelectMany(i => i.Items).ToArray())
            {
                var modelType = GetActionElementType(api.ActionDescriptor);
                if (modelType != null)
                    model.MapSet(modelType, GetEdmName(api.ActionDescriptor), FormatUrl(api.RelativePath, api.ActionDescriptor.Parameters));
            }

            return model;
        }

        public string GetEdmName(ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor is ControllerActionDescriptor cad)
                return cad.ControllerName;

            throw new NotSupportedException();
        }

        private Type GetActionElementType(ActionDescriptor actionDescriptor)
        {
            return actionDescriptor.Parameters[0].ParameterType.GetGenericArguments()[0].GetGenericArguments()[0];
        }

        private string FormatUrl(string relativePath, IList<ParameterDescriptor> parameters)
        {
            relativePath = relativePath.Replace(_options.MetadataPath.ToString().TrimStart('/'), "").TrimStart('/');

            foreach (var par in parameters)
                relativePath = relativePath.Replace($"{{{par.Name}}}", "").TrimEnd('/');

            return relativePath;
        }
    }
}
