using System;
using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Abp.Startup;
using Abp.Web.Api.Tests.DynamicApiController.Clients;
using Abp.WebApi.Controllers.Dynamic.Builders;
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

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            DynamicApiClientBuilder
                .For<IMyAppService>("http://www.aspnetboilerplate.com/api/services/myapp/myservice")
                .Build();
        }
    }
}