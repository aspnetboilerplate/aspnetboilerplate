using System.Reflection;
using Abp.AspNetCore;
using Abp.Modules;
using Abp.Resources.Embedded;

namespace AbpAspNetCoreDemo.PlugIn
{
    [DependsOn(typeof(AbpAspNetCoreModule))]
    public class AbpAspNetCoreDemoPlugInModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Views/",
                    Assembly.GetExecutingAssembly(),
                    "AbpAspNetCoreDemo.PlugIn.Views"
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
