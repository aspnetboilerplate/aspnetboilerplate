using System;
using Abp.Application.Services;
using Abp.AspNetCore.Configuration;
using Abp.Extensions;
using Abp.MsDependencyInjection.Extensions;
using Abp.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.AspNetCore.Mvc.Conventions
{
    public class AbpAppServiceConvention : IApplicationModelConvention
    {
        private readonly Lazy<AbpAspNetCoreConfiguration> _configuration;

        public AbpAppServiceConvention(IServiceCollection services)
        {
            _configuration = new Lazy<AbpAspNetCoreConfiguration>(() =>
            {
                return services
                    .GetSingletonService<AbpBootstrapper>()
                    .IocManager
                    .Resolve<AbpAspNetCoreConfiguration>();
            }, true);
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var type = controller.ControllerType.AsType();

                if (typeof(IApplicationService).IsAssignableFrom(type))
                {
                    NormalizeAppServiceControllerName(controller);
                    ConfigureRemoteService(controller);
                }
                else
                {
                    var remoteServiceAtt = ReflectionHelper.GetSingleAttributeOrDefault<RemoteServiceAttribute>(type);
                    if (remoteServiceAtt != null && remoteServiceAtt.IsEnabledFor(type))
                    {
                        ConfigureRemoteService(controller);
                    }
                }
            }
        }

        private void ConfigureRemoteService(ControllerModel controller)
        {
            ConfigureApiExplorer(controller);
            AddSelector(controller);

            foreach (var action in controller.Actions)
            {
                ConfigureApiExplorer(action);
            }
        }

        private void ConfigureApiExplorer(ControllerModel controller)
        {
            if (controller.ApiExplorer.GroupName.IsNullOrEmpty())
            {
                controller.ApiExplorer.GroupName = controller.ControllerName;
            }

            if (controller.ApiExplorer.IsVisible == null)
            {
                controller.ApiExplorer.IsVisible = true;
            }
        }

        private void ConfigureApiExplorer(ActionModel action)
        {
            if (action.ApiExplorer.IsVisible == null)
            {
                var remoteServiceAtt = ReflectionHelper.GetSingleAttributeOrDefault<RemoteServiceAttribute>(action.ActionMethod);
                action.ApiExplorer.IsVisible = remoteServiceAtt?.IsEnabledFor(action.ActionMethod);
            }
        }

        private void AddSelector(ControllerModel controller)
        {
            var moduleName = GetModuleNameOrDefault(controller.ControllerType.AsType());
            
            var selectorModel = new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(
                    new RouteAttribute(
                        $"api/services/{moduleName}/{controller.ControllerName}/[action]"
                    )
                )
            };

            controller.Selectors.Clear();
            controller.Selectors.Add(selectorModel);
        }

        private string GetModuleNameOrDefault(Type controllerType)
        {
            foreach (var controllerSetting in _configuration.Value.ServiceControllerSettings)
            {
                if (controllerSetting.Assembly == controllerType.Assembly)
                {
                    return controllerSetting.ModuleName;
                }
            }

            return AbpServiceControllerSetting.DefaultServiceModuleName;
        }

        private static void NormalizeAppServiceControllerName(ControllerModel controller)
        {
            if (controller.ControllerName.EndsWith("AppService"))
            {
                controller.ControllerName = controller.ControllerName.Left(controller.ControllerName.Length - "AppService".Length);
            }
            else if (controller.ControllerName.EndsWith("Service"))
            {
                controller.ControllerName = controller.ControllerName.Left(controller.ControllerName.Length - "Service".Length);
            }
        }
    }
}