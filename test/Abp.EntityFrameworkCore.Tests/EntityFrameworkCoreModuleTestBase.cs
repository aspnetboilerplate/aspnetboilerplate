using System;
using System.Threading.Tasks;
using Abp.Collections;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.Modules;
using Abp.TestBase;

namespace Abp.EntityFrameworkCore.Tests
{
    public abstract class EntityFrameworkCoreModuleTestBase : AbpIntegratedTestBase
    {
        protected EntityFrameworkCoreModuleTestBase()
        {
            CreateInitialData();
        }

        private void CreateInitialData()
        {
            UsingDbContext(
                context =>
                {
                    var blog1 = new Blog() {Name = "test-blog-1", Url = "http://testblog1.myblogs.com"};
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

        public async Task UsingDbContextAsync(Func<BloggingDbContext, Task> action)
        {
            using (var context = LocalIocManager.Resolve<BloggingDbContext>())
            {
                await action(context);
                await context.SaveChangesAsync(true);
            }
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