using System.Reflection;
using System.Web.Http.Controllers;
using System.Collections.ObjectModel;
using System.Web.Http.Filters;
using Abp.Collections.Extensions;
using Abp.Reflection;
using Abp.Web.Models;

namespace Abp.WebApi.Controllers.Dynamic.Selectors
{
    public class DynamicHttpActionDescriptor : ReflectedHttpActionDescriptor
    {
        /// <summary>
        /// The Action filters for the Action Descriptor.
        /// </summary>
        private readonly IFilter[] _filters;

        public DynamicHttpActionDescriptor(HttpControllerDescriptor controllerDescriptor, MethodInfo methodInfo, IFilter[] filters = null)
            : base(controllerDescriptor, methodInfo)
        {
            _filters = filters;

            Properties["__AbpDynamicApiDontWrapResultAttribute"] =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrNull<WrapResultAttribute>(methodInfo)
                ?? WrapResultAttribute.Default;
        }

        /// <summary>
        /// Overrides the GetFilters for the action and adds the Dynamic Action filters.
        /// </summary>
        /// <returns> The Collection of filters.</returns>
        public override Collection<IFilter> GetFilters()
        {
            if (_filters.IsNullOrEmpty())
            {
                return base.GetFilters();
            }

            var actionFilters = new Collection<IFilter>();

            foreach (var filter in _filters)
            {
                actionFilters.Add(filter);
            }

            foreach (var baseFilter in base.GetFilters())
            {
                actionFilters.Add(baseFilter);
            }

            return actionFilters;
        }
    }
}