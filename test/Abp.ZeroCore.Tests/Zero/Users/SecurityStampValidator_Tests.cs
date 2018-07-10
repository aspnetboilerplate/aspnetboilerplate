using Abp.Authorization;
using Abp.ZeroCore.SampleApp.Core;
using Microsoft.AspNetCore.Identity;
using Shouldly;
using Xunit;

namespace Abp.Zero.Users
{
    public class SecurityStampValidator_Tests : AbpZeroTestBase
    {
        [Fact]
        public void Should_Resolve_AbpSecurityStampValidator()
        {
            (Resolve<ISecurityStampValidator>() is AbpSecurityStampValidator<Tenant, Role, User>).ShouldBeTrue();
            (Resolve<SecurityStampValidator<User>>() is AbpSecurityStampValidator<Tenant, Role, User>).ShouldBeTrue();
        }
    }
}