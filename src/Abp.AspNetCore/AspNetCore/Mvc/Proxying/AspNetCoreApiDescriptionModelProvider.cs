using System.Linq;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Dependency;
using Abp.Web.Api.Modeling;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Abp.AspNetCore.Mvc.Proxying
{
    public class AspNetCoreApiDescriptionModelProvider : IApiDescriptionModelProvider, ISingletonDependency
    {
        private readonly IApiDescriptionGroupCollectionProvider _descriptionProvider;

        public AspNetCoreApiDescriptionModelProvider(IApiDescriptionGroupCollectionProvider descriptionProvider)
        {
            _descriptionProvider = descriptionProvider;
        }

        public ApplicationApiDescriptionModel CreateModel()
        {
            var model = new ApplicationApiDescriptionModel();
            
            foreach (var descriptionGroupItem in _descriptionProvider.ApiDescriptionGroups.Items)
            {
                foreach (var apiDescription in descriptionGroupItem.Items)
                {
                    var module = model.GetOrAddModule("app"); //TODO: Get right module
                    var controller = module.GetOrAddController(apiDescription.GroupName);
                    var action = controller.AddAction(
                        new ActionApiDescriptionModel(
                            apiDescription.ActionDescriptor.GetMethodInfo().Name,
                            apiDescription.HttpMethod,
                            apiDescription.RelativePath
                        )
                    );

                    foreach (var parameterDescription in apiDescription.ParameterDescriptions)
                    {
                        action.AddParameter(
                            new ParameterApiDescriptionModel(
                                parameterDescription.Name,
                                parameterDescription.Type.FullName,
                                parameterDescription.RouteInfo.IsOptional,
                                parameterDescription.RouteInfo.DefaultValue,
                                parameterDescription.RouteInfo.Constraints?.Select(c => c.GetType().Name).ToArray(),
                                parameterDescription.Source.Id
                            )
                        );
                    }
                }
            }

            return model;
        }
    }
}
