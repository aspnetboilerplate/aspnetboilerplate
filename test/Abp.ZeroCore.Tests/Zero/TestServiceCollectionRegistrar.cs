using Abp.Dependency;
using Abp.ZeroCore.SampleApp.Core;
using Abp.ZeroCore.SampleApp.EntityFramework;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.Zero
{
    public static class TestServiceCollectionRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            RegisterServiceCollectionServices(iocManager);
            RegisterSqliteInMemoryDb(iocManager);
        }

        private static void RegisterServiceCollectionServices(IIocManager iocManager)
        {
            var services = new ServiceCollection();
            ServicesCollectionDependencyRegistrar.Register(services);
            WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);
        }

        private static void RegisterSqliteInMemoryDb(IIocManager iocManager)
        {
            var builder = new DbContextOptionsBuilder<SampleAppDbContext>();

            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            builder.UseSqlite(inMemorySqlite);

            iocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<SampleAppDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );

            inMemorySqlite.Open();
            new SampleAppDbContext(builder.Options).Database.EnsureCreated();
        }
    }
}
