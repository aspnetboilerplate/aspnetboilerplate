using System;
using System.Transactions;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityFrameworkCore.Tests.Ef;
using Abp.Modules;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Microsoft.EntityFrameworkCore;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Reflection.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;

namespace Abp.EntityFrameworkCore.Tests
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule), typeof(AbpTestBaseModule))]
    public class EntityFrameworkCoreTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsolationLevel = IsolationLevel.Unspecified;

            //BloggingDbContext
            RegisterBloggingDbContextToSqliteInMemoryDb(IocManager);

            //SupportDbContext
            RegisterSupportDbContextToSqliteInMemoryDb(IocManager);
            
            //Custom repository
            Configuration.ReplaceService<IRepository<Post, Guid>>(() =>
            {
                IocManager.IocContainer.Register(
                    Component.For<IRepository<Post, Guid>, IPostRepository, PostRepository>()
                        .ImplementedBy<PostRepository>()
                        .LifestyleTransient()
                );
            });

            Configuration.IocManager.Register<IRepository<TicketListItem>, TicketListItemRepository>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EntityFrameworkCoreTestModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            using (var context = IocManager.Resolve<BloggingDbContext>())
            {
                context.Database.ExecuteSqlCommand("CREATE VIEW BlogView AS SELECT Id, Name, Url FROM Blogs");
            }
        }

        private static void RegisterBloggingDbContextToSqliteInMemoryDb(IIocManager iocManager)
        {
            var builder = new DbContextOptionsBuilder<BloggingDbContext>();

            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            builder.UseSqlite(inMemorySqlite);

            iocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<BloggingDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );

            inMemorySqlite.Open();
            new BloggingDbContext(builder.Options).Database.EnsureCreated();
        }

        private static void RegisterSupportDbContextToSqliteInMemoryDb(IIocManager iocManager)
        {
            var builder = new DbContextOptionsBuilder<SupportDbContext>();

            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            builder.UseSqlite(inMemorySqlite);

            iocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<SupportDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );

            inMemorySqlite.Open();
            var ctx = new SupportDbContext(builder.Options);
            ctx.Database.EnsureCreated();

            using (var command = ctx.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = SupportDbContext.TicketViewSql;
                ctx.Database.OpenConnection();

                command.ExecuteNonQuery();
            }
        }
    }
}