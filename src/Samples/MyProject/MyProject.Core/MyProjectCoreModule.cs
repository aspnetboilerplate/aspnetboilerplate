using System.Reflection;
using Abp.Modules;

namespace MyProject
{
    public class MyProjectCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
