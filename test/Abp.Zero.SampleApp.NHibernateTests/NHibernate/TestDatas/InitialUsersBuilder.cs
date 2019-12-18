using System.Linq;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Users;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Linq;

namespace Abp.Zero.SampleApp.NHibernate.TestDatas
{
    public class InitialUsersBuilder
    {
        private readonly ISession _session;

        public InitialUsersBuilder(ISession session)
        {
            _session = session;
        }

        public void Build()
        {
            CreateUsers();
        }

        private void CreateUsers()
        {
            var defaultTenant = _session.Query<Tenant>().Single(t => t.Name == Tenant.DefaultTenantName);

            var user = new User
            {
                TenantId = defaultTenant.Id,
                Name = "System",
                Surname = "Administrator",
                UserName = User.AdminUserName,
                Password = new PasswordHasher().HashPassword("123qwe"),
                EmailAddress = "admin@aspnetboilerplate.com"
            };

            user.SetNormalizedNames();

            _session.Save(user);
        }
    }
}