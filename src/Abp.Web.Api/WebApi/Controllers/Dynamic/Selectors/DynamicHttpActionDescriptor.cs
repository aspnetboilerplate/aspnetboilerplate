using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Web.Http.Controllers;
using System.Collections.ObjectModel;
using System.Web.Http.Filters;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Web.Models;

namespace Abp.WebApi.Controllers.Dynamic.Selectors
{
    public class DynamicHttpActionDescriptor : ReflectedHttpActionDescriptor
    {
        /// <summary>
        /// The Action filters for the Action Descriptor.
        /// </summary>
        private readonly IFilter[] _filters;

        public override Type ReturnType
        {
            get
            {
                return typeof(AjaxResponse);
            }
        }

        public DynamicHttpActionDescriptor(HttpControllerDescriptor controllerDescriptor, MethodInfo methodInfo, IFilter[] filters = null)
            : base(controllerDescriptor, methodInfo)
        {
            _filters = filters;
        }

        public override System.Threading.Tasks.Task<object> ExecuteAsync(HttpControllerContext controllerContext, System.Collections.Generic.IDictionary<string, object> arguments, System.Threading.CancellationToken cancellationToken)
        {
            return base
                .ExecuteAsync(controllerContext, arguments, cancellationToken)
                .ContinueWith(task =>
                {
                    try
                    {
                        if (task.Result == null)
                        {
                            return new AjaxResponse();
                        }

                        if (task.Result is AjaxResponse)
                        {
                            return task.Result;
                        }
                        
                        return new AjaxResponse(task.Result);
                    }
                    catch (AggregateException ex)
                    {
                        ex.InnerException.ReThrow();
                        throw; // The previous line will throw, but we need this to make compiler happy
                    }
                }, cancellationToken);
        }

        /// <summary>
        /// The overrides the GetFilters for the action and adds the Dynamic Action filters.
        /// </summary>
        /// <returns> The Collection of filters.</returns>
        public override Collection<IFilter> GetFilters()
        {
            var actionFilters = new Collection<IFilter>();

            if (!_filters.IsNullOrEmpty())
            {
                foreach (var filter in _filters)
                {
                    actionFilters.Add(filter);
                }
            }

            foreach (var baseFilter in base.GetFilters())
            {
                actionFilters.Add(baseFilter);
            }
            return actionFilters;
        }
    }
}