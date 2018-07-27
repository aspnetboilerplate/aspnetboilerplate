using Abp.Dependency;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.Reflection.Extensions;

namespace Abp.MailKit
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpMailKitModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpMailKitConfiguration, AbpMailKitConfiguration>();
            Configuration.ReplaceService<IEmailSender, MailKitEmailSender>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpMailKitModule).GetAssembly());
        }
    }
}
