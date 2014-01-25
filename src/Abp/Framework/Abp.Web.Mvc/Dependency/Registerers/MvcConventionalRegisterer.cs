using System.Reflection;
using System.Web.Mvc;
using Abp.Dependency.Conventions;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Abp.Web.Mvc.Startup
{
    /// <summary>
    /// 
    /// </summary>
    public class MvcConventionalRegisterer : IConventionalRegisterer
    {
        public void RegisterAssembly(IWindsorContainer container, Assembly assembly)
        {
            container.Register(
                Classes.FromAssembly(assembly).BasedOn<Controller>().LifestyleTransient()
                );
        }
    }
}