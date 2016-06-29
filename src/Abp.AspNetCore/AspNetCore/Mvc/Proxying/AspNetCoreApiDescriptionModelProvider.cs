using System.Linq;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Dependency;
using Abp.Web.Api.Modeling;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Abp.AspNetCore.Mvc.Proxying
{
    public class AspNetCoreApiDescriptionModelProvider : IApiDescriptionModelProvider, ISingletonDependency
    {
        public ILogger Logger { get; set; }

        private readonly IApiDescriptionGroupCollectionProvider _descriptionProvider;
        private readonly AbpAspNetCoreConfiguration _configuration;

        public AspNetCoreApiDescriptionModelProvider(
            IApiDescriptionGroupCollectionProvider descriptionProvider,
            AbpAspNetCoreConfiguration configuration)
        {
            _descriptionProvider = descriptionProvider;
            _configuration = configuration;

            Logger = NullLogger.Instance;
        }

        public ApplicationApiDescriptionModel CreateModel()
        {
            var model = new ApplicationApiDescriptionModel();
            
            foreach (var descriptionGroupItem in _descriptionProvider.ApiDescriptionGroups.Items)
            {
                foreach (var apiDescription in descriptionGroupItem.Items)
                {
                    AddApiDescriptionToModel(apiDescription, model);
                }
            }

            return model;
        }

        private void AddApiDescriptionToModel(ApiDescription apiDescription, ApplicationApiDescriptionModel model)
        {
            var moduleName = GetModuleName(apiDescription);
            var module = model.GetOrAddModule(moduleName);
            var controller = module.GetOrAddController(apiDescription.GroupName);
            var actionName = apiDescription.ActionDescriptor.GetMethodInfo().Name;

            if (controller.Actions.ContainsKey(actionName))
            {
                Logger.Warn($"Controller '{controller.Name}' contains more than one action with name '{actionName}' for module '{moduleName}'. Ignored: " + apiDescription.ActionDescriptor.GetMethodInfo());
                return;
            }

            var action = controller.AddAction(CreateActionApiDescriptionModel(apiDescription));
            foreach (var parameterDescription in apiDescription.ParameterDescriptions)
            {
                action.AddParameter(CreateParameterApiDescriptionModel(parameterDescription));
            }
        }

        private static ActionApiDescriptionModel CreateActionApiDescriptionModel(ApiDescription apiDescription)
        {
            var method = apiDescription.ActionDescriptor.GetMethodInfo();
            return new ActionApiDescriptionModel(
                method,
                method.Name,
                apiDescription.RelativePath,
                apiDescription.HttpMethod
            );
        }

        private string GetModuleName(ApiDescription apiDescription)
        {
            var controllerType = apiDescription.ActionDescriptor.GetMethodInfo().DeclaringType; //TODO: What about base class methods?
            if (controllerType == null)
            {
                return AbpServiceControllerSetting.DefaultServiceModuleName;
            }

            foreach (var controllerSetting in _configuration.ServiceControllerSettings)
            {
                if (controllerType.Assembly == controllerSetting.Assembly)
                {
                    return controllerSetting.ModuleName;
                }
            }

            return AbpServiceControllerSetting.DefaultServiceModuleName;
        }

        private static ParameterApiDescriptionModel CreateParameterApiDescriptionModel(ApiParameterDescription parameterDescription)
        {
            return new ParameterApiDescriptionModel(
                parameterDescription.Name,
                parameterDescription.Type,
                parameterDescription.RouteInfo?.IsOptional ?? false,
                parameterDescription.RouteInfo?.DefaultValue,
                parameterDescription.RouteInfo?.Constraints?.Select(c => c.GetType().Name).ToArray(),
                parameterDescription.Source.Id
            );
        }
    }
}
