using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Microsoft.AspNet.SignalR;

namespace Abp.Web.SignalR
{
    /// <summary>
    /// 替换<see cref ="DefaultDependencyResolver"/>来解决Castle Windsor的依赖关系（<see cref ="IWindsorContainer"/>）。
    /// </summary>
    public class WindsorDependencyResolver : DefaultDependencyResolver
    {
        private readonly IWindsorContainer _windsorContainer;

        /// <summary>
        /// 初始化<see cref ="WindsorDependencyResolver"/>类的新实例。
        /// </summary>
        /// <param name="windsorContainer">The windsor container.</param>
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