using System.Collections.Generic;
using System.Reflection;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// Used to build <see cref="DynamicApiControllerInfo"/> object.
    /// </summary>
    /// <typeparam name="T">The of the proxied object</typeparam>
    internal class ApiControllerBuilder<T> : IApiControllerBuilder<T>
    {

        /// <summary>
        /// Building controller info object.
        /// </summary>
        private readonly DynamicApiControllerInfo _controllerInfo;

        private readonly IDictionary<string, ApiControllerActionBuilder<T>> _actionBuilders;

        public bool UsingConventions { get; private set; }

        /// <summary>
        /// Creates a new instance of ApiControllerInfoBuilder.
        /// </summary>
        public ApiControllerBuilder()
        {
            _actionBuilders = new Dictionary<string, ApiControllerActionBuilder<T>>();
            _controllerInfo = new DynamicApiControllerInfo(typeof(AbpDynamicApiController<T>), typeof(T));
        }

        /// <summary>
        /// Used to specify a method definition.
        /// </summary>
        /// <param name="methodName">Name of the method in proxied type</param>
        /// <returns>Action builder</returns>
        public IApiControllerActionBuilder<T> ForMethod(string methodName)
        {
            return _actionBuilders[methodName] = new ApiControllerActionBuilder<T>(this, methodName);
        }

        /// <summary>
        /// Sets name of the api controller.
        /// </summary>
        /// <param name="controllerName">Api controller name</param>
        /// <returns>Controller builder</returns>
        public IApiControllerBuilder<T> WithControllerName(string controllerName)
        {
            _controllerInfo.Name = controllerName;
            return this;
        }

        /// <summary>
        /// Builds the controller.
        /// This method must be called at last of the build operation.
        /// </summary>
        public void Build()
        {
            if (string.IsNullOrWhiteSpace(_controllerInfo.Name))
            {
                _controllerInfo.Name = DynamicApiHelper.GetConventionalControllerName<T>();
            }

            foreach (var method in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                DynamicApiActionInfo actionInfo;
                if (_actionBuilders.ContainsKey(method.Name))
                {
                    var actionBuilder = _actionBuilders[method.Name];
                    if(actionBuilder.DontCreate)
                    {
                        continue;
                    }

                    actionInfo = actionBuilder.ActionInfo;
                }
                else
                {
                    actionInfo = new DynamicApiActionInfo(method.Name, method);
                    if (UsingConventions)
                    {
                        actionInfo.Verb = DynamicApiHelper.GetConventionalVerbForMethodName(method.Name);
                    }
                }

                _controllerInfo.Actions[actionInfo.ActionName] = actionInfo;
            }

            DynamicApiControllerManager.RegisterServiceController(_controllerInfo);
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
    }
}