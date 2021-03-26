using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.IdentityFramework;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.TestBase;
using Abp.Zero.SampleApp.EntityFramework;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Roles;
using Abp.Zero.SampleApp.Tests.TestDatas;
using Abp.Zero.SampleApp.Users;
using Castle.MicroKernel.Registration;
using EntityFramework.DynamicFilters;
using Microsoft.AspNet.Identity;
using Shouldly;

namespace Abp.Zero.SampleApp.Tests
{
    public abstract class SampleAppTestBase : SampleAppTestBase<SampleAppTestModule>
    {

    }

    public abstract class SampleAppTestBase<TModule> : AbpIntegratedTestBase<TModule>
        where TModule : AbpModule
    {
        protected readonly RoleManager RoleManager;
        protected readonly UserManager UserManager;
        protected readonly IPermissionManager PermissionManager;
        protected readonly IPermissionChecker PermissionChecker;
        protected readonly UserStore UserStore;

        protected SampleAppTestBase()
        {
            CreateInitialData();

            RoleManager = Resolve<RoleManager>();
            UserManager = Resolve<UserManager>();
            PermissionManager = Resolve<IPermissionManager>();
            PermissionChecker = Resolve<IPermissionChecker>();
            UserStore = Resolve<UserStore>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            //Fake DbConnection using Effort!
            LocalIocManager.IocContainer.Register(
                Component.For<DbConnection>()
                    .UsingFactoryMethod(Effort.DbConnectionFactory.CreateTransient)
                    .LifestyleSingleton()
                );
        }

        private void CreateInitialData()
        {
            UsingDbContext(context => new InitialTestDataBuilder(context).Build());
        }

        public void UsingDbContext(Action<AppDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<AppDbContext>())
            {
                context.DisableAllFilters();
                action(context);
                context.SaveChanges();
            }
        }

        public T UsingDbContext<T>(Func<AppDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<AppDbContext>())
            {
                context.DisableAllFilters();
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        protected Tenant GetDefaultTenant()
        {
            return GetTenant(AbpTenantBase.DefaultTenantName);
        }

        protected Tenant GetTenant(string tenancyName)
        {
            return UsingDbContext(
                context =>
                {
                    return context.Tenants.Single(t => t.TenancyName == tenancyName);
                });
        }

        protected User GetDefaultTenantAdmin()
        {
            var defaultTenant = GetDefaultTenant();
            return UsingDbContext(
                context =>
                {
                    return context.Users.Single(u => u.UserName == User.AdminUserName && u.TenantId == defaultTenant.Id);
                });
        }

        protected Role CreateRole(string name)
        {
            return CreateRole(name, name);
        }

        protected Role CreateRole(string name, string displayName)
        {
            var role = new Role(null, name, displayName);

            RoleManager.Create(role).Succeeded.ShouldBe(true);

            UsingDbContext( context =>
            {
                var createdRole = context.Roles.FirstOrDefault(r => r.Name == name);
                createdRole.ShouldNotBe(null);
            });

            return role;
        }

        protected User CreateUser(string userName)
        {
            var user = new User
            {
                TenantId = AbpSession.TenantId,
                UserName = userName,
                Name = userName,
                Surname = userName,
                EmailAddress = userName + "@aspnetboilerplate.com",
                IsEmailConfirmed = true,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
            };


            WithUnitOfWork(()=> UserManager.Create(user).CheckErrors());

            UsingDbContext(context =>
            {
                var createdUser = context.Users.FirstOrDefault(u => u.UserName == userName);
                createdUser.ShouldNotBe(null);
            });

            return user;
        }

        protected async Task ProhibitPermissionAsync(Role role, string permissionName)
        {
            await RoleManager.ProhibitPermissionAsync(role, PermissionManager.GetPermission(permissionName));
            (await RoleManager.IsGrantedAsync(role.Id, PermissionManager.GetPermission(permissionName))).ShouldBe(false);
        }

        protected async Task GrantPermissionAsync(Role role, string permissionName)
        {
            await RoleManager.GrantPermissionAsync(role, PermissionManager.GetPermission(permissionName));
            (await RoleManager.IsGrantedAsync(role.Id, PermissionManager.GetPermission(permissionName))).ShouldBe(true);
        }

        protected async Task GrantPermissionAsync(User user, string permissionName)
        {
            await UserManager.GrantPermissionAsync(user, PermissionManager.GetPermission(permissionName));
            (await UserManager.IsGrantedAsync(user.Id, permissionName)).ShouldBe(true);
        }
        
        protected void GrantPermission(User user, string permissionName)
        {
            GrantPermission(user, PermissionManager.GetPermission(permissionName));
            UserManager.IsGranted(user.Id, permissionName).ShouldBe(true);
        }
        
        /// <summary>
        /// Grants a permission for a user if not already granted.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="permission">Permission</param>
        protected void GrantPermission(User user, Permission permission)
        {
            UserStore.RemovePermission(user, new PermissionGrantInfo(permission.Name, false));

            if (UserManager.IsGranted(user.Id, permission))
            {
                return;
            }

            UserStore.AddPermission(user, new PermissionGrantInfo(permission.Name, true));
        }

        protected async Task ProhibitPermissionAsync(User user, string permissionName)
        {
            await UserManager.ProhibitPermissionAsync(user, PermissionManager.GetPermission(permissionName));
            (await UserManager.IsGrantedAsync(user.Id, permissionName)).ShouldBe(false);
        }
        
        protected void ProhibitPermission(User user, Permission permission)
        {
            UserStore.RemovePermission(user, new PermissionGrantInfo(permission.Name, true));

            if (!UserManager.IsGranted(user.Id, permission))
            {
                return;
            }

            UserStore.AddPermission(user, new PermissionGrantInfo(permission.Name, false));
            
            UserManager.IsGranted(user.Id, permission.Name).ShouldBe(false);
        }
    }
}
