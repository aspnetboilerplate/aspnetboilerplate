using System.Reflection;
using Abp.Modules;

namespace Reation_APP.CMS
{
    public class CMSCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
