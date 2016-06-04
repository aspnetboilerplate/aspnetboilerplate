using System.Reflection;
using Abp.Configuration;
using Abp.Dependency;
using Abp.EntityFramework;
using Abp.GraphDiff.Configuration;
using Abp.GraphDiff.Mapping;
using Abp.Modules;

namespace Abp.GraphDiff
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(AbpKernelModule))]
    public class AbpEntityFrameworkGraphDiffModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpEntityFrameworkGraphDiffModuleConfiguration, AbpEntityFrameworkGraphDiffModuleConfiguration>();
            IocManager.Register<IEntityMappingManager, EntityMappingManager>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
