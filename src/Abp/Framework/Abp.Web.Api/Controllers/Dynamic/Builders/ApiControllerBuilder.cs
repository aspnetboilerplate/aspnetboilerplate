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
        private string _controllerName;

        /// <summary>
        /// True if using conventions.
        /// </summary>
        public bool UsingConventions { get; private set; }

        /// <summary>
        /// Creates a new instance of ApiControllerInfoBuilder.
        /// </summary>
        public ApiControllerBuilder()
        {
            _actionBuilders = new Dictionary<string, ApiControllerActionBuilder<T>>();
            foreach (var methodInfo in GetPublicInstanceMethods())
            {
                _actionBuilders[methodInfo.Name] = new ApiControllerActionBuilder<T>(this, methodInfo);
            }
        }

        /// <summary>
        /// Used to tell builder to use conventions for api.
        /// </summary>
        /// <returns>Controller builder</returns>
        public IApiControllerBuilder<T> UseConventions()
        {
            UsingConventions = true;
            return this;
        }

        /// <summary>
        /// Sets name of the api controller.
        /// </summary>
        /// <param name="controllerName">Api controller name</param>
        /// <returns>Controller builder</returns>
        public IApiControllerBuilder<T> WithControllerName(string controllerName)
        {
            _controllerName = controllerName;
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

        /// <summary>
        /// Builds the controller.
        /// This method must be called at last of the build operation.
        /// </summary>
        public void Build()
        {
            string areaName = null;

            if (string.IsNullOrWhiteSpace(_controllerName))
            {
                //TODO: How we can determine area name?
                _controllerName = UsingConventions ? DynamicApiHelper.GetConventionalControllerName<T>() : typeof(T).Name;
            }
            else
            {
                if (_controllerName.Contains("/"))
                {
                    var splittedArray = _controllerName.Split('/');
                    areaName = splittedArray[0].ToPascalCase();
                    _controllerName = splittedArray[1].ToPascalCase();
                }

                _controllerName = _controllerName.ToPascalCase();
            }

            var controllerInfo = new DynamicApiControllerInfo(areaName, _controllerName, typeof(AbpDynamicApiController<T>), typeof(T));
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

        private IEnumerable<MethodInfo> GetPublicInstanceMethods()
        {
            return typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance);
        }

        #endregion
    }
}