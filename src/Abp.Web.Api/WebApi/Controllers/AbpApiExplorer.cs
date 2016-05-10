using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Controllers.Dynamic.Selectors;

//TODO: This code need to be refactored.

namespace Abp.Web.Api.Description
{
    public class AbpApiExplorer : ApiExplorer, IApiExplorer
    {
        private readonly Lazy<Collection<ApiDescription>> _apiDescriptions;
        private readonly HttpConfiguration _config;

        public AbpApiExplorer(HttpConfiguration config) : base(config)
        {
            _apiDescriptions = new Lazy<Collection<ApiDescription>>(InitializeApiDescriptions);
            _config = config;
        }

        public new Collection<ApiDescription> ApiDescriptions
        {
            get { return _apiDescriptions.Value; }
        }

        private Collection<ApiDescription> InitializeApiDescriptions()
        {
            var apiDescriptions = new Collection<ApiDescription>();
            //webapi
            foreach (var item in base.ApiDescriptions)
            {
                apiDescriptions.Add(item);
            }

            //dynamic api
            var dynamicapiinfos = DynamicApiControllerManager.GetAll();
            foreach (var dynamicapiinfo in dynamicapiinfos)
            {
                foreach (var item in dynamicapiinfo.Actions)
                {
                    var api = new ApiDescription();
                    var httpaction = new HttpControllerDescriptor();
                    httpaction.Configuration = _config;
                    httpaction.ControllerType = dynamicapiinfo.ServiceInterfaceType;
                    httpaction.ControllerName = dynamicapiinfo.ServiceName;
                    var action = new DynamicHttpActionDescriptor(httpaction, item.Value.Method, item.Value.Filters);
                    api.ActionDescriptor = action;
                    api.HttpMethod = GetMethod(item.Value.Verb);
                    var actionValueBinder = _config.Services.GetActionValueBinder();
                    var actionBinding = actionValueBinder.GetBinding(action);

                    //parameter
                    var parameterDescriptions = CreateParameterDescription(actionBinding, action);
                    //using refletions to internal set
                    var prop =
                        typeof(ApiDescription).GetProperties()
                            .Where(p => p.Name == "ParameterDescriptions")
                            .SingleOrDefault();
                    prop.SetValue(api, new Collection<ApiParameterDescription>(parameterDescriptions));

                    //resopnse
                    var responseDescription = CreateResponseDescription(action);
                    var prop2 =
                        typeof(ApiDescription).GetProperties()
                            .Where(p => p.Name == "ResponseDescription")
                            .SingleOrDefault();
                    prop2.SetValue(api, responseDescription);

                    api.RelativePath = "api/services/" + dynamicapiinfo.ServiceName + "/" + item.Value.ActionName;

                    apiDescriptions.Add(api);
                }
            }
            return apiDescriptions;
        }

        private IList<ApiParameterDescription> CreateParameterDescription(HttpActionBinding actionBinding,
            HttpActionDescriptor actionDescriptor)
        {
            IList<ApiParameterDescription> parameterDescriptions = new List<ApiParameterDescription>();
            // try get parameter binding information if available
            if (actionBinding != null)
            {
                var parameterBindings = actionBinding.ParameterBindings;
                if (parameterBindings != null)
                {
                    foreach (var parameter in parameterBindings)
                    {
                        parameterDescriptions.Add(CreateParameterDescriptionFromBinding(parameter));
                    }
                }
            }
            else
            {
                var parameters = actionDescriptor.GetParameters();
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
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
                Source = ApiParameterSource.Unknown
            };
        }

        private ApiParameterDescription CreateParameterDescriptionFromBinding(HttpParameterBinding parameterBinding)
        {
            var parameterDescription = CreateParameterDescriptionFromDescriptor(parameterBinding.Descriptor);
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
            var responseTypeAttribute = actionDescriptor.GetCustomAttributes<ResponseTypeAttribute>();
            var responseType = responseTypeAttribute.Select(attribute => attribute.ResponseType).FirstOrDefault();

            return new ResponseDescription
            {
                DeclaredType = actionDescriptor.ReturnType,
                ResponseType = responseType,
                Documentation = GetApiResponseDocumentation(actionDescriptor)
            };
        }

        private string GetApiResponseDocumentation(HttpActionDescriptor actionDescriptor)
        {
            var documentationProvider = DocumentationProvider ??
                                        actionDescriptor.Configuration.Services.GetDocumentationProvider();
            if (documentationProvider != null)
            {
                return documentationProvider.GetResponseDocumentation(actionDescriptor);
            }

            return null;
        }

        private string GetApiParameterDocumentation(HttpParameterDescriptor parameterDescriptor)
        {
            var documentationProvider = DocumentationProvider ??
                                        parameterDescriptor.Configuration.Services.GetDocumentationProvider();
            if (documentationProvider != null)
            {
                return documentationProvider.GetDocumentation(parameterDescriptor);
            }

            return null;
        }

        private HttpMethod GetMethod(HttpVerb verb)
        {
            if (verb == HttpVerb.Post)
                return HttpMethod.Post;
            if (verb == HttpVerb.Get)
                return HttpMethod.Get;
            if (verb == HttpVerb.Delete)
                return HttpMethod.Delete;
            if (verb == HttpVerb.Put)
                return HttpMethod.Put;
            if (verb == HttpVerb.Trace)
                return HttpMethod.Trace;
            if (verb == HttpVerb.Options)
                return HttpMethod.Options;
            if (verb == HttpVerb.Head)
                return HttpMethod.Head;
            return HttpMethod.Post;
        }
    }

    internal static class HttpParameterBindingExtensions
    {
        public static bool WillReadUri(this HttpParameterBinding parameterBinding)
        {
            if (parameterBinding == null)
            {
                throw new ArgumentNullException("parameterBinding");
            }

            var valueProviderParameterBinding = parameterBinding as IValueProviderParameterBinding;
            if (valueProviderParameterBinding != null)
            {
                var valueProviderFactories = valueProviderParameterBinding.ValueProviderFactories;
                if (valueProviderFactories.Any() &&
                    valueProviderFactories.All(factory => factory is IUriValueProviderFactory))
                {
                    return true;
                }
            }

            return false;
        }
    }
}