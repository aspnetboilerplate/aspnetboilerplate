using System.Reflection;
using Abp.Dependency;
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
            IocManager.Register<IEntityMappingManager, EntityMappingManager>();
            Configuration.Settings.Providers.Add<EntityMappingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
