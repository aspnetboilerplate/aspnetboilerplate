using System.Reflection;
using Adorable.Modules;
using Adorable.Web.Api.Tests.DynamicApiController.Clients;
using Adorable.WebApi;
using Adorable.WebApi.Controllers.Dynamic.Clients;

namespace Adorable.Web.Api.Tests
{
    [DependsOn(typeof(AbpWebApiModule))]
    public class AbpWebApiTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            base.PreInitialize();
            Configuration.Localization.IsEnabled = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            
            DynamicApiClientBuilder
                .For<IMyAppService>("http://www.aspnetboilerplate.com/api/services/myapp/myservice")
                .Build();
        }
    }
}