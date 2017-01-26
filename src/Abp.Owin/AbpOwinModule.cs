using System.Reflection;
using Abp.Modules;
using Abp.Web;

namespace Abp.Owin
{
    /// <summary>
    /// OWIN integration module for ABP.
    /// </summary>
    [DependsOn(typeof (AbpWebCommonModule))]
    public class AbpOwinModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
