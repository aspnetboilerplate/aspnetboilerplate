using System.Reflection;
using Abp.EntityFramework;
using Abp.GraphDiff.Mapping;
using Abp.Modules;

namespace Abp.GraphDiff
{
    [DependsOn(typeof(AbpEntityFrameworkModule))]
    public class AbpEntityFrameworkGraphDiffModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Settings.Providers.Add<EntityMappingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
