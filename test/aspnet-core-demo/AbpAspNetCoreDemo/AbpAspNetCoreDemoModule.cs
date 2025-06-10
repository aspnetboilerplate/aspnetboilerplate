using System;
using System.Threading;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.OData;
using Abp.AspNetCore.OData.ResultWrapping;
using Abp.Castle.Logging.Log4Net;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Extensions;
using Abp.HtmlSanitizer;
using Abp.HtmlSanitizer.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpAspNetCoreDemo.Core;
using AbpAspNetCoreDemo.Core.Application.Account;
using AbpAspNetCoreDemo.Db;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AbpAspNetCoreDemo;

[DependsOn(
    typeof(AbpAspNetCoreModule),
    typeof(AbpAspNetCoreDemoCoreModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpCastleLog4NetModule),
    typeof(AbpAspNetCoreODataModule),
    typeof(AbpHtmlSanitizerModule)
)]
public class AbpAspNetCoreDemoModule : AbpModule
{
    public static AsyncLocal<Action<IAbpStartupConfiguration>> ConfigurationAction =
        new AsyncLocal<Action<IAbpStartupConfiguration>>();

    public override void PreInitialize()
    {
        RegisterDbContextToSqliteInMemoryDb(IocManager);

        Configuration.Modules.AbpAspNetCore()
            .CreateControllersForAppServices(
                typeof(AbpAspNetCoreDemoCoreModule).GetAssembly()
            );

        Configuration.Modules.AbpWebCommon().WrapResultFilters.Add(new AbpODataDontWrapResultFilter());

        Configuration.IocManager.Resolve<IAbpAspNetCoreConfiguration>().EndpointConfiguration.Add(endpoints =>
        {
            endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
        });

        Configuration.Caching.MemoryCacheOptions = new MemoryCacheOptions
        {
            SizeLimit = 2048
        };

        ConfigurationAction.Value?.Invoke(Configuration);

        Configuration.Modules.AbpHtmlSanitizer()
            .AddSelector<IAccountAppService>(x => nameof(x.Register));
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(AbpAspNetCoreDemoModule).GetAssembly());
    }

    private static void RegisterDbContextToSqliteInMemoryDb(IIocManager iocManager)
    {
        var builder = new DbContextOptionsBuilder<MyDbContext>();

        var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
        builder.UseSqlite(inMemorySqlite).AddAbpDbContextOptionsExtension();

        iocManager.IocContainer.Register(
            Component
                .For<DbContextOptions<MyDbContext>>()
                .Instance(builder.Options)
                .LifestyleSingleton()
        );

        inMemorySqlite.Open();
        var ctx = new MyDbContext(builder.Options);
        ctx.Database.EnsureCreated();
    }
}