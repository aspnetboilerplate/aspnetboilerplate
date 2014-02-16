using System.Reflection;
using System.Web.Mvc;
using Abp.Dependency.Conventions;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Abp.Web.Mvc.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ControllerConventionalRegisterer : IConventionalRegisterer
    {
        public void RegisterAssembly(IWindsorContainer container, Assembly assembly)
        {
            container.Register(
                Classes.FromAssembly(assembly).BasedOn<Controller>().LifestyleTransient()
                );
        }
    }
}