using System.Reflection;
using Abp.Application.Services;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.TestBase;
using Abp.Web.Api.Tests.AppServices;
using Abp.WebApi;

namespace Abp.Web.Api.Tests
{
    [DependsOn(typeof(AbpWebApiModule), typeof(AbpTestBaseModule))]
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

            Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
                .ForAll<IApplicationService>(Assembly.GetExecutingAssembly(), "myapp")
                .ForMethods(builder =>
                {
                    if (builder.Method.IsDefined(typeof(MyIgnoreApiAttribute)))
                    {
                        builder.DontCreate = true;
                    }
                })
                .Build();
        }
    }
}