using System.Reflection;
using Abp.Modules;

namespace Abp.Events.Producer
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpEventsProducerModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
