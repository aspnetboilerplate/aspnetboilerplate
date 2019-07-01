using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.OData;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpAspNetCoreDemo.Core;
using AbpAspNetCoreDemo.Db;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AbpAspNetCoreDemo
{
    [DependsOn(
        typeof(AbpAspNetCoreModule),
        typeof(AbpAspNetCoreDemoCoreModule),
        typeof(AbpEntityFrameworkCoreModule),
        typeof(AbpCastleLog4NetModule),
        typeof(AbpAspNetCoreODataModule)
        )]
    public class AbpAspNetCoreDemoModule : AbpModule
    {
        public override void PreInitialize()
        {
            RegisterDbContextToSqliteInMemoryDb(IocManager);

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AbpAspNetCoreDemoCoreModule).GetAssembly()
                );


            Configuration.IocManager.Resolve<IAbpAspNetCoreConfiguration>().RouteConfiguration.Add(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpAspNetCoreDemoModule).GetAssembly());
        }

        private static void RegisterDbContextToSqliteInMemoryDb(IIocManager iocManager)
        {
            var builder = new DbContextOptionsBuilder<MyDbContext>();

            builder.ReplaceService<IEntityMaterializerSource, AbpEntityMaterializerSource>();

            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            builder.UseSqlite(inMemorySqlite);

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
}