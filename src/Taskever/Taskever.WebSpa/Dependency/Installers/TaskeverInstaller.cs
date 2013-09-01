using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Abp.Localization;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Taskever.Data.Repositories.NHibernate;
using Taskever.Services.Impl;

namespace Taskever.Web.Dependency.Installers
{
    public class TaskeverInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                //All MVC controllers
                Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient(),

                //All Web Api Controllers
                Classes.FromThisAssembly().BasedOn<ApiController>().LifestyleTransient(),

                //All repoistories
                Classes.FromAssembly(Assembly.GetAssembly(typeof(NhTaskRepository))).InSameNamespaceAs<NhTaskRepository>().WithService.DefaultInterfaces().LifestyleTransient(),

                //All services
                Classes.FromAssembly(Assembly.GetAssembly(typeof(TaskService))).InSameNamespaceAs<TaskService>().WithService.DefaultInterfaces().LifestyleTransient(),

                //Localization manager
                Component.For<ILocalizationManager>().ImplementedBy<NullLocalizationManager>().LifestyleSingleton()
                
                );
        }
    }
}