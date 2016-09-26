using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using Abp.Application.Services;
using Abp.Dependency;
using Abp.WebApi.Configuration;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Controllers.Dynamic.Selectors;

//TODO: This code need to be refactored.

namespace Abp.WebApi.Controllers.ApiExplorer
{
    public class AbpApiExplorer : System.Web.Http.Description.ApiExplorer, IApiExplorer, ISingletonDependency
    {
        private readonly Lazy<Collection<ApiDescription>> _apiDescriptions;
        private readonly IAbpWebApiConfiguration _abpWebApiConfiguration;
        private readonly DynamicApiControllerManager _dynamicApiControllerManager;

        public AbpApiExplorer(
            IAbpWebApiConfiguration abpWebApiConfiguration,
            DynamicApiControllerManager dynamicApiControllerManager
            ) : base(abpWebApiConfiguration.HttpConfiguration)
        {
            _apiDescriptions = new Lazy<Collection<ApiDescription>>(InitializeApiDescriptions);
            _abpWebApiConfiguration = abpWebApiConfiguration;
            _dynamicApiControllerManager = dynamicApiControllerManager;
        }

        public new Collection<ApiDescription> ApiDescriptions
        {
            get
            {
                return _apiDescriptions.Value;
            }
        }

        private Collection<ApiDescription> InitializeApiDescriptions()
        {
            var apiDescriptions = new Collection<ApiDescription>();

            foreach (var item in base.ApiDescriptions)
            {
                apiDescriptions.Add(item);
            }

            var dynamicApiControllerInfos = _dynamicApiControllerManager.GetAll();
            foreach (var dynamicApiControllerInfo in dynamicApiControllerInfos)
            {
                if (IsApiExplorerDisabled(dynamicApiControllerInfo))
                {
                    continue;
                }

                foreach (var dynamicApiActionInfo in dynamicApiControllerInfo.Actions.Values)
                {
                    if (IsApiExplorerDisabled(dynamicApiActionInfo))
                    {
                        continue;
                    }

                    var apiDescription = new ApiDescription();

                    var controllerDescriptor = new DynamicHttpControllerDescriptor(_abpWebApiConfiguration.HttpConfiguration, dynamicApiControllerInfo);
                    var actionDescriptor = new DynamicHttpActionDescriptor(_abpWebApiConfiguration, controllerDescriptor, dynamicApiActionInfo);

                    apiDescription.ActionDescriptor = actionDescriptor;
                    apiDescription.HttpMethod = actionDescriptor.SupportedHttpMethods[0];

                    var actionValueBinder = _abpWebApiConfiguration.HttpConfiguration.Services.GetActionValueBinder();
                    var actionBinding = actionValueBinder.GetBinding(actionDescriptor);

                    apiDescription.ParameterDescriptions.Clear();
                    foreach (var apiParameterDescription in CreateParameterDescription(actionBinding, actionDescriptor))
                    {
                        apiDescription.ParameterDescriptions.Add(apiParameterDescription);
                    }

                    SetResponseDescription(apiDescription, actionDescriptor);

                    apiDescription.RelativePath = "api/services/" + dynamicApiControllerInfo.ServiceName + "/" + dynamicApiActionInfo.ActionName;

                    apiDescriptions.Add(apiDescription);
                }
            }

            return apiDescriptions;
        }

