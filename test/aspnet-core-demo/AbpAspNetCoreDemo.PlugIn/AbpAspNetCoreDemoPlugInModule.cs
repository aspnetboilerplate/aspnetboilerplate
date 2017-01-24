using System.Reflection;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Views;
using Abp.Modules;

namespace AbpAspNetCoreDemo.PlugIn
{
    [DependsOn(typeof(AbpAspNetCoreModule))]
    public class AbpAspNetCoreDemoPlugInModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAspNetCore().EmbeddedViews.Sources.Add(
                new EmbeddedViewInfo(Assembly.GetExecutingAssembly(), "AbpAspNetCoreDemo.PlugIn.Views")
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
