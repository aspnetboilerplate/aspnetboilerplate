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
            var services = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
                IocManager.IocContainer,
                services
            );

            var builder = new DbContextOptionsBuilder<BloggingDbContext>();
            builder.UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider);

            var options = builder.Options;

            IocManager.IocContainer.Register(
                Component.For<DbContextOptions<BloggingDbContext>>().Instance(options).LifestyleSingleton()
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}