using System;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Tests.Authorization
{
    public class AuthorizationInterceptor_Tests : TestBaseWithLocalIocManager
    {
        private readonly MyTestClassToBeAuthorized_Sync _syncObj;
        private readonly MyTestClassToBeAuthorized_Async _asyncObj;
        private readonly MyTestClassToBeAllowProtected_Async _asyncObjForProtectedMethod;
        private readonly MyTestClassToBeAllowProtected_Sync _syncObjForProtectedMethod;

        public AuthorizationInterceptor_Tests()
        {
            //SUT: AuthorizationInterceptor and AuthorizeAttributeHelper
            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureChecker>().Instance(Substitute.For<IFeatureChecker>())
                );

            LocalIocManager.Register<IAuthorizationConfiguration, AuthorizationConfiguration>();
            LocalIocManager.Register<IMultiTenancyConfig, MultiTenancyConfig>();
            LocalIocManager.Register<AuthorizationInterceptor>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<IAuthorizationHelper, AuthorizationHelper>(DependencyLifeStyle.Transient);
            LocalIocManager.IocContainer.Register(
                Component.For<MyTestClassToBeAuthorized_Sync>().Interceptors<AuthorizationInterceptor>().LifestyleTransient(),
                Component.For<MyTestClassToBeAuthorized_Async>().Interceptors<AuthorizationInterceptor>().LifestyleTransient(),
                Component.For<MyTestClassToBeAllowProtected_Async>().Interceptors<AuthorizationInterceptor>().LifestyleTransient(),
                Component.For<MyTestClassToBeAllowProtected_Sync>().Interceptors<AuthorizationInterceptor>().LifestyleTransient()
                );

            //Mock session
            var session = Substitute.For<IAbpSession>();
            session.TenantId.Returns(1);
            session.UserId.Returns(1);
            LocalIocManager.IocContainer.Register(Component.For<IAbpSession>().Instance(session));

            //Mock permission checker
            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGrantedAsync("Permission1").Returns(true);
            permissionChecker.IsGrantedAsync("Permission2").Returns(true);
            permissionChecker.IsGrantedAsync("Permission3").Returns(false); //Permission3 is not granted

            permissionChecker.IsGranted("Permission1").Returns(true);
            permissionChecker.IsGranted("Permission2").Returns(true);
            permissionChecker.IsGranted("Permission3").Returns(false); //Permission3 is not granted

            LocalIocManager.IocContainer.Register(Component.For<IPermissionChecker>().Instance(permissionChecker));

            _syncObj = LocalIocManager.Resolve<MyTestClassToBeAuthorized_Sync>();
            _asyncObj = LocalIocManager.Resolve<MyTestClassToBeAuthorized_Async>();

            _syncObjForProtectedMethod = LocalIocManager.Resolve<MyTestClassToBeAllowProtected_Sync>();
            _asyncObjForProtectedMethod = LocalIocManager.Resolve<MyTestClassToBeAllowProtected_Async>();
        }

        [Fact]
        public void Test_Authorization_Sync()
        {
            //Authorized methods

            _syncObj.MethodWithoutPermission();
            _syncObj.Called_MethodWithoutPermission.ShouldBe(true);

            _syncObj.MethodWithPermission1().ShouldBe(42);
            _syncObj.Called_MethodWithPermission1.ShouldBe(true);

            _syncObj.MethodWithPermission1AndPermission2();
            _syncObj.Called_MethodWithPermission1AndPermission2.ShouldBe(true);

            _syncObj.MethodWithPermission1AndPermission3();
            _syncObj.Called_MethodWithPermission1AndPermission3.ShouldBe(true);

            //Non authorized methods

            Assert.Throws<AbpAuthorizationException>(() => _syncObj.MethodWithPermission3());
            _syncObj.Called_MethodWithPermission3.ShouldBe(false);

            Assert.Throws<AbpAuthorizationException>(() => _syncObj.MethodWithPermission1AndPermission3WithRequireAll());
            _syncObj.Called_MethodWithPermission1AndPermission3WithRequireAll.ShouldBe(false);
        }

        [Fact]
        public async Task Test_Authorization_Async()
        {
            //Authorized methods

            await _asyncObj.MethodWithoutPermission();
            _asyncObj.Called_MethodWithoutPermission.ShouldBe(true);

            (await _asyncObj.MethodWithPermission1Async()).ShouldBe(42);
            _asyncObj.Called_MethodWithPermission1.ShouldBe(true);

            await _asyncObj.MethodWithPermission1AndPermission2Async();
            _asyncObj.Called_MethodWithPermission1AndPermission2.ShouldBe(true);

            await _asyncObj.MethodWithPermission1AndPermission3Async();
            _asyncObj.Called_MethodWithPermission1AndPermission3.ShouldBe(true);

            await _asyncObj.MethodWithoutPermission();
            _asyncObj.Called_MethodWithoutPermission.ShouldBe(true);

            //Non authorized methods

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () => await _asyncObj.MethodWithPermission3Async());
            _asyncObj.Called_MethodWithPermission3.ShouldBe(false);

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () => await _asyncObj.MethodWithPermission1AndPermission3WithRequireAllAsync());
            _asyncObj.Called_MethodWithPermission1AndPermission3WithRequireAll.ShouldBe(false);
        }

        [Fact]
        public void Test_Authorization_For_Protected_Sync()
        {
            EmptySession();

            _syncObjForProtectedMethod.MethodWithoutForProtectedPermission();
            _syncObjForProtectedMethod.Called_AnonymousProtectedMethod.ShouldBe(true);

            //Non authorized methods

            Assert.Throws<AbpAuthorizationException>(() => _syncObjForProtectedMethod.MethodWithPermissionForProtected());
            _syncObjForProtectedMethod.Called_AuthorizedProtectedMethod.ShouldBe(false);
        }

        [Fact]
        public async Task Test_Authorization_For_Protected_Async()
        {
            EmptySession();

            await _asyncObjForProtectedMethod.MethodWithoutPermissionForProtectedAsync();
            _asyncObjForProtectedMethod.Called_AnonymousProtectedMethod.ShouldBe(true);

            //Non authorized methods

            await Assert.ThrowsAsync<AbpAuthorizationException>(async () => await _asyncObjForProtectedMethod.MethodWithPermissionForProtectedAsync());
            _asyncObjForProtectedMethod.Called_AuthorizedProtectedMethod.ShouldBe(false);
        }

        private void EmptySession()
        {
            LocalIocManager.Resolve<IAbpSession>().TenantId.Returns((int?) null);
            LocalIocManager.Resolve<IAbpSession>().UserId.Returns((int?) null);
        }

        public class MyTestClassToBeAuthorized_Sync
        {
            public bool Called_MethodWithoutPermission { get; private set; }

            public bool Called_MethodWithPermission1 { get; private set; }

            public bool Called_MethodWithPermission3 { get; private set; }

            public bool Called_MethodWithPermission1AndPermission2 { get; private set; }

            public bool Called_MethodWithPermission1AndPermission3 { get; private set; }

            public bool Called_MethodWithPermission1AndPermission3WithRequireAll { get; private set; }

            public virtual void MethodWithoutPermission()
            {
                Called_MethodWithoutPermission = true;
            }

            [AbpAuthorize("Permission1")]
            public virtual int MethodWithPermission1()
            {
                Called_MethodWithPermission1 = true;
                return 42;
            }

            //Should not be called since Permission3 is not granted
            [AbpAuthorize("Permission3")]
            public virtual void MethodWithPermission3()
            {
                Called_MethodWithPermission3 = true;
            }

            //Should be called since both of Permission1 and Permission2 are granted
            [AbpAuthorize("Permission1", "Permission2")]
            public virtual void MethodWithPermission1AndPermission2()
            {
                Called_MethodWithPermission1AndPermission2 = true;
            }

            //Should be called. Permission3 is not granted but Permission1 is granted.
            [AbpAuthorize("Permission1", "Permission3")]
            public virtual void MethodWithPermission1AndPermission3()
            {
                Called_MethodWithPermission1AndPermission3 = true;
            }

            //Should not be called. Permission3 is not granted and it required all permissions must be granted
            [AbpAuthorize("Permission1", "Permission3", RequireAllPermissions = true)]
            public virtual void MethodWithPermission1AndPermission3WithRequireAll()
            {
                Called_MethodWithPermission1AndPermission3WithRequireAll = true;
            }
        }

        public class MyTestClassToBeAuthorized_Async
        {
            public bool Called_MethodWithoutPermission { get; private set; }

            public bool Called_MethodWithPermission1 { get; private set; }

            public bool Called_MethodWithPermission3 { get; private set; }

            public bool Called_MethodWithPermission1AndPermission2 { get; private set; }

            public bool Called_MethodWithPermission1AndPermission3 { get; private set; }

            public bool Called_MethodWithPermission1AndPermission3WithRequireAll { get; private set; }

            public virtual async Task MethodWithoutPermission()
            {
                Called_MethodWithoutPermission = true;
                await Task.Delay(1);
            }

            [AbpAuthorize("Permission1")]
            public virtual async Task<int> MethodWithPermission1Async()
            {
                Called_MethodWithPermission1 = true;
                await Task.Delay(1);
                return 42;
            }

            //Should not be called since Permission3 is not granted
            [AbpAuthorize("Permission3")]
            public virtual async Task MethodWithPermission3Async()
            {
                Called_MethodWithPermission3 = true;
                await Task.Delay(1);
            }

            //Should be called since both of Permission1 and Permission2 are granted
            [AbpAuthorize("Permission1", "Permission2")]
            public virtual async Task MethodWithPermission1AndPermission2Async()
            {
                Called_MethodWithPermission1AndPermission2 = true;
                await Task.Delay(1);
            }

            //Should be called. Permission3 is not granted but Permission1 is granted.
            [AbpAuthorize("Permission1", "Permission3")]
            public virtual async Task MethodWithPermission1AndPermission3Async()
            {
                Called_MethodWithPermission1AndPermission3 = true;
                await Task.Delay(1);
            }

            //Should not be called. Permission3 is not granted and it required all permissions must be granted
            [AbpAuthorize("Permission1", "Permission3", RequireAllPermissions = true)]
            public virtual async Task MethodWithPermission1AndPermission3WithRequireAllAsync()
            {
                Called_MethodWithPermission1AndPermission3WithRequireAll = true;
                await Task.Delay(1);
            }
        }

        [AbpAuthorize]
        public class MyTestClassToBeAllowProtected_Async
        {
            public bool Called_AnonymousProtectedMethod { get; private set; }

            public bool Called_AuthorizedProtectedMethod { get; private set; }

            [AbpAllowAnonymous]
            public virtual async Task MethodWithoutPermissionForProtectedAsync()
            {
                await AnonymousProtectedMethod();
            }

            [AbpAllowAnonymous]
            public virtual async Task MethodWithPermissionForProtectedAsync()
            {
                await AuthorizedProtectedMethod();
            }

            protected virtual async Task AnonymousProtectedMethod()
            {
                Called_AnonymousProtectedMethod = true;
                await Task.Delay(1);
            }

            [AbpAuthorize]
            protected virtual async Task AuthorizedProtectedMethod()
            {
                Called_AuthorizedProtectedMethod = true;
                await Task.Delay(1);
            }
        }

        [AbpAuthorize]
        public class MyTestClassToBeAllowProtected_Sync
        {
            public bool Called_AnonymousProtectedMethod { get; private set; }

            public bool Called_AuthorizedProtectedMethod { get; private set; }

            [AbpAllowAnonymous]
            public virtual void MethodWithoutForProtectedPermission()
            {
                AnonymousProtectedMethod();
            }

            [AbpAllowAnonymous]
            public virtual void MethodWithPermissionForProtected()
            {
                AuthorizedProtectedMethod();
            }

            protected virtual void AnonymousProtectedMethod()
            {
                Called_AnonymousProtectedMethod = true;
            }

            [AbpAuthorize]
            protected virtual void AuthorizedProtectedMethod()
            {
                Called_AuthorizedProtectedMethod = true;
            }
        }
    }
}
