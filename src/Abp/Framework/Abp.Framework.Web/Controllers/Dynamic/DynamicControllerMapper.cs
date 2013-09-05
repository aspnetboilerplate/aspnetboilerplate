using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Abp.Web.Controllers.Dynamic
{
    public static class DynamicControllerMapper
    {
        internal static WindsorContainer IocContainer { get; set; }

        public static void Map<T>()
        {
            IocContainer.Register(
                Component.For<AbpDynamicApiControllerInterceptor<T>>().LifestyleTransient(),
                Component.For<AbpDynamicApiController<T>>().Proxy.AdditionalInterfaces(new[] {typeof (T)}).Interceptors<AbpDynamicApiControllerInterceptor<T>>().LifestyleTransient()
                );

            DynamicControllerManager.RegisterServiceController(new DynamicControllerInfo
                                                                   {
                                                                       Name = "task",
                                                                       Type = typeof (AbpDynamicApiController<T>)
                                                                   });
        }
    }

    internal static class DynamicControllerManager
    {
        private static ConcurrentDictionary<string, DynamicControllerInfo> _dynamicTypes = new ConcurrentDictionary<string, DynamicControllerInfo>();

        public static DynamicControllerInfo FindServiceController(string serviceName)
        {
            DynamicControllerInfo controllerInfo;
            return _dynamicTypes.TryGetValue(serviceName, out controllerInfo) ? controllerInfo : null;
        }

        public static void RegisterServiceController(DynamicControllerInfo controllerInfo)
        {
            _dynamicTypes[controllerInfo.Name] = controllerInfo;
        }
    }

    internal class DynamicControllerInfo
    {
        public string Name { get; set; }

        public Type Type { get; set; }
    }
}
