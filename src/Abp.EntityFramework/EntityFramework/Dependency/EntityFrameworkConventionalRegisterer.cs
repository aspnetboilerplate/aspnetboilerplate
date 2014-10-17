using Abp.Configuration.Startup;
using Abp.Dependency;
using Castle.MicroKernel.Registration;

namespace Abp.EntityFramework.Dependency
{
    public class EntityFrameworkConventionalRegisterer : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<AbpDbContext>()
                    .WithServiceSelf()
                    .LifestyleTransient()
                    .Configure(c => c.DynamicParameters(
                        (kernel, dynamicParams) =>
                        {
                            if (!kernel.HasComponent(typeof (IAbpStartupConfiguration)))
                            {
                                return;
                            }

                            var defaultConnectionString = kernel.Resolve<IAbpStartupConfiguration>().DefaultNameOrConnectionString;
                            if (string.IsNullOrWhiteSpace(defaultConnectionString))
                            {
                                return;
                            }

                            dynamicParams["nameOrConnectionString"] = defaultConnectionString;
                        })));
        }
    }
}