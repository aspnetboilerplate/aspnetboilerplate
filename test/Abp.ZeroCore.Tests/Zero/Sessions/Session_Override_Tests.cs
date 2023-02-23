using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Sessions
{
    public class Session_Override_Tests : AbpZeroTestBase
    {
        private readonly RoleStore _roleStore;

        public Session_Override_Tests()
        {
            _roleStore = Resolve<RoleStore>();
        }

        [Fact]
        public async Task Should_Override_Session_Values()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                using (AbpSession.Use(null,2))
                {
                    await _roleStore.CreateAsync(new Role
                    {
                        Name = "Manager",
                        NormalizedName = "MANAGER",
                        DisplayName = "Manager"
                    });   
                }
            });
            
            await WithUnitOfWorkAsync(async () =>
            {
                var managerRole = await _roleStore.FindByNameAsync("MANAGER");
                managerRole.TenantId.ShouldBe(1);
                managerRole.CreatorUserId.ShouldBe(2);
                managerRole.ShouldNotBeNull();
            });
        }
    }
}