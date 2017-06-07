using System;
using System.Collections.Generic;
using System.Reflection;

using Abp.Configuration.Startup;
using Abp.Dapper;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Dapper.Tests.Domain;
using Abp.EntityFrameworkCore.Dapper.Tests.Ef;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;

using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;

using DapperExtensions.Sql;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.EntityFrameworkCore.Dapper.Tests
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreModule),
        typeof(AbpDapperModule),
        typeof(AbpTestBaseModule))]
    public class AbpEfCoreDapperTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            DapperExtensions.DapperExtensions.SqlDialect = new SqliteDialect();

            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = ":memory:",
                Cache = SqliteCacheMode.Shared
            };
            string connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            connection.Open();

            IServiceCollection services = new ServiceCollection()
                .AddEntityFrameworkSqlite();

            IServiceProvider serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
                IocManager.IocContainer,
                services
            );

            //BloggingDbContext
            var blogDbOptionsBuilder = new DbContextOptionsBuilder<BloggingDbContext>();
            blogDbOptionsBuilder.UseSqlite(connection);
            blogDbOptionsBuilder.UseInternalServiceProvider(serviceProvider);

            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<BloggingDbContext>>()
                    .Instance(blogDbOptionsBuilder.Options)
                    .LifestyleSingleton()
            );

            Configuration.ReplaceService<IRepository<Post, Guid>>(() =>
            {
                IocManager.IocContainer.Register(
                    Component.For<IRepository<Post, Guid>, IPostRepository, PostRepository>()
                             .ImplementedBy<PostRepository>()
                             .LifestyleTransient()
                );
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpEfCoreDapperTestModule).GetAssembly());

            DapperExtensions.DapperExtensions.SetMappingAssemblies(new List<Assembly> { Assembly.GetExecutingAssembly() });
        }
    }
}