        private static bool IsApiExplorerDisabled(DynamicApiControllerInfo dynamicApiControllerInfo)
        {
            if (dynamicApiControllerInfo.IsApiExplorerEnabled == false)
            {
                if (!RemoteServiceAttribute.IsMetadataExplicitlyEnabledFor(dynamicApiControllerInfo.ServiceInterfaceType))
                {
                    return true;
                }
            }
            else
            {
                if (RemoteServiceAttribute.IsMetadataExplicitlyDisabledFor(dynamicApiControllerInfo.ServiceInterfaceType))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsApiExplorerDisabled(DynamicApiActionInfo dynamicApiActionInfo)
        {
            if (dynamicApiActionInfo.IsApiExplorerEnabled == false)
            {
                if (!RemoteServiceAttribute.IsMetadataExplicitlyEnabledFor(dynamicApiActionInfo.Method))
                {
                    return true;
                }
            }
            else
            {
                if (RemoteServiceAttribute.IsMetadataExplicitlyDisabledFor(dynamicApiActionInfo.Method))
                {
                    return true;
                }
            }

            return false;
        }

        private void SetResponseDescription(ApiDescription apiDescription, DynamicHttpActionDescriptor actionDescriptor)
        {
            var responseDescription = CreateResponseDescription(actionDescriptor);
            var prop2 = typeof(ApiDescription).GetProperties().Single(p => p.Name == "ResponseDescription");
            prop2.SetValue(apiDescription, responseDescription);
        }

        private IList<ApiParameterDescription> CreateParameterDescription(HttpActionBinding actionBinding, HttpActionDescriptor actionDescriptor)
        {
            IList<ApiParameterDescription> parameterDescriptions = new List<ApiParameterDescription>();
            // try get parameter binding information if available
            if (actionBinding != null)
            {
                HttpParameterBinding[] parameterBindings = actionBinding.ParameterBindings;
                if (parameterBindings != null)
                {
                    foreach (HttpParameterBinding parameter in parameterBindings)
                    {
                        parameterDescriptions.Add(CreateParameterDescriptionFromBinding(parameter));
                    }
                }
            }
            else
            {
                Collection<HttpParameterDescriptor> parameters = actionDescriptor.GetParameters();
                if (parameters != null)
                {
                    foreach (HttpParameterDescriptor parameter in parameters)
                    {
                        parameterDescriptions.Add(CreateParameterDescriptionFromDescriptor(parameter));
                    }
                }
            }


            return parameterDescriptions;
        }
        
        private ApiParameterDescription CreateParameterDescriptionFromDescriptor(HttpParameterDescriptor parameter)
        {

            return new ApiParameterDescription
            {
                ParameterDescriptor = parameter,
                Name = parameter.Prefix ?? parameter.ParameterName,
                Documentation = GetApiParameterDocumentation(parameter),
                Source = ApiParameterSource.Unknown,
            };
        }

        private ApiParameterDescription CreateParameterDescriptionFromBinding(HttpParameterBinding parameterBinding)
        {
            ApiParameterDescription parameterDescription = CreateParameterDescriptionFromDescriptor(parameterBinding.Descriptor);
            if (parameterBinding.WillReadBody)
            {
                parameterDescription.Source = ApiParameterSource.FromBody;
            }
            else if (parameterBinding.WillReadUri())
            {
                parameterDescription.Source = ApiParameterSource.FromUri;
            }

            return parameterDescription;
        }

        private ResponseDescription CreateResponseDescription(HttpActionDescriptor actionDescriptor)
        {
            Collection<ResponseTypeAttribute> responseTypeAttribute = actionDescriptor.GetCustomAttributes<ResponseTypeAttribute>();
            Type responseType = responseTypeAttribute.Select(attribute => attribute.ResponseType).FirstOrDefault();

            return new ResponseDescription
            {
                DeclaredType = actionDescriptor.ReturnType,
                ResponseType = responseType,
                Documentation = GetApiResponseDocumentation(actionDescriptor)
            };
        }

        private string GetApiResponseDocumentation(HttpActionDescriptor actionDescriptor)
        {
            IDocumentationProvider documentationProvider = DocumentationProvider ?? actionDescriptor.Configuration.Services.GetDocumentationProvider();
            if (documentationProvider != null)
            {
                return documentationProvider.GetResponseDocumentation(actionDescriptor);
            }

            return null;
        }

        private string GetApiParameterDocumentation(HttpParameterDescriptor parameterDescriptor)
        {
            IDocumentationProvider documentationProvider = DocumentationProvider ?? parameterDescriptor.Configuration.Services.GetDocumentationProvider();
            if (documentationProvider != null)
            {
                return documentationProvider.GetDocumentation(parameterDescriptor);
            }

            return null;
        }
    }

    internal static class HttpParameterBindingExtensions
    {
        public static bool WillReadUri(this HttpParameterBinding parameterBinding)
        {
            if (parameterBinding == null)
            {
                throw new ArgumentNullException(nameof(parameterBinding));
            }

            IValueProviderParameterBinding valueProviderParameterBinding = parameterBinding as IValueProviderParameterBinding;
            if (valueProviderParameterBinding != null)
            {
                IEnumerable<ValueProviderFactory> valueProviderFactories = valueProviderParameterBinding.ValueProviderFactories;
                if (valueProviderFactories.Any() && valueProviderFactories.All(factory => factory is IUriValueProviderFactory))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
