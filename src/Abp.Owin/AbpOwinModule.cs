using System.Reflection;
using Abp.Modules;

namespace Abp.Owin
{
    /// <summary>
    /// OWIN integration module for ABP.
    /// </summary>
    [DependsOn(typeof (AbpKernelModule))]
    public class AbpOwinModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
