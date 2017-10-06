using System.Linq;
using Abp.Zero.SampleApp.Users;
using Microsoft.AspNet.Identity;

namespace Abp.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.HostDatas
{
    public class HostUserBuilder
    {
        private readonly AppDbContext _context;

        public HostUserBuilder(AppDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            CreateUsers();
        }

        private void CreateUsers()
        {
            var adminUser = _context.Users.FirstOrDefault(u => u.UserName == User.AdminUserName);

            if (adminUser == null)
            {
                adminUser = _context.Users.Add(
                    new User
                    {
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