using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Configuration.Startup;
using NSubstitute;
using Xunit;

namespace Abp.Tests.Authorization
{
    public class AuthorizationHelper_Tests
    {
        private readonly AuthorizationHelper _authorizeHelper;

        public AuthorizationHelper_Tests()
        {
            var featureChecker = Substitute.For<IFeatureChecker>();
            featureChecker.GetValueAsync(Arg.Any<string>()).Returns("false");

            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGrantedAsync(Arg.Any<string>()).Returns(false);

            var configuration = Substitute.For<IAuthorizationConfiguration>();
            configuration.IsEnabled.Returns(true);

            _authorizeHelper = new AuthorizationHelper(featureChecker, configuration)
            {
                PermissionChecker = permissionChecker
            };
        }

        [Fact]
        public async Task NotAuthorizedMethodsCanBeCalledAnonymously()
        {
            await _authorizeHelper.AuthorizeAsync(
                typeof(MyNonAuthorizedClass).GetMethod(nameof(MyNonAuthorizedClass.Test_NotAuthorized))
                );

            await _authorizeHelper.AuthorizeAsync(
                typeof(MyAuthorizedClass).GetMethod(nameof(MyAuthorizedClass.Test_NotAuthorized))
            );
        }

        [Fact]
        public async Task AuthorizedMethodsCanNotBeCalledAnonymously()
        {
            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await _authorizeHelper.AuthorizeAsync(
                    typeof(MyNonAuthorizedClass).GetMethod(nameof(MyNonAuthorizedClass.Test_Authorized))
                );
            });

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await _authorizeHelper.AuthorizeAsync(
                    typeof(MyAuthorizedClass).GetMethod(nameof(MyAuthorizedClass.Test_Authorized))
                );
            });
        }

        public class MyNonAuthorizedClass
        {
            public void Test_NotAuthorized()
            {

            }

            [AbpAuthorize]
            public void Test_Authorized()
            {

            }
        }

        [AbpAuthorize]
        public class MyAuthorizedClass
        {
            [AbpAllowAnonymous]
            public void Test_NotAuthorized()
            {

            }

            public void Test_Authorized()
            {

            }
        }
    }
}
