using System.Reflection;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Modules;
using Abp.AspNetCore.OData.Configuration;
using Microsoft.AspNet.OData;

namespace Abp.AspNetCore.OData
{
    [DependsOn(typeof(AbpAspNetCoreModule))]
    public class AbpAspNetCoreODataModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpAspNetCoreODataModuleConfiguration, AbpAspNetCoreODataModuleConfiguration>();

            Configuration.Validation.IgnoredTypes.AddIfNotContains(typeof(Delta));
        }

        public override void Initialize()
        {
            IocManager.Register<MetadataController>(DependencyLifeStyle.Transient);
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
