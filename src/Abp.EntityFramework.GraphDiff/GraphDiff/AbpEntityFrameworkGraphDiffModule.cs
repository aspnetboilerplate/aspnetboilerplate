using System.Collections.Generic;
using System.Reflection;
using Abp.EntityFramework.GraphDiff.Configuration;
using Abp.EntityFramework.GraphDiff.Mapping;
using Abp.Modules;

namespace Abp.EntityFramework.GraphDiff
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(AbpKernelModule))]
    public class AbpEntityFrameworkGraphDiffModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpEntityFrameworkGraphDiffModuleConfiguration, AbpEntityFrameworkGraphDiffModuleConfiguration>();

            Configuration.Modules
                .AbpEfGraphDiff()
                .UseMappings(new List<EntityMapping>());
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
