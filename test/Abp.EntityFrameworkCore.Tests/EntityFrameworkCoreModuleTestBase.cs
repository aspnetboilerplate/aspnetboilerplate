using System;
using System.Threading.Tasks;
using Abp.Collections;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.Modules;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.EntityFrameworkCore.Tests
{
    public abstract class EntityFrameworkCoreModuleTestBase : AbpIntegratedTestBase
    {
        protected EntityFrameworkCoreModuleTestBase()
        {
            ////Fake DbConnection using Effort!
            //LocalIocManager.IocContainer.Register(
            //    Component.For<DbConnection>()
            //        .UsingFactoryMethod(Effort.DbConnectionFactory.CreateTransient)
            //        .LifestyleSingleton()
            //    );

            var services = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(LocalIocManager.IocContainer, services);

            var builder = new DbContextOptionsBuilder<BloggingDbContext>();
            builder.UseInMemoryDatabase()
                   .UseInternalServiceProvider(serviceProvider);

            var options = builder.Options;

            LocalIocManager.IocContainer.Register(
                Component.For<DbContextOptions<BloggingDbContext>>().Instance(options).LifestyleSingleton()
            );

            CreateInitialData();
        }

        private void CreateInitialData()
        {
            UsingDbContext(
                context =>
                {
                    var blog1 = new Blog() {Name = "Test blog 1", Url = "http://testblog1.myblogs.com"};
                    context.Blogs.Add(blog1);
                });
        }

        
        protected override void AddModules(ITypeList<AbpModule> modules)
        {
            base.AddModules(modules);
            modules.Add<EntityFrameworkCoreTestModule>();
        }

        public void UsingDbContext(Action<BloggingDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<BloggingDbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        public T UsingDbContext<T>(Func<BloggingDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<BloggingDbContext>())
            {
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        public async Task<T> UsingDbContextAsync<T>(Func<BloggingDbContext, Task<T>> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<BloggingDbContext>())
            {
                result = await func(context);
                context.SaveChanges();
            }

            return result;
        }
    }
}