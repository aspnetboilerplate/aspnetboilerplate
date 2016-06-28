using System;
using System.Collections.Generic;
using Abp.Application.Services;
using Abp.AspNetCore.Configuration;
using Abp.Extensions;
using Abp.MsDependencyInjection.Extensions;
using Abp.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Abp.Collections.Extensions;
using Castle.Components.DictionaryAdapter;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

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
            ConfigureSelector(controller);
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

            foreach (var action in controller.Actions)
            {
                ConfigureApiExplorer(action);
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

        private void ConfigureSelector(ControllerModel controller)
        {
            RemoveEmptySelectors(controller.Selectors);

            var moduleName = GetModuleNameOrDefault(controller.ControllerType.AsType());
            foreach (var action in controller.Actions)
            {
                ConfigureSelector(moduleName, controller.ControllerName, action);
            }
        }

        private void ConfigureSelector(string moduleName, string controllerName, ActionModel action)
        {
            RemoveEmptySelectors(action.Selectors);

            var actionConstraints = new List<IActionConstraintMetadata>();

            foreach (var selector in action.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    return;
                }

                if (!selector.ActionConstraints.IsNullOrEmpty())
                {
                    actionConstraints.AddRange(selector.ActionConstraints);
                }
            }

            if (!actionConstraints.Any())
            {
                actionConstraints.Add(new HttpMethodActionConstraint(new[] { "POST" }));
            }

            action.Selectors.Clear();

            var selectorModel = new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(
                    new RouteAttribute(
                        $"api/services/{moduleName}/{controllerName}/{action.ActionName}"
                    )
                )
            };
            
            foreach (var actionConstraint in actionConstraints)
            {
                selectorModel.ActionConstraints.Add(actionConstraint);
            }

            action.Selectors.Add(selectorModel);
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

        private void RemoveEmptySelectors(IList<SelectorModel> selectors)
        {
            selectors
                .Where(IsEmptySelector)
                .ToList()
                .ForEach(s => selectors.Remove(s));
        }

        private bool IsEmptySelector(SelectorModel s)
        {
            return s.AttributeRouteModel == null && s.ActionConstraints.IsNullOrEmpty();
        }
    }
}