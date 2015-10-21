using System.Reflection;
using Abp.Modules;

namespace Reation.CMS
{
    public class CMSCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
