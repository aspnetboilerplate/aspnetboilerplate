using System.Reflection;
using System.Web.Http;
using Abp.Dependency.Conventions;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Abp.WebApi.Controllers
{
    /// <summary>
    /// Registers all Api controllers derived from <see cref="AbpApiController"/>.
    /// </summary>
    public class ApiControllerConventionalRegisterer : IConventionalRegisterer
    {
        public void RegisterAssembly(IWindsorContainer container, Assembly assembly)
        {
            container.Register(
                Classes.FromAssembly(assembly).BasedOn<ApiController>().LifestyleTransient()
                );
        }
    }
}
