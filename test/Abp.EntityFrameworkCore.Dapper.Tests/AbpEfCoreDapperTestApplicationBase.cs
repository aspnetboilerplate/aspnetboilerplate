using System;
using System.Threading.Tasks;

using Abp.EntityFrameworkCore.Dapper.Tests.Domain;
using Abp.EntityFrameworkCore.Dapper.Tests.Ef;
using Abp.TestBase;
using Abp.Timing;

using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Dapper.Tests
{
    public class AbpEfCoreDapperTestApplicationBase : AbpIntegratedTestBase<AbpEfCoreDapperTestModule>
    {
        protected AbpEfCoreDapperTestApplicationBase()
        {
            Clock.Provider = ClockProviders.Utc;

            CreateInitialData();
        }

        private void CreateInitialData()
        {
            using (var bloggingDbContext = LocalIocManager.Resolve<BloggingDbContext>())
            {
                bloggingDbContext.Database.OpenConnection();
                bloggingDbContext.Database.EnsureDeleted();
                bloggingDbContext.Database.EnsureCreated();
                bloggingDbContext.Database.Migrate();
            }

            UsingDbContext(
                context =>
                {
                    var blog1 = new Blog("test-blog-1", "http://testblog1.myblogs.com");

                    context.Blogs.Add(blog1);

                    var post1 = new Post { Blog = blog1, Title = "test-post-1-title", Body = "test-post-1-body" };
                    var post2 = new Post { Blog = blog1, Title = "test-post-2-title", Body = "test-post-2-body" };

                    context.Posts.AddRange(post1, post2);
                });
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
            await using (var context = LocalIocManager.Resolve<BloggingDbContext>())
            {
                await action(context);
                await context.SaveChangesAsync(true);
            }
        }

        public async Task<T> UsingDbContextAsync<T>(Func<BloggingDbContext, Task<T>> func)
        {
            T result;

            await using (var context = LocalIocManager.Resolve<BloggingDbContext>())
            {
                result = await func(context);
                await context.SaveChangesAsync();
            }

            return result;
        }
    }
}
