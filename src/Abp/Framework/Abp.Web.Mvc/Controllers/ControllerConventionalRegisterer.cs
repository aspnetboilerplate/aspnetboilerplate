using System.Web.Mvc;
using Abp.Dependency.Conventions;
using Castle.MicroKernel.Registration;

namespace Abp.Web.Mvc.Controllers
{
    /// <summary>
    /// Registers all MVC Controllers derived from <see cref="Controller"/>.
    /// </summary>
    public class ControllerConventionalRegisterer : IConventionalRegisterer
    {
        public void RegisterAssembly(ConventionalRegistrationContext context)
        {
            context.IocContainer.Register(Classes.FromAssembly(context.Assembly)
                                                 .BasedOn<Controller>()
                                                 .LifestyleTransient());
        }
    }
}