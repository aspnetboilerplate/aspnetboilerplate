using System;
using System.Linq;
using System.Reflection;
using Abp.Domain.Services;
using Castle.MicroKernel.Registration;

namespace Abp.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConventionalRegisterer
    {
        void Register(Assembly assembly);
    }

    public class DomainServicesRegisterer : IConventionalRegisterer
    {
        public void Register(Assembly assembly)
        {
            if (!assembly.GetTypes().Any(t => typeof(IDomainService).IsAssignableFrom(t)))
            {
                return;
            }

            IocManager.Instance.IocContainer.Register(
                Classes.FromAssembly(assembly)
                    .BasedOn<IDomainService>()
                    .WithService.DefaultInterfaces()
                    .WithService.Self()
                    .LifestyleTransient()
                );
        }
    }
}