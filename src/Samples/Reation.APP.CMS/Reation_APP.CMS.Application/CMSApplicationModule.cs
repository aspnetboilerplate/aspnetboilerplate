using System.Reflection;
using Abp.Modules;

namespace Reation_APP.CMS
{
    [DependsOn(typeof(CMSCoreModule))]
    public class CMSApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
