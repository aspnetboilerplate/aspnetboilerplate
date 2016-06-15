using System.Reflection;
using Abp.AspNetCore;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using AbpAspNetCoreDemo.Core;

namespace AbpAspNetCoreDemo
{
    [DependsOn(
        typeof(AbpAspNetCoreModule), 
        typeof(AbpAspNetCoreDemoCoreModule), 
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class AbpAspNetCoreDemoModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}