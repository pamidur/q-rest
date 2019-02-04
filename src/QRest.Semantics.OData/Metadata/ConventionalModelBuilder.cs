using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Semantics.OData.Metadata
{
    public class ConventionalModelBuilder : IEdmBuilder
    {
        private readonly IApiDescriptionGroupCollectionProvider _provider;
        private readonly ODataOptions _options;
        private readonly string[] _rootChunks;
        private readonly Type _typedActionResult = typeof(ActionResult<>);
        private readonly Type _typedQueryable = typeof(IQueryable<>);

        public ConventionalModelBuilder(IApiDescriptionGroupCollectionProvider provider, IOptions<ODataOptions> options)
        {
            _provider = provider;
            _options = options.Value;

            _rootChunks = ToStringChunks(TemplateParser.Parse(_options.ServiceRoot.ToString().Trim('/')).Segments);
        }

        public ODataModel Build()
        {
            var model = ODataModel.New(_options.Namespace);
            foreach (var api in _provider.ApiDescriptionGroups.Items.SelectMany(i => i.Items).ToArray())
            {
                var type = GetSetType(api);
                var url = GetUrl(api);
                var edmName = GetEdmName(api.ActionDescriptor);

                if (type != null && edmName != null && url != null)
                    model.MapSet(type, edmName, url.ToLowerInvariant());
            }

            return model;
        }

        public string GetEdmName(ActionDescriptor actionDescriptor)
        {
            var name = "";

            if (actionDescriptor is ControllerActionDescriptor cad)
            {
                name = cad.ControllerName;

                var route = cad.EndpointMetadata
                    .Where(a => a is IActionHttpMethodProvider provider && provider.HttpMethods.Contains("GET"))
                    .OfType<IRouteTemplateProvider>().FirstOrDefault();

                if (route != null && !route.Template.StartsWith("{"))
                    name += $"_{cad.ActionName}";
            }

            if (!string.IsNullOrEmpty(name))
                return name;

            return null;
        }

        private Type GetSetType(ApiDescription api)
        {
            if (api.ActionDescriptor is ControllerActionDescriptor cad)
            {
                var ret = cad.MethodInfo.ReturnType;
                if (!ret.IsGenericType || ret.GetGenericTypeDefinition() != _typedActionResult)
                    return default;

                var retType = ret.GenericTypeArguments[0];
                if (!retType.IsGenericType || retType.GetGenericTypeDefinition() != _typedQueryable)
                    return default;

                return retType.GenericTypeArguments[0];
            }

            throw new NotSupportedException();
        }

        private string GetUrl(ApiDescription api)
        {
            var template = TemplateParser.Parse(api.RelativePath);

            var pathSegments = template.Segments.Where(s => s.Parts.All(p => !p.IsParameter || !api.ParameterDescriptions.First(pd=>pd.ParameterDescriptor.Name == p.Name).IsRequired)).ToList();

            if (template.Segments.Count != pathSegments.Count)
                return default;

            var pathChunks = ToStringChunks(pathSegments);            

            for (int i = 0; i < _rootChunks.Length; i++)            
                if(_rootChunks[i]!=pathChunks[i])
                    return default;           

            return string.Join("/", pathChunks.Skip(_rootChunks.Length));
        }

        private string[] ToStringChunks(IList<TemplateSegment> segments)
        {
            return segments
                .Select(s => string.Join("", s.Parts.Select(p => p.Text))).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }
    }
}
