using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.EntityFrameworkCore.Extensions;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.Runtime.Session;
using Abp.TestBase;
using Abp.Zero.TestData;
using Abp.ZeroCore.SampleApp.Core;
using Abp.ZeroCore.SampleApp.Core.EntityHistory;
using Abp.ZeroCore.SampleApp.Core.Shop;
using Abp.ZeroCore.SampleApp.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Abp.Zero
{
    public abstract class AbpZeroTestBase : AbpIntegratedTestBase<AbpZeroTestModule>
    {
        protected AbpZeroTestBase()
        {
            SeedTestData();
            SeedTestDataForEntityHistory();
            SeedTestDataForMultiLingualEntities();
            LoginAsDefaultTenantAdmin();
        }

        private void SeedTestData()
        {
            void NormalizeDbContext(SampleAppDbContext context)
            {
                context.EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
                context.EventBus = NullEventBus.Instance;
                context.SuppressAutoSetTenantId = true;
            }

            //Seed initial data for default tenant
            AbpSession.TenantId = 1;
            UsingDbContext(context =>
            {
                NormalizeDbContext(context);
                new TestDataBuilder(context, 1).Create();
            });
        }

        private void SeedTestDataForEntityHistory()
        {
            UsingDbContext(
                context =>
                {
                    var blog1 = new Blog("test-blog-1", "http://testblog1.myblogs.com", "blogger-1");

                    context.Blogs.Add(blog1);
                    context.SaveChanges();

                    var post1 = new Post { Blog = blog1, Title = "test-post-1-title", Body = "test-post-1-body" };
                    var post2 = new Post { Blog = blog1, Title = "test-post-2-title", Body = "test-post-2-body" };
                    var post3 = new Post { Blog = blog1, Title = "test-post-3-title", Body = "test-post-3-body-deleted", IsDeleted = true };
                    var post4 = new Post { Blog = blog1, Title = "test-post-4-title", Body = "test-post-4-body", TenantId = 42 };

                    context.Posts.AddRange(post1, post2, post3, post4);

                    var comment1 = new Comment { Post = post1, Content = "test-comment-1-content" };

                    context.Comments.Add(comment1);

                    var advertisement1 = new Advertisement { Banner = "test-advertisement-1" };

                    context.Advertisements.Add(advertisement1);
                });
        }

        private void SeedTestDataForMultiLingualEntities()
        {
            UsingDbContext(
                context =>
                {
                    var product1 = new Product
                    {
                        Price = 10,
                        Stock = 1000
                    };

                    var product2 = new Product
                    {
                        Price = 99,
                        Stock = 1000
                    };

                    var product3 = new Product
                    {
                        Price = 15,
                        Stock = 500
                    };

                    context.Products.Add(product1);
                    context.Products.Add(product2);
                    context.Products.Add(product3);
                    context.SaveChanges();

                    //Product1 translations (Watch)
                    var product1_en = new ProductTranslation { CoreId = product1.Id, Language = "en", Name = "Watch" };
                    var product1_tr = new ProductTranslation { CoreId = product1.Id, Language = "tr", Name = "Saat" };

                    context.ProductTranslations.Add(product1_en);
                    context.ProductTranslations.Add(product1_tr);

                    //Product2 translations (Bike)
                    var product2_en = new ProductTranslation { CoreId = product2.Id, Language = "en", Name = "Bike" };
                    var product2_fr = new ProductTranslation { CoreId = product2.Id, Language = "fr", Name = "Bicyclette" };

                    context.ProductTranslations.Add(product2_en);
                    context.ProductTranslations.Add(product2_fr);

                    //Product3 translations (Newspaper)
                    var product3_it = new ProductTranslation { CoreId = product3.Id, Language = "it", Name = "Giornale" };

                    context.ProductTranslations.Add(product3_it);

                    context.SaveChanges();
                });

            UsingDbContext(context =>
            {
                var products = context.Products.ToList();
                var order = new Order
                {
                    Price = 100,
                    Products = products
                };

                context.Orders.Add(order);
                context.SaveChanges();

                var order_en = new OrderTranslation { CoreId = order.Id, Language = "en", Name = "Test" };
                var order_fr = new OrderTranslation { CoreId = order.Id, Language = "fr", Name = "Tester" };

                context.OrderTranslations.Add(order_en);
                context.OrderTranslations.Add(order_fr);
                context.SaveChanges();
            });
        }

        protected IDisposable UsingTenantId(int? tenantId)
        {
            var previousTenantId = AbpSession.TenantId;
            AbpSession.TenantId = tenantId;
            return new DisposeAction(() => AbpSession.TenantId = previousTenantId);
        }

        protected void UsingDbContext(Action<SampleAppDbContext> action)
        {
            UsingDbContext(AbpSession.TenantId, action);
        }

        protected Task UsingDbContextAsync(Func<SampleAppDbContext, Task> action)
        {
            return UsingDbContextAsync(AbpSession.TenantId, action);
        }

        protected T UsingDbContext<T>(Func<SampleAppDbContext, T> func)
        {
            return UsingDbContext(AbpSession.TenantId, func);
        }

        protected Task<T> UsingDbContextAsync<T>(Func<SampleAppDbContext, Task<T>> func)
        {
            return UsingDbContextAsync(AbpSession.TenantId, func);
        }

        protected void UsingDbContext(int? tenantId, Action<SampleAppDbContext> action)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<SampleAppDbContext>())
                {
                    action(context);
                    context.SaveChanges();
                }
            }
        }

        protected async Task UsingDbContextAsync(int? tenantId, Func<SampleAppDbContext, Task> action)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<SampleAppDbContext>())
                {
                    await action(context);
                    await context.SaveChangesAsync();
                }
            }
        }

        protected T UsingDbContext<T>(int? tenantId, Func<SampleAppDbContext, T> func)
        {
            T result;

            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<SampleAppDbContext>())
                {
                    result = func(context);
                    context.SaveChanges();
                }
            }

            return result;
        }

        protected async Task<T> UsingDbContextAsync<T>(int? tenantId, Func<SampleAppDbContext, Task<T>> func)
        {
            T result;

            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<SampleAppDbContext>())
                {
                    result = await func(context);
                    await context.SaveChangesAsync();
                }
            }

            return result;
        }

        #region Login

        protected void LoginAsHostAdmin()
        {
            LoginAsHost(AbpUserBase.AdminUserName);
        }

        protected void LoginAsDefaultTenantAdmin()
        {
            LoginAsTenant(Tenant.DefaultTenantName, AbpUserBase.AdminUserName);
        }

        protected void LoginAsHost(string userName)
        {
            AbpSession.TenantId = null;

            var user = UsingDbContext(context => context.Users.FirstOrDefault(u => u.TenantId == AbpSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for host.");
            }

            AbpSession.UserId = user.Id;
        }

        protected void LoginAsTenant(string tenancyName, string userName)
        {
            AbpSession.TenantId = null;

            var tenant = UsingDbContext(context => context.Tenants.FirstOrDefault(t => t.TenancyName == tenancyName));
            if (tenant == null)
            {
                throw new Exception("There is no tenant: " + tenancyName);
            }

            AbpSession.TenantId = tenant.Id;

            var user = UsingDbContext(context => context.Users.FirstOrDefault(u => u.TenantId == AbpSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for tenant: " + tenancyName);
            }

            AbpSession.UserId = user.Id;
        }

        #endregion

        #region GetCurrentUser

        /// <summary>
        /// Gets current user if <see cref="IAbpSession.UserId"/> is not null.
        /// Throws exception if it's null.
        /// </summary>
        protected User GetCurrentUser()
        {
            var userId = AbpSession.GetUserId();
            return UsingDbContext(context => context.Users.Single(u => u.Id == userId));
        }

        /// <summary>
        /// Gets current user if <see cref="IAbpSession.UserId"/> is not null.
        /// Throws exception if it's null.
        /// </summary>
        protected async Task<User> GetCurrentUserAsync()
        {
            var userId = AbpSession.GetUserId();
            return await UsingDbContext(context => context.Users.SingleAsync(u => u.Id == userId));
        }

        #endregion

        #region GetCurrentTenant

        /// <summary>
        /// Gets current tenant if <see cref="IAbpSession.TenantId"/> is not null.
        /// Throws exception if there is no current tenant.
        /// </summary>
        protected Tenant GetCurrentTenant()
        {
            var tenantId = AbpSession.GetTenantId();
            return UsingDbContext(null, context => context.Tenants.Single(t => t.Id == tenantId));
        }

        /// <summary>
        /// Gets current tenant if <see cref="IAbpSession.TenantId"/> is not null.
        /// Throws exception if there is no current tenant.
        /// </summary>
        protected async Task<Tenant> GetCurrentTenantAsync()
        {
            var tenantId = AbpSession.GetTenantId();
            return await UsingDbContext(null, context => context.Tenants.SingleAsync(t => t.Id == tenantId));
        }

        #endregion

        #region GetTenant / GetTenantOrNull

        protected Tenant GetTenant(string tenancyName)
        {
            return UsingDbContext(null, context => context.Tenants.Single(t => t.TenancyName == tenancyName));
        }

        protected async Task<Tenant> GetTenantAsync(string tenancyName)
        {
            return await UsingDbContext(null, async context => await context.Tenants.SingleAsync(t => t.TenancyName == tenancyName));
        }

        protected Tenant GetTenantOrNull(string tenancyName)
        {
            return UsingDbContext(null, context => context.Tenants.FirstOrDefault(t => t.TenancyName == tenancyName));
        }

        protected async Task<Tenant> GetTenantOrNullAsync(string tenancyName)
        {
            return await UsingDbContext(null, async context => await context.Tenants.FirstOrDefaultAsync(t => t.TenancyName == tenancyName));
        }

        #endregion

        #region GetRole

        protected Role GetRole(string roleName)
        {
            return UsingDbContext(context => context.Roles.Single(r => r.Name == roleName && r.TenantId == AbpSession.TenantId));
        }

        protected async Task<Role> GetRoleAsync(string roleName)
        {
            return await UsingDbContext(async context => await context.Roles.SingleAsync(r => r.Name == roleName && r.TenantId == AbpSession.TenantId));
        }

        #endregion

        #region GetUserByUserName

        protected User GetUserByUserName(string userName)
        {
            var user = GetUserByUserNameOrNull(userName);
            if (user == null)
            {
                throw new Exception("Can not find a user with username: " + userName);
            }

            return user;
        }

        protected async Task<User> GetUserByUserNameAsync(string userName)
        {
            var user = await GetUserByUserNameOrNullAsync(userName);
            if (user == null)
            {
                throw new Exception("Can not find a user with username: " + userName);
            }

            return user;
        }

        protected User GetUserByUserNameOrNull(string userName)
        {
            return UsingDbContext(context =>
                context.Users.FirstOrDefault(u =>
                    u.UserName == userName &&
                    u.TenantId == AbpSession.TenantId
                ));
        }

        protected async Task<User> GetUserByUserNameOrNullAsync(string userName, bool includeRoles = false)
        {
            return await UsingDbContextAsync(async context =>
                await context.Users
                    .IncludeIf(includeRoles, u => u.Roles)
                    .FirstOrDefaultAsync(u =>
                        u.UserName == userName &&
                        u.TenantId == AbpSession.TenantId
                    ));
        }

        #endregion
    }
}
