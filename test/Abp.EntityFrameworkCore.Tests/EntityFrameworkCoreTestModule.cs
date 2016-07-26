using System.Reflection;
using Abp.Modules;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.EntityFrameworkCore.Tests
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule), typeof(AbpTestBaseModule))]
    public class EntityFrameworkCoreTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions

            var services = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
                IocManager.IocContainer,
                services
            );

            var builder = new DbContextOptionsBuilder<BloggingDbContext>();
            builder.UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider);

            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<BloggingDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}