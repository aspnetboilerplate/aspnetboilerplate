using Abp.Modules;
using System.Reflection;

namespace Abp
{
    [DependsOn(typeof(AbpKernelModule))]
    public class Log4NetModule:AbpModule
    {
        public override void PreInitialize()
        {

        }

        public override void Initialize()
        {
            this.IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}