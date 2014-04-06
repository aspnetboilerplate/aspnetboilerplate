using System.Web.Http;
using Abp.Dependency.Conventions;
using Castle.MicroKernel.Registration;

namespace Abp.WebApi.Controllers
{
    /// <summary>
    /// Registers all Web API Controllers derived from <see cref="ApiController"/>.
    /// </summary>
    public class ApiControllerConventionalRegisterer : IConventionalRegisterer
    {
        public void RegisterAssembly(ConventionalRegistrationContext context)
        {
            context.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .BasedOn<ApiController>()
                    .LifestyleTransient()
                );
        }
    }
}
