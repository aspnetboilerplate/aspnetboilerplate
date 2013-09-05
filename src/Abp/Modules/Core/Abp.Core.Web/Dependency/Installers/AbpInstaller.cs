using System.Reflection;
using System.Web.Http;
using Abp.Data.Repositories;
using Abp.Data.Repositories.NHibernate;
using Abp.Modules.Core.Data.Repositories.NHibernate;
using Abp.Modules.Core.Services.Impl;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Modules.Core.Dependency.Installers
{
    public class AbpInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //Interceptors
            container.Register(

                //All MVC controllers
                //Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient(),

                //All Web Api Controllers
                Classes.FromThisAssembly().BasedOn<ApiController>().LifestyleTransient(),

                //Generic repositories
                Component.For(typeof(IRepository<>)).ImplementedBy(typeof(NhRepositoryBase<>)).LifestyleTransient(),
                Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(NhRepositoryBase<,>)).LifestyleTransient(),
                
                //All repoistories //TODO: Web is dependent to NHibernate now!!!
                Classes.FromAssembly(Assembly.GetAssembly(typeof(NhUserRepository))).InSameNamespaceAs<NhUserRepository>().WithService.DefaultInterfaces().LifestyleTransient(),

                //All services
                Classes.FromAssembly(Assembly.GetAssembly(typeof(UserService))).InSameNamespaceAs<UserService>().WithService.DefaultInterfaces().LifestyleTransient()

                );
        }
    }
}
