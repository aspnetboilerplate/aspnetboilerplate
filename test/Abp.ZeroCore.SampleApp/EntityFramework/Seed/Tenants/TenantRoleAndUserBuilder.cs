using System.Linq;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.ZeroCore.SampleApp.Application;
using Abp.ZeroCore.SampleApp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Abp.ZeroCore.SampleApp.EntityFramework.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly SampleAppDbContext _context;
        private readonly int _tenantId;

        public TenantRoleAndUserBuilder(SampleAppDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            //Admin role

            var adminRole = _context.Roles.FirstOrDefault(r => r.TenantId == _tenantId && r.Name == AppStaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(_tenantId, AppStaticRoleNames.Tenants.Admin, AppStaticRoleNames.Tenants.Admin) { IsStatic = true }).Entity;
                _context.SaveChanges();

                //Grant all permissions to admin role
                var permissions = PermissionFinder
                    .GetAllPermissions(new AppAuthorizationProvider())
                    .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant))
                    .ToList();

                foreach (var permission in permissions)
                {
                    _context.Permissions.Add(
                        new RolePermissionSetting
                        {
                            TenantId = _tenantId,
                            Name = permission.Name,
                            IsGranted = true,
                            RoleId = adminRole.Id
                        });
                }

                _context.SaveChanges();
            }

            //User role

            var userRole = _context.Roles.FirstOrDefault(r => r.TenantId == _tenantId && r.Name == AppStaticRoleNames.Tenants.User);
            if (userRole == null)
            {
                _context.Roles.Add(new Role(_tenantId, AppStaticRoleNames.Tenants.User, AppStaticRoleNames.Tenants.User) { IsStatic = true, IsDefault = true });
                _context.SaveChanges();
            }

            //admin user

            var adminUser = _context.Users.FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser != null)
            {
                return;
            }
            
            adminUser = User.CreateTenantAdminUser(_tenantId, "admin@defaulttenant.com");
            adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "123qwe");
            adminUser.IsEmailConfirmed = true;
            adminUser.IsActive = true;

            _context.Users.Add(adminUser);
            _context.SaveChanges();

            //Assign Admin role to admin user
            _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
            _context.SaveChanges();

            //User account of admin user
            if (_tenantId != 1)
            {
                return;
            }
                
            _context.UserAccounts.Add(new UserAccount
            {
                TenantId = _tenantId,
                UserId = adminUser.Id,
                UserName = AbpUserBase.AdminUserName,
                EmailAddress = adminUser.EmailAddress
            });
            _context.SaveChanges();
        }
    }
}
