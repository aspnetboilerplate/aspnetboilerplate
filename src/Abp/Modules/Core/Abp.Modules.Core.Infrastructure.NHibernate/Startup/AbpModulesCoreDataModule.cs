using System.Reflection;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Data.Repositories.Interceptors;
using Abp.Security.Tenants;
using Abp.Startup;
using Abp.Startup.Infrastructure.NHibernate;
using Castle.Core;
using Castle.MicroKernel;
using Castle.Windsor;

namespace Abp.Modules.Core.Startup
{
    [AbpModule("Abp.Modules.Core.Infrastructure.NHibernate", Dependencies = new[] { "Abp.Infrastructure.NHibernate" })]
    public class AbpModulesCoreDataModule : AbpModule
    {
        private IWindsorContainer IocContainer { get; set; }

        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            IocContainer = initializationContext.IocContainer;

            initializationContext.IocContainer.Kernel.ComponentRegistered += ComponentRegistered;

            initializationContext.GetModule<AbpNHibernateModule>().Configuration
                .Mappings(
                    m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly())
                );
        }

        private void ComponentRegistered(string key, IHandler handler)
        {
            if (typeof(IRepository).IsAssignableFrom(handler.ComponentModel.Implementation))
            {
                foreach (var implementedInterface in handler.ComponentModel.Implementation.GetInterfaces())
                {
                    if (implementedInterface.Name == "IRepository`2" && implementedInterface.IsGenericType && implementedInterface.GenericTypeArguments.Length == 2)
                    {
                        var typeArgs = implementedInterface.GenericTypeArguments;
                        if ((typeof(IHasTenant)).IsAssignableFrom(typeArgs[0]))
                        {
                            var genType = (typeof(MultiTenancyInterceptor<,>)).MakeGenericType(typeArgs[0], typeArgs[1]);
                            handler.ComponentModel.Interceptors.Add(new InterceptorReference(genType));
                        }
                        if (typeof(ICreationAudited).IsAssignableFrom(typeArgs[0]) || typeof(IModificationAudited).IsAssignableFrom(typeArgs[0]))
                        {
                            handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AuditInterceptor)));
                        }
                    }
                }
            }
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
