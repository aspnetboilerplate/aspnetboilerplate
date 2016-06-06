using System.Collections.Generic;
using System.Reflection;
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
