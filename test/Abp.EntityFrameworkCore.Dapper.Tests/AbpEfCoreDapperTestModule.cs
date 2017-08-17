using System;
using System.Collections.Generic;
using System.Reflection;
using System.Transactions;
using Abp.Configuration.Startup;
using Abp.Dapper;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Dapper.Tests.Domain;
using Abp.EntityFrameworkCore.Dapper.Tests.Ef;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using DapperExtensions.Sql;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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
            Configuration.UnitOfWork.IsolationLevel = IsolationLevel.Unspecified;

            DapperExtensions.DapperExtensions.SqlDialect = new SqliteDialect();
            
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
            var builder = new DbContextOptionsBuilder<BloggingDbContext>();

            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            builder.UseSqlite(inMemorySqlite);

            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<BloggingDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );

            inMemorySqlite.Open();
            new BloggingDbContext(builder.Options).Database.EnsureCreated();

            IocManager.RegisterAssemblyByConvention(typeof(AbpEfCoreDapperTestModule).GetAssembly());

            DapperExtensions.DapperExtensions.SetMappingAssemblies(new List<Assembly> { typeof(AbpEfCoreDapperTestModule).GetAssembly() });
        }
    }
}
