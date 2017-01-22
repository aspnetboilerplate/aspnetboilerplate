using System.Reflection;
using Abp.Web;
using System.Web.Http.Filters;
using System.Linq;
using Abp.Reflection;
using System.Web.Http;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// Used to build <see cref="DynamicApiActionInfo"/> object.
    /// </summary>
    /// <typeparam name="T">Type of the proxied object</typeparam>
    internal class ApiControllerActionBuilder<T> : IApiControllerActionBuilder<T>
    {
        /// <summary>
        /// Selected action name.
        /// </summary>
        public string ActionName { get; }

        /// <summary>
        /// Underlying proxying method.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Selected Http verb.
        /// </summary>
        public HttpVerb? Verb { get; set; }

        /// <summary>
        /// Is API Explorer enabled.
        /// </summary>
        public bool? IsApiExplorerEnabled { get; set; }

        /// <summary>
        /// Action Filters for dynamic controller method.
        /// </summary>
        public IFilter[] Filters { get; set; }

        /// <summary>
        /// A flag to set if no action will be created for this method.
        /// </summary>
        public bool DontCreate { get; set; }

        /// <summary>
        /// Reference to the <see cref="ApiControllerBuilder{T}"/> which created this object.
        /// </summary>
        public IApiControllerBuilder Controller
        {
            get { return _controller; }
        }
        private readonly ApiControllerBuilder<T> _controller;

        /// <summary>
        /// Creates a new <see cref="ApiControllerActionBuilder{T}"/> object.
        /// </summary>
        /// <param name="apiControllerBuilder">Reference to the <see cref="ApiControllerBuilder{T}"/> which created this object</param>
        /// <param name="methodInfo">Method</param>
        public ApiControllerActionBuilder(ApiControllerBuilder<T> apiControllerBuilder, MethodInfo methodInfo)
        {
            _controller = apiControllerBuilder;
            Method = methodInfo;
            ActionName = Method.Name;
        }

        /// <summary>
        /// Used to specify Http verb of the action.
        /// </summary>
        /// <param name="verb">Http very</param>
        /// <returns>Action builder</returns>
        public IApiControllerActionBuilder<T> WithVerb(HttpVerb verb)
        {
            Verb = verb;
            return this;
        }

        /// <summary>
        /// Enables/Disables API Explorer for the action.
        /// </summary>
        public IApiControllerActionBuilder<T> WithApiExplorer(bool isEnabled)
        {
            IsApiExplorerEnabled = isEnabled;
            return this;
        }

        /// <summary>
        /// Used to specify another method definition.
        /// </summary>
        /// <param name="methodName">Name of the method in proxied type</param>
        /// <returns>Action builder</returns>
        public IApiControllerActionBuilder<T> ForMethod(string methodName)
        {
            return _controller.ForMethod(methodName);
        }

        /// <summary>
        /// Used to add action filters to apply to this method.
        /// </summary>
        /// <param name="filters"> Action Filters to apply.</param>
        public IApiControllerActionBuilder<T> WithFilters(params IFilter[] filters)
        {
            Filters = filters;
            return this;
        }

        /// <summary>
        /// Tells builder to not create action for this method.
        /// </summary>
        /// <returns>Controller builder</returns>
        public IApiControllerBuilder<T> DontCreateAction()
        {
            DontCreate = true;
            return _controller;
        }

        /// <summary>
        /// Builds the controller.
        /// This method must be called at last of the build operation.
        /// </summary>
        public void Build()
        {
            _controller.Build();
        }

        /// <summary>
        /// Builds <see cref="DynamicApiActionInfo"/> object for this configuration.
        /// </summary>
        /// <param name="conventionalVerbs"></param>
        /// <returns></returns>
        internal DynamicApiActionInfo BuildActionInfo(bool conventionalVerbs)
        {
            return new DynamicApiActionInfo(
                ActionName,
                GetNormalizedVerb(conventionalVerbs),
                Method,
                Filters,
                IsApiExplorerEnabled
            );
        }

        private HttpVerb GetNormalizedVerb(bool conventionalVerbs)
        {
            if (Verb != null)
            {
                return Verb.Value;
            }

            if (Method.IsDefined(typeof(HttpGetAttribute)))
            {
                return HttpVerb.Get;
            }

            if (Method.IsDefined(typeof(HttpPostAttribute)))
            {
                return HttpVerb.Post;
            }

            if (Method.IsDefined(typeof(HttpPutAttribute)))
            {
                return HttpVerb.Put;
            }

            if (Method.IsDefined(typeof(HttpDeleteAttribute)))
            {
                return HttpVerb.Delete;
            }

            if (Method.IsDefined(typeof(HttpPatchAttribute)))
            {
                return HttpVerb.Patch;
            }

            if (Method.IsDefined(typeof(HttpOptionsAttribute)))
            {
                return HttpVerb.Options;
            }

            if (Method.IsDefined(typeof(HttpHeadAttribute)))
            {
                return HttpVerb.Head;
            }

            if (conventionalVerbs)
            {
                var conventionalVerb = DynamicApiVerbHelper.GetConventionalVerbForMethodName(ActionName);
                if (conventionalVerb == HttpVerb.Get && !HasOnlyPrimitiveIncludingNullableTypeParameters(Method))
                {
                    conventionalVerb = DynamicApiVerbHelper.GetDefaultHttpVerb();
                }

                return conventionalVerb;
            }

            return DynamicApiVerbHelper.GetDefaultHttpVerb();
        }

        private static bool HasOnlyPrimitiveIncludingNullableTypeParameters(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().All(p => TypeHelper.IsPrimitiveExtendedIncludingNullable(p.ParameterType) || p.IsDefined(typeof(FromUriAttribute)));
        }
    }
}