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
        /// Build context.
        /// </summary>
        private readonly DynamicApiBuildContext _context;

        /// <summary>
        /// Building controller info object.
        /// </summary>
        private readonly DynamicApiControllerInfo _controllerInfo;

        /// <summary>
        /// Creates a new instance of ApiControllerInfoBuilder.
        /// </summary>
        public ApiControllerBuilder()
        {
            _context = new DynamicApiBuildContext();
            _controllerInfo = new DynamicApiControllerInfo(typeof(AbpDynamicApiController<T>), typeof(T));
        }

        /// <summary>
        /// Used to specify a method definition.
        /// </summary>
        /// <param name="methodName">Name of the method in proxied type</param>
        /// <returns>Action builder</returns>
        public IApiControllerActionBuilder<T> ForMethod(string methodName)
        {
            return new ApiControllerActionBuilder<T>(this, _context, methodName);
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
                if (_context.CustomizedMethods.ContainsKey(method.Name))
                {
                    var actionName = _context.CustomizedMethods[method.Name].ActionName;
                    _controllerInfo.Actions[actionName] = _context.CustomizedMethods[method.Name];
                }
                else
                {
                    var apiMethodInfo = new DynamicApiActionInfo(method.Name, method);
                    _controllerInfo.Actions[method.Name] = apiMethodInfo;
                }
            }

            DynamicApiControllerManager.RegisterServiceController(_controllerInfo);
        }
    }
}