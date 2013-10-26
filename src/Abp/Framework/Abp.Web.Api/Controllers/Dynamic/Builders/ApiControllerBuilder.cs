using System;
using System.Collections.Generic;
using System.Reflection;
using Abp.Exceptions;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Abp.Utils.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    internal class ApiControllerBuilder
    {

        /// <summary>
        /// Reference to current Ioc container.
        /// </summary>
        internal static WindsorContainer IocContainer { get; set; }
    }

    /// <summary>
    /// Used to build <see cref="DynamicApiControllerInfo"/> object.
    /// </summary>
    /// <typeparam name="T">The of the proxied object</typeparam>
    internal class ApiControllerBuilder<T> : ApiControllerBuilder, IApiControllerBuilder<T>
    {
        /// <summary>
        /// List of all builders.
        /// </summary>
        private readonly IDictionary<string, ApiControllerActionBuilder<T>> _actionBuilders;

        /// <summary>
        /// Name of the controller.
        /// </summary>
        private readonly string _controllerName;
        
        /// <summary>
        /// Area name.
        /// </summary>
        private readonly string _areaName;

        /// <summary>
        /// Creates a new instance of ApiControllerInfoBuilder.
        /// </summary>
        /// <param name="controllerName"> </param>
        public ApiControllerBuilder(string controllerName)
        {
            if (string.IsNullOrWhiteSpace(controllerName) || !controllerName.Contains("/"))
            {
                throw new ArgumentException("controllerName is not valid! It must be formatted as {areaName}/{controllerName}", "controllerName");
            }

            var splittedArray = controllerName.Split('/');
            _areaName = splittedArray[0].ToPascalCase();
            _controllerName = splittedArray[1].ToPascalCase();

            _actionBuilders = new Dictionary<string, ApiControllerActionBuilder<T>>();
            foreach (var methodInfo in GetPublicInstanceMethods())
            {
                _actionBuilders[methodInfo.Name] = new ApiControllerActionBuilder<T>(this, methodInfo);
            }
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

        /// <summary>
        /// Builds the controller.
        /// This method must be called at last of the build operation.
        /// </summary>
        public void Build()
        {
            var controllerInfo = new DynamicApiControllerInfo(_areaName, _controllerName, typeof(AbpDynamicApiController<T>), typeof(T));
            foreach (var actionBuilder in _actionBuilders.Values)
            {
                if (actionBuilder.DontCreate)
                {
                    continue;
                }

                var actionInfo = actionBuilder.BuildActionInfo();
                controllerInfo.Actions[actionInfo.ActionName] = actionInfo;
            }

            IocContainer.Register(
                Component.For<AbpDynamicApiControllerInterceptor<T>>().LifestyleTransient(),
                Component.For<AbpDynamicApiController<T>>().Proxy.AdditionalInterfaces(new[] { typeof(T) }).Interceptors<AbpDynamicApiControllerInterceptor<T>>().LifestyleTransient()
                );

            DynamicApiControllerManager.RegisterServiceController(controllerInfo);
        }

        #region Private methods

        private static IEnumerable<MethodInfo> GetPublicInstanceMethods()
        {
            return typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance);
        }

        #endregion
    }
}