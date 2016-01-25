using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Microsoft.AspNet.SignalR;

namespace Abp.Web.SignalR
{
    public class WindsorDependencyResolver : DefaultDependencyResolver
    {
        private readonly IWindsorContainer _windsorContainer;

        public WindsorDependencyResolver(IWindsorContainer windsorContainer)
        {
            _windsorContainer = windsorContainer;
        }

        public override object GetService(Type serviceType)
        {
            return _windsorContainer.Kernel.HasComponent(serviceType)
                ? _windsorContainer.Resolve(serviceType)
                : base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _windsorContainer.Kernel.HasComponent(serviceType)
                ? _windsorContainer.ResolveAll(serviceType).Cast<object>()
                : base.GetServices(serviceType);
        }
    }
}