using System.Linq;
using System.Threading.Tasks;
using Abp.IdentityFramework;
using Abp.Organizations;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Roles
{
    public class OrganizationUnitRole_Tests : SampleAppTestBase
    {
        public OrganizationUnitRole_Tests()
        {
            var defaultTenant = GetDefaultTenant();
            AbpSession.TenantId = defaultTenant.Id;
        }

        [Fact]
        public async Task Test_AddToOrganizationUnitAsync()
        {
            //Arrange
            var ou2 = GetOu("OU2");
            var role = await CreateRole("role_1");

            //Act
            await RoleManager.AddToOrganizationUnitAsync(role, ou2);

            //Assert
            (await RoleManager.IsInOrganizationUnitAsync(role, ou2)).ShouldBe(true);
            UsingDbContext(context => context.OrganizationUnitRoles.FirstOrDefault(ou => ou.RoleId == role.Id && ou.OrganizationUnitId == ou2.Id).ShouldNotBeNull());
        }

        [Fact]
        public async Task Test_RemoveFromOrganizationUnitAsync()
        {
            //Arrange
            var ou11 = GetOu("OU11");
            var role = await CreateRole("role_1");

            //Act
            await RoleManager.RemoveFromOrganizationUnitAsync(role, ou11);

            //Assert
            (await RoleManager.IsInOrganizationUnitAsync(role, ou11)).ShouldBe(false);
            UsingDbContext(context => context.OrganizationUnitRoles.FirstOrDefault(ou => ou.RoleId == role.Id && ou.OrganizationUnitId == ou11.Id).ShouldBeNull());
        }

        [Fact]
        public async Task Should_Remove_Role_From_Organization_When_Role_Is_Deleted()
        {
            //Arrange
            var role = await CreateRole("role_1");
            var ou11 = GetOu("OU11");

            await RoleManager.AddToOrganizationUnitAsync(role, ou11);
            (await RoleManager.IsInOrganizationUnitAsync(role, ou11)).ShouldBe(true);

            //Act
            (await RoleManager.DeleteAsync(role)).CheckErrors();

            //Assert
            (await RoleManager.IsInOrganizationUnitAsync(role, ou11)).ShouldBe(false);
        }

        [Theory]
        [InlineData(new object[] { new string[0] })]
        [InlineData(new object[] { new[] { "OU12", "OU21" } })]
        [InlineData(new object[] { new[] { "OU11", "OU12", "OU2" } })]
        public async Task Test_SetOrganizationUnitsAsync(string[] organizationUnitNames)
        {
            //Arrange
            var role = await CreateRole("role_1");
            var organizationUnitIds = organizationUnitNames.Select(oun => GetOu(oun).Id).ToArray();

            //Act
            await RoleManager.SetOrganizationUnitsAsync(role, organizationUnitIds);

            //Assert
            UsingDbContext(context =>
            {
                context.OrganizationUnitRoles
                    .Count(uou => uou.RoleId == role.Id && organizationUnitIds.Contains(uou.OrganizationUnitId))
                    .ShouldBe(organizationUnitIds.Length);
            });
        }

        [Fact]
        public async Task Test_GetRolesInOrganizationUnit()
        {
            //Act & Assert
            (await RoleManager.GetRolesInOrganizationUnit(GetOu("OU11"))).Count.ShouldBe(1);
            (await RoleManager.GetRolesInOrganizationUnit(GetOu("OU1"))).Count.ShouldBe(0);
            (await RoleManager.GetRolesInOrganizationUnit(GetOu("OU1"), true)).Count.ShouldBe(1);
        }

        private OrganizationUnit GetOu(string displayName)
        {
            var organizationUnit = UsingDbContext(context => context.OrganizationUnits.FirstOrDefault(ou => ou.DisplayName == displayName));
            organizationUnit.ShouldNotBeNull();

            return organizationUnit;
        }
    }
}
