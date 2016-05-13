using Abp.WebApi.Controllers.ApiExplorer;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Controllers.Dynamic.Selectors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Dispatcher;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

//TODO: This code need to be refactored.

namespace Abp.Web.Api.Description
{
    public class AbpApiExplorer : ApiExplorer,IApiExplorer
    {
        private Lazy<Collection<ApiDescription>> _apiDescriptions;
        private HttpConfiguration _config;
        public AbpApiExplorer(HttpConfiguration config):base(config)
        {
            _apiDescriptions = new Lazy<Collection<ApiDescription>>(InitializeApiDescriptions);
            _config = config;

        }

       public new  Collection<ApiDescription> ApiDescriptions
        {
            get
            {
                return _apiDescriptions.Value;
            }
        }

        private Collection<ApiDescription> InitializeApiDescriptions() {
           
            Collection<ApiDescription> apiDescriptions = new Collection<ApiDescription>();
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
                    ApiDescription api = new ApiDescription();
                    var httpaction = new HttpControllerDescriptor();
                    httpaction.Configuration = _config;
                    httpaction.ControllerType = dynamicapiinfo.ServiceInterfaceType;
                    httpaction.ControllerName = dynamicapiinfo.ServiceName;               
                    var action = new DynamicHttpActionDescriptor(httpaction, item.Value.Method, item.Value.Filters);
                    api.ActionDescriptor = action;
                    api.HttpMethod = GetMethod(item.Value.Verb);
                    IActionValueBinder actionValueBinder = _config.Services.GetActionValueBinder();
                    HttpActionBinding actionBinding = actionValueBinder.GetBinding(action);

                    //parameter
                    IList<ApiParameterDescription> parameterDescriptions = CreateParameterDescription(actionBinding,action);
                    //using refletions to internal set 
                    var prop=typeof(ApiDescription).GetProperties().Where(p => p.Name == "ParameterDescriptions").SingleOrDefault();
                    prop.SetValue(api, new Collection<ApiParameterDescription>(parameterDescriptions));        


                    //resopnse
                    ResponseDescription responseDescription = CreateResponseDescription(action);
                    var prop2 = typeof(ApiDescription).GetProperties().Where(p => p.Name == "ResponseDescription").SingleOrDefault();
                    prop2.SetValue(api, responseDescription);
                    
                    api.RelativePath = "api/services/"+dynamicapiinfo.ServiceName + "/" + item.Value.ActionName;
                    
                    apiDescriptions.Add(api);
                }
            }
            return apiDescriptions;
        }
        private IList<ApiParameterDescription> CreateParameterDescription(HttpActionBinding actionBinding,HttpActionDescriptor actionDescriptor)
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
        private HttpMethod GetMethod(Abp.Web.HttpVerb verb)
        {
            if (verb == HttpVerb.Post)
                return HttpMethod.Post;
            else if (verb == HttpVerb.Get)
                return HttpMethod.Get;
            else if (verb == HttpVerb.Delete)
                return HttpMethod.Delete;
            else if (verb == HttpVerb.Put)
                return HttpMethod.Put;
            else if (verb == HttpVerb.Trace)
                return HttpMethod.Trace;
            else if (verb == HttpVerb.Options)
                return HttpMethod.Options;
            else if (verb == HttpVerb.Head)
                return HttpMethod.Head;
            else
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
