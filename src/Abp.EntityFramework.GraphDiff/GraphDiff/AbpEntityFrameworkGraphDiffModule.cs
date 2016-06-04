using System.Reflection;
using Abp.Configuration;
using Abp.Dependency;
using Abp.EntityFramework;
using Abp.GraphDiff.Mapping;
using Abp.Modules;

namespace Abp.GraphDiff
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(AbpKernelModule))]
    public class AbpEntityFrameworkGraphDiffModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterIfNot<IEntityMappingManager, EntityMappingManager>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
