using System.Reflection;
using System.Web.Http;
using Abp.Data.Repositories.NHibernate.Core;
using Abp.Services;
using Abp.Services.Core.Impl;
using Abp.Web.Dependency.Interceptors;
using Castle.Core;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Abp.Web.Controllers;

namespace Abp.Web.Dependency.Installers
{
    public class AbpInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //Log4Net
            container.AddFacility<LoggingFacility>(f => f.UseLog4Net());

            //Interceptors
            container.Register(

                //ApiController interceptor
                Component.For<AbpApiControllerInterceptor>().LifeStyle.Transient,

                //All repoistories //TODO: Web is dependent to NHibernate now!!!
                Classes.FromAssembly(Assembly.GetAssembly(typeof(NhUserRepository))).InSameNamespaceAs<NhUserRepository>().WithService.DefaultInterfaces().LifestyleTransient(),

                //All services
                Classes.FromAssembly(Assembly.GetAssembly(typeof(IService))).InSameNamespaceAs<UserService>().WithService.DefaultInterfaces().LifestyleTransient(),

                //All api controllers
                Classes.FromThisAssembly().BasedOn<AbpApiController>().LifestyleTransient()

                );
        }
    }
}
