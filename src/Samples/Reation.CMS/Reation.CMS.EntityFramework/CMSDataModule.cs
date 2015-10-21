using System.Data.Entity;
using System.Reflection;
using Abp.EntityFramework;
using Abp.Modules;
using Reation.CMS.EntityFramework;

namespace Reation.CMS
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(CMSCoreModule))]
    public class CMSDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            Database.SetInitializer<CMSDbContext>(null);
        }
    }
}
