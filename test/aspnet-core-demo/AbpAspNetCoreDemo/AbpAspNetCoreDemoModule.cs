using System.Reflection;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Castle.Logging.Log4Net;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using AbpAspNetCoreDemo.Core;
using AbpAspNetCoreDemo.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AbpAspNetCoreDemo
{
    [DependsOn(
        typeof(AbpAspNetCoreModule), 
        typeof(AbpAspNetCoreDemoCoreModule),
        typeof(AbpEntityFrameworkCoreModule),
        typeof(AbpCastleLog4NetModule)
        )]
    public class AbpAspNetCoreDemoModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = IocManager.Resolve<IConfigurationRoot>().GetConnectionString("Default");

            Configuration.Modules.AbpEfCore().AddDbContext<MyDbContext>(options =>
            {
                options.DbContextOptions.UseSqlServer(options.ConnectionString);
            });

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AbpAspNetCoreDemoCoreModule).Assembly
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}