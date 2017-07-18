using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using Abp.Collections.Extensions;
using Abp.Reflection;
using Abp.Web;
using Abp.WebApi.Configuration;
using Abp.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Selectors
{
    internal static class DynamicApiDescriptorHelper
    {
        internal static ReadOnlyCollection<T> FilterType<T>(object[] objects) where T : class
        {
            int max = objects.Length;
            List<T> list = new List<T>(max);
            int idx = 0;
            for (int i = 0; i < max; i++)
            {
                T attr = objects[i] as T;
                if (attr != null)
                {
                    list.Add(attr);
                    idx++;
                }
            }
            list.Capacity = idx;

            return new ReadOnlyCollection<T>(list);
        }
    }

    public class DynamicHttpActionDescriptor : ReflectedHttpActionDescriptor
    {
        public override Collection<HttpMethod> SupportedHttpMethods { get; }

        private readonly DynamicApiActionInfo _actionInfo;
        private readonly Lazy<Collection<IFilter>> _filters;
        private readonly Lazy<Collection<HttpParameterDescriptor>> _parameters;

        private readonly object[] _attributes;
        private readonly object[] _declaredOnlyAttributes;
        
        public DynamicHttpActionDescriptor(
            IAbpWebApiConfiguration configuration,
            HttpControllerDescriptor controllerDescriptor,
            DynamicApiActionInfo actionInfo)
            : base(
                  controllerDescriptor,
                  actionInfo.Method)
        {
            _actionInfo = actionInfo;
            SupportedHttpMethods = new Collection<HttpMethod> { actionInfo.Verb.ToHttpMethod() };

            Properties["__AbpDynamicApiActionInfo"] = actionInfo;
            Properties["__AbpDynamicApiDontWrapResultAttribute"] =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    actionInfo.Method,
                    configuration.DefaultDynamicApiWrapResultAttribute
                );

            _filters = new Lazy<Collection<IFilter>>(GetFiltersInternal, true);
            _parameters = new Lazy<Collection<HttpParameterDescriptor>>(GetParametersInternal, true);

            _declaredOnlyAttributes = _actionInfo.Method.GetCustomAttributes(inherit: false);
            _attributes = _actionInfo.Method.GetCustomAttributes(inherit: true);
        }

        /// <summary>
        /// Overrides the GetFilters for the action and adds the Dynamic Action filters.
        /// </summary>
        /// <returns> The Collection of filters.</returns>
        public override Collection<IFilter> GetFilters()
        {
            return _filters.Value;
        }

        public override Collection<T> GetCustomAttributes<T>(bool inherit)
        {
            object[] attributes = inherit ? _attributes : _declaredOnlyAttributes;
            return new Collection<T>(DynamicApiDescriptorHelper.FilterType<T>(attributes));
        }

        public override Collection<HttpParameterDescriptor> GetParameters()
        {
            return _parameters.Value;
        }

        private Collection<IFilter> GetFiltersInternal()
        {
            if (_actionInfo.Filters.IsNullOrEmpty())
            {
                return base.GetFilters();
            }

            var actionFilters = new Collection<IFilter>();

            foreach (var filter in _actionInfo.Filters)
            {
                actionFilters.Add(filter);
            }

            foreach (var baseFilter in base.GetFilters())
            {
                actionFilters.Add(baseFilter);
            }

            return actionFilters;
        }

        private Collection<HttpParameterDescriptor> GetParametersInternal()
        {
            var parameters = base.GetParameters();

            if (_actionInfo.Verb.IsIn(HttpVerb.Get, HttpVerb.Head))
            {
                foreach (var parameter in parameters)
                {
                    if (parameter.ParameterBinderAttribute == null)
                    {
                        parameter.ParameterBinderAttribute = new FromUriAttribute();
                    }
                }
            }

            return parameters;
        }
    }
}