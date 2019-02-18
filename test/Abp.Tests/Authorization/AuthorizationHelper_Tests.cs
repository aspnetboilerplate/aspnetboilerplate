using System.Reflection;
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
            _authorizeHelper = GetAuthorizationHelper(false, false);
        }

        [Fact]
        public async Task NotAuthorizedMethodsCanBeCalledAnonymously()
        {
            await _authorizeHelper.AuthorizeAsync(
                typeof(MyNonAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyNonAuthorizedClass.Test_NotAuthorized)),
                typeof(MyNonAuthorizedClass)
                );

            await _authorizeHelper.AuthorizeAsync(
                typeof(MyAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyAuthorizedClass.Test_NotAuthorized)),
                typeof(MyAuthorizedClass)
            );
        }

        [Fact]
        public async Task AuthorizedMethodsCanNotBeCalledAnonymously()
        {
            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await _authorizeHelper.AuthorizeAsync(
                    typeof(MyNonAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyNonAuthorizedClass.Test_Authorized)),
                    typeof(MyNonAuthorizedClass)
                );
            });

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await _authorizeHelper.AuthorizeAsync(
                    typeof(MyAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyAuthorizedClass.Test_Authorized)),
                    typeof(MyAuthorizedClass)
                );
            });
        }

        [Fact]
        public async Task NotAuthorizedFeatureDependentMethodsCanBeCalledAnonymously()
        {
            var authorizeHelper = GetAuthorizationHelper(true, false);

            await authorizeHelper.AuthorizeAsync(
                typeof(MyNonAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyNonAuthorizedClass.Test_FeatureDependent)),
                typeof(MyNonAuthorizedClass)
            );
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

            [RequiresFeature("Test")]
            public void Test_FeatureDependent()
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

        private static AuthorizationHelper GetAuthorizationHelper(
            bool featureCheckerValue,
            bool isGranted)
        {
            var featureChecker = Substitute.For<IFeatureChecker>();
            featureChecker.GetValueAsync(Arg.Any<string>()).Returns(featureCheckerValue.ToString().ToLower());    
            featureChecker.IsEnabledAsync(Arg.Any<string>()).Returns(featureCheckerValue);
            
            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGrantedAsync(Arg.Any<string>()).Returns(isGranted);

            var configuration = Substitute.For<IAuthorizationConfiguration>();
            configuration.IsEnabled.Returns(true);

            var authorizeHelper = new AuthorizationHelper(featureChecker, configuration)
            {
                PermissionChecker = permissionChecker
            };

            return authorizeHelper;
        }
    }
}
