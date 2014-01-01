using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Startup;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Modules
{
    internal class AbpModuleSystemInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<AbpModuleCollection>().LifestyleSingleton(),
                Component.For<AbpModuleManager>().LifestyleSingleton()
                );
        }
    }
}
