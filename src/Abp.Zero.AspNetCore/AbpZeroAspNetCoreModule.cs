using System.Reflection;
using Abp.Authorization.Users;
using Abp.Configuration.Startup;
using Abp.Modules;

namespace Abp.Zero.AspNetCore
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class AbpZeroAspNetCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpZeroAspNetCoreConfiguration, AbpZeroAspNetCoreConfiguration>();
            Configuration.ReplaceService<IUserTokenProviderAccessor, AspNetCoreUserTokenProviderAccessor>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}