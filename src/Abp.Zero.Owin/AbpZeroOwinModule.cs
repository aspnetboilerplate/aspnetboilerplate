using System.Reflection;
using Abp.Authorization.Users;
using Abp.Modules;
using Abp.Zero;
using Abp.Configuration.Startup;
using Abp.Owin;

namespace Abp
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class AbpZeroOwinModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.ReplaceService<IUserTokenProviderAccessor, OwinUserTokenProviderAccessor>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
