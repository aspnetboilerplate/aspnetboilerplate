using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.WebApi.OData.Configuration;
using System.Reflection;
using System.Web.OData;
using System.Web.OData.Extensions;

namespace Abp.WebApi.OData
{
    [DependsOn(typeof(AbpWebApiModule))]
    public class AbpWebApiODataModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpWebApiODataModuleConfiguration, AbpWebApiODataModuleConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.Register<MetadataController>(DependencyLifeStyle.Transient);
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.AbpWebApi().HttpConfiguration.MapODataServiceRoute(
                    routeName: "ODataRoute",
                    routePrefix: "odata",
                    model: Configuration.Modules.AbpWebApiOData().ODataModelBuilder.GetEdmModel()
                );
        }
    }
}