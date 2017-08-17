using System.Linq;
using Abp.Zero.SampleApp.Users;
using Microsoft.AspNet.Identity;

namespace Abp.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.TenantDatas
{
    public class TenantUserBuilder
    {
        private readonly AppDbContext _context;

        public TenantUserBuilder(AppDbContext context)
        {
            _context = context;
        }

        public void Build(int tenantId)
        {
            CreateUsers(tenantId);
        }

        private void CreateUsers(int tenantId)
        {
            var adminUser = _context.Users.FirstOrDefault(u => u.TenantId == tenantId && u.UserName == User.AdminUserName);

            if (adminUser == null)
            {
                adminUser = _context.Users.Add(
                    new User
                    {
                        TenantId = tenantId,
                        Name = "System",
                        Surname = "Administrator",
                        UserName = User.AdminUserName,
                        Password = new PasswordHasher().HashPassword("123qwe"),
                        EmailAddress = "admin@aspnetboilerplate.com"
                    }).Entity;

                _context.SaveChanges();
            }
        }
    }
}