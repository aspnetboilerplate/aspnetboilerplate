using Abp.Domain.Repositories;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Data.Repositories
{
    public class RepositoryInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        { 
            container.Register(
                Classes.FromThisAssembly().BasedOn<IRepository>().WithServiceDefaultInterfaces().LifestyleTransient().WithServiceSelf().LifestyleTransient()
                );
        }
    }
}
