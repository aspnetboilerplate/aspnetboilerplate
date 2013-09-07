using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abp.Modules.Core.Data.Repositories.NHibernate;
using Abp.Modules.Core.Services.Impl;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Modules.Core.Startup.Dependency
{
    public class AbpCoreDataModuleDependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                //All repoistories //TODO: Move to Abp.Modules.Core.Data?
                Classes.FromAssembly(Assembly.GetAssembly(typeof(NhUserRepository))).InSameNamespaceAs<NhUserRepository>().WithService.DefaultInterfaces().LifestyleTransient(),

                //All services //TODO: Move to Abp.Modules.Core?
                Classes.FromAssembly(Assembly.GetAssembly(typeof(UserService))).InSameNamespaceAs<UserService>().WithService.DefaultInterfaces().LifestyleTransient()

                );
        }
    }
}
