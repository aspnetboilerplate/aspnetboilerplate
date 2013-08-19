using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Abp.Localization;
using Abp.Web.Dependency.Interceptors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ExamCenter.Data.Repositories.NHibernate;
using ExamCenter.Services.Impl;
using Castle.DynamicProxy;

namespace ExamCenter.Web.Dependency.Installers
{
    public class ExamCenterInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                //All MVC controllers
                Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient(),

                //All Web Api Controllers
                Classes.FromThisAssembly().BasedOn<ApiController>().LifestyleTransient(),

                //All repoistories
                Classes.FromAssembly(Assembly.GetAssembly(typeof(NhQuestionRepository))).InSameNamespaceAs<NhQuestionRepository>().WithService.DefaultInterfaces().LifestyleTransient(),

                //All services
                Classes.FromAssembly(Assembly.GetAssembly(typeof(QuestionService))).InSameNamespaceAs<QuestionService>().WithService.DefaultInterfaces().LifestyleTransient(),

                //Localization manager
                Component.For<ILocalizationManager>().ImplementedBy<NullLocalizationManager>().LifestyleSingleton()
                
                );
        }
    }
}