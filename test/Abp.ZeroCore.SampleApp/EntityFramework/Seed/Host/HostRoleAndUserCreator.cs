using System.Linq;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.ZeroCore.SampleApp.Application;
using Abp.ZeroCore.SampleApp.Core;

namespace Abp.ZeroCore.SampleApp.EntityFramework.Seed.Host
{
    public class HostRoleAndUserCreator
    {
        private readonly SampleAppDbContext _context;

        public HostRoleAndUserCreator(SampleAppDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateHostRoleAndUsers();
        }

        private void CreateHostRoleAndUsers()
        {
            //Admin role for host

            var adminRoleForHost = _context.Roles.FirstOrDefault(r => r.TenantId == null && r.Name == AppStaticRoleNames.Host.Admin);
            if (adminRoleForHost == null)
            {
                adminRoleForHost = _context.Roles.Add(new Role(null, AppStaticRoleNames.Host.Admin, AppStaticRoleNames.Host.Admin) { IsStatic = true, IsDefault = true }).Entity;
                _context.SaveChanges();
            }

            //admin user for host

            var adminUserForHost = _context.Users.FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
            if (adminUserForHost != null)
            {
                return;
            }
            
            var user = new User
            {
                TenantId = null,
                UserName = AbpUserBase.AdminUserName,
                Name = "admin",
                Surname = "admin",
                EmailAddress = "admin@aspnetzero.com",
                IsEmailConfirmed = true,
                IsActive = true,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
            };

            user.SetNormalizedNames();

            adminUserForHost = _context.Users.Add(user).Entity;
            _context.SaveChanges();

            //Assign Admin role to admin user
            _context.UserRoles.Add(new UserRole(null, adminUserForHost.Id, adminRoleForHost.Id));
            _context.SaveChanges();

            //Grant all permissions
            var permissions = PermissionFinder
                .GetAllPermissions(new AppAuthorizationProvider())
                .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Host))
                .ToList();

            foreach (var permission in permissions)
            {
                _context.Permissions.Add(
                    new RolePermissionSetting
                    {
                        TenantId = null,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = adminRoleForHost.Id
                    });
            }

            _context.SaveChanges();

            //User account of admin user
            _context.UserAccounts.Add(new UserAccount
            {
                TenantId = null,
                UserId = adminUserForHost.Id,
                UserName = AbpUserBase.AdminUserName,
                EmailAddress = adminUserForHost.EmailAddress
            });

            _context.SaveChanges();
        }
    }
}
