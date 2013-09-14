using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// Used to generate dynamic api controllers for arbitrary types.
    /// </summary>
    public static class BuildApiController
    {
        /// <summary>
        /// Reference to current Ioc container.
        /// </summary>
        internal static WindsorContainer IocContainer { get; set; }

        /// <summary>
        /// Generates a new dynamic api controller for given type.
        /// </summary>
        /// <typeparam name="T">Type of the proxied object</typeparam>
        public static IApiControllerBuilder<T> For<T>()
        {
            //TODO: Move into ApiControllerInfoBuilder.Build() ?
            IocContainer.Register(
                Component.For<AbpDynamicApiControllerInterceptor<T>>().LifestyleTransient(),
                Component.For<AbpDynamicApiController<T>>().Proxy.AdditionalInterfaces(new[] { typeof(T) }).Interceptors<AbpDynamicApiControllerInterceptor<T>>().LifestyleTransient()
                );

            return new ApiControllerBuilder<T>();
        }
    }
}
