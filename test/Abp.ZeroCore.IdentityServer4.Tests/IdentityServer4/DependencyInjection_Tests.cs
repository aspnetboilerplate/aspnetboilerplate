using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Security;
using Abp.ZeroCore.SampleApp.Core;
using Microsoft.AspNetCore.Identity;
using Shouldly;
using Xunit;

namespace Abp.IdentityServer4
{
    public class DependencyInjection_Tests : AbpZeroIdentityServerTestBase
    {
        [Fact]
        public void Should_Inject_AbpPersistedGrantStore()
        {
            Resolve<AbpPersistedGrantStore>();
        }

        [Fact]
        public async Task Should_Inject_AbpUserClaimsPrincipalFactory()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            AbpSession.TenantId = 1;

            var repository = Resolve<IRepository<User, long>>();

            var userToAdd = User.CreateTenantAdminUser(AbpSession.TenantId.Value, "admin@test.com");
            userToAdd.Password = "123qwe";
            var userId = await repository.InsertAndGetIdAsync(userToAdd);

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                //Arrange
                var user = await repository.FirstOrDefaultAsync(userId);
                user.ShouldNotBeNull();

                var principalFactory = Resolve<IUserClaimsPrincipalFactory<User>>();

                //Act
                var identity = (await principalFactory.CreateAsync(user)).Identity;

                //Assert
                identity.GetTenantId().ShouldBe(AbpSession.TenantId);
                identity.GetUserId().ShouldBe(user.Id);

                await uow.CompleteAsync();
            }
        }
    }
}
