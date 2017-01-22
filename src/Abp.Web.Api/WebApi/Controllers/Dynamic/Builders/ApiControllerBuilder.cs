using System;
using System.Collections.Generic;
using System.Web.Http.Filters;
using Abp.Application.Services;
using Abp.Dependency;
using Abp.Reflection.Extensions;
using Abp.WebApi.Controllers.Dynamic.Interceptors;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// Used to build <see cref="DynamicApiControllerInfo"/> object.
    /// </summary>
    /// <typeparam name="T">The of the proxied object</typeparam>
    internal class ApiControllerBuilder<T> : IApiControllerBuilder<T>
    {
        /// <summary>
        /// Name of the controller.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets type of the service interface for this dynamic controller.
        /// </summary>
        public Type ServiceInterfaceType { get; }

        /// <summary>
        /// Action Filters to apply to this dynamic controller.
        /// </summary>
        public IFilter[] Filters { get; set; }

        /// <summary>
        /// Is API Explorer enabled.
        /// </summary>
        public bool? IsApiExplorerEnabled { get; set; }

        /// <summary>
        /// True, if using conventional verbs for this dynamic controller.
        /// </summary>
        public bool ConventionalVerbs { get; set; }

        /// <summary>
        /// List of all action builders for this controller.
        /// </summary>
        private readonly IDictionary<string, ApiControllerActionBuilder<T>> _actionBuilders;

        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Creates a new instance of ApiControllerInfoBuilder.
        /// </summary>
        /// <param name="serviceName">Name of the controller</param>
        /// <param name="iocResolver">Ioc resolver</param>
        public ApiControllerBuilder(string serviceName, IIocResolver iocResolver)
        {
            Check.NotNull(iocResolver, nameof(iocResolver));

            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentException("serviceName null or empty!", "serviceName");
            }

            if (!DynamicApiServiceNameHelper.IsValidServiceName(serviceName))
            {
                throw new ArgumentException("serviceName is not properly formatted! It must contain a single-depth namespace at least! For example: 'myapplication/myservice'.", "serviceName");
            }

            _iocResolver = iocResolver;

            ServiceName = serviceName;
            ServiceInterfaceType = typeof (T);

            _actionBuilders = new Dictionary<string, ApiControllerActionBuilder<T>>();

            foreach (var methodInfo in DynamicApiControllerActionHelper.GetMethodsOfType(typeof(T)))
            {
                var actionBuilder = new ApiControllerActionBuilder<T>(this, methodInfo);

                var remoteServiceAttr = methodInfo.GetSingleAttributeOrNull<RemoteServiceAttribute>();
                if (remoteServiceAttr != null && !remoteServiceAttr.IsEnabledFor(methodInfo))
                {
                    actionBuilder.DontCreateAction();
                }

                _actionBuilders[methodInfo.Name] = actionBuilder;
            }
        }

        /// <summary>
        /// The adds Action filters for the whole Dynamic Controller
        /// </summary>
        /// <param name="filters"> The filters. </param>
        /// <returns>The current Controller Builder </returns>
        public IApiControllerBuilder<T> WithFilters(params IFilter[] filters)
        {
            Filters = filters;
            return this;
        }

        /// <summary>
        /// Used to specify a method definition.
        /// </summary>
        /// <param name="methodName">Name of the method in proxied type</param>
        /// <returns>Action builder</returns>
        public IApiControllerActionBuilder<T> ForMethod(string methodName)
        {
            if (!_actionBuilders.ContainsKey(methodName))
            {
                throw new AbpException("There is no method with name " + methodName + " in type " + typeof(T).Name);
            }

            return _actionBuilders[methodName];
        }

        public IApiControllerBuilder<T> ForMethods(Action<IApiControllerActionBuilder> action)
        {
            foreach (var actionBuilder in _actionBuilders.Values)
            {
                action(actionBuilder);
            }

            return this;
        }

        public IApiControllerBuilder<T> WithConventionalVerbs()
        {
            ConventionalVerbs = true;
            return this;
        }

        public IApiControllerBuilder<T> WithApiExplorer(bool isEnabled)
        {
            IsApiExplorerEnabled = isEnabled;
            return this;
        }

        /// <summary>
        /// Builds the controller.
        /// This method must be called at last of the build operation.
        /// </summary>
        public void Build()
        {
            var controllerInfo = new DynamicApiControllerInfo(
                ServiceName,
                ServiceInterfaceType,
                typeof(DynamicApiController<T>),
                typeof(AbpDynamicApiControllerInterceptor<T>),
                Filters,
                IsApiExplorerEnabled
                );
            
            foreach (var actionBuilder in _actionBuilders.Values)
            {
                if (actionBuilder.DontCreate)
                {
                    continue;
                }

                controllerInfo.Actions[actionBuilder.ActionName] = actionBuilder.BuildActionInfo(ConventionalVerbs);
            }

            _iocResolver.Resolve<DynamicApiControllerManager>().Register(controllerInfo);
        }
    }
}