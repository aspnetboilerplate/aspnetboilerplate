using System.Reflection;
using System.Web.OData;
using System.Web.OData.Extensions;
using Adorable.Configuration.Startup;
using Adorable.Dependency;
using Adorable.Modules;
using Adorable.WebApi.OData.Configuration;

namespace Adorable.WebApi.OData
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
