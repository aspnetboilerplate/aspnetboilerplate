using System;
using System.Reflection;
using Abp.Modules;
using Abp.Web.Api.Tests.DynamicApiController.Clients;
using Abp.WebApi.Controllers.Dynamic.Clients;
using Abp.WebApi.Startup;

namespace Abp.Web.Api.Tests
{
    public class AbpWebApiTestModule : AbpModule
    {
        public override Type[] GetDependedModules()
        {
            return new[]
                   {
                       typeof (AbpWebApiModule)
                   };
        }

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