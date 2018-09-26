using System.Reflection;
using Abp.Modules;
using Castle.MicroKernel.Registration;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace Abp.Web.SignalR
{
    /// <summary>
    /// ABP SignalR integration module.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpWebSignalRModule : AbpModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            GlobalHost.DependencyResolver = new WindsorDependencyResolver(IocManager.IocContainer);
            UseAbpSignalRContractResolver();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        private void UseAbpSignalRContractResolver()
        {
            var serializer = JsonSerializer.Create(
                new JsonSerializerSettings
                {
                    ContractResolver = new AbpSignalRContractResolver()
                });

            IocManager.IocContainer.Register(
                Component.For<JsonSerializer>().Instance(serializer)
            );
        }
    }
}
