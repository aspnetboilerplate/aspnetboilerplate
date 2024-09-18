using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Roles;

public class RoleStore_Tests : AbpZeroTestBase
{
    private readonly RoleStore _roleStore;

    public RoleStore_Tests()
    {
        _roleStore = Resolve<RoleStore>();
    }

    [Fact]
    public async Task Should_Get_Role_Claims()
    {
        using (var uow = Resolve<IUnitOfWorkManager>().Begin())
        {
            var role = await _roleStore.FindByNameAsync("ADMIN");
            role.ShouldNotBeNull();

            var claims = await _roleStore.GetClaimsAsync(role);

            claims.ShouldNotBeNull();

            await uow.CompleteAsync();
        }
    }
}