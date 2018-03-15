using System;
using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Abp.Authorization;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Web.Models;
using Abp.Web.MultiTenancy;
using Microsoft.Net.Http.Headers;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class MultiTenancy_Tests : AppTestBase
    {
        private readonly IWebMultiTenancyConfiguration _multiTenancyConfiguration;
        private readonly MyMultiTenancyClass _multiTenancyClass;
        private readonly MyTenantClass _tenantClass;

        public MultiTenancy_Tests()
        {
            _multiTenancyConfiguration = Resolve<IWebMultiTenancyConfiguration>();
            _multiTenancyClass = Resolve<MyMultiTenancyClass>();
            _tenantClass = Resolve<MyTenantClass>();
        }

        [Fact]
        public async Task HttpHeaderTenantResolveContributor_Test()
        {
            Client.DefaultRequestHeaders.Add(MultiTenancyConsts.TenantIdResolveKey, "42");

            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<int?>>(
                GetUrl<MultiTenancyTestController>(
                    nameof(MultiTenancyTestController.GetTenantId)
                )
            );

            //Assert
            response.Result.ShouldBe(42);
        }

        [Fact]
        public async Task HttpCookieTenantResolveContributor_Test()
        {
            Client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(MultiTenancyConsts.TenantIdResolveKey, "42").ToString());

            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<int?>>(
                GetUrl<MultiTenancyTestController>(
                    nameof(MultiTenancyTestController.GetTenantId)
                )
            );

            //Assert
            response.Result.ShouldBe(42);
        }

        [Fact]
        public async Task Header_Should_Have_High_Priority_Than_Cookie()
        {
            Client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(MultiTenancyConsts.TenantIdResolveKey, "43").ToString());
            Client.DefaultRequestHeaders.Add(MultiTenancyConsts.TenantIdResolveKey, "42");

            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<int?>>(
                GetUrl<MultiTenancyTestController>(
                    nameof(MultiTenancyTestController.GetTenantId)
                )
            );

            //Assert
            response.Result.ShouldBe(42);
        }

        [Theory]
        [InlineData("http://{TENANCY_NAME}.mysite.com", "http://default.mysite.com")]
        [InlineData("http://{TENANCY_NAME}.mysite.com:8080", "http://default.mysite.com:8080")]
        [InlineData("http://{TENANCY_NAME}.mysite.com/", "http://default.mysite.com/")]
        public async Task DomainTenantResolveContributor_Test(string domainFormat, string domain)
        {
            _multiTenancyConfiguration.DomainFormat = domainFormat;
            Client.BaseAddress = new Uri(domain);

            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<int?>>(
                GetUrl<MultiTenancyTestController>(
                    nameof(MultiTenancyTestController.GetTenantId)
                )
            );

            //Assert
            response.Result.ShouldBe(1);
        }

        [Fact]
        public void Should_Not_Intercept_Class_For_Authenticated_User()
        {
            using (AbpSession.Use(null, 1))
            {
                //Allow authenticated host user to call tenant method
                _tenantClass.TenantMethod();
            }

            using (AbpSession.Use(1, 1))
            {
                //Allow authenticated tenant user to call host method
                _tenantClass.HostMethod();
            }
        }

        [Fact]
        public void Should_Not_Intercept_Method_For_Authenticated_User()
        {
            using (AbpSession.Use(null, 1))
            {
                //Allow authenticated host user to call tenant method
                _multiTenancyClass.TenantMethod();
            }

            using (AbpSession.Use(1, 1))
            {
                //Allow authenticated tenant user to call host method
                _multiTenancyClass.HostMethod();
            }
        }

        [Fact]
        public void Should_Intercept_Class_For_Anonymous_Host_User()
        {
            using (AbpSession.Use(null, null))
            {
                Should.Throw<AbpAuthorizationException>(() => _tenantClass.TenantMethod()).Message.ShouldBe("Anonymous host user must not call tenant method.");
            }
        }

        [Fact]
        public void Should_Intercept_Method_For_Anonymous_Host_User()
        {
            using (AbpSession.Use(null, null))
            {
                Should.Throw<AbpAuthorizationException>(() => _multiTenancyClass.TenantMethod()).Message.ShouldBe("Anonymous host user must not call tenant method.");
                _multiTenancyClass.BothMethod();
            }
        }

        [Fact]
        public void Should_Intercept_Class_For_Anonymous_Tenant_User()
        {
            using (AbpSession.Use(1, null))
            {
                Should.Throw<AbpAuthorizationException>(() => _tenantClass.HostMethod()).Message.ShouldBe("Anonymous tenant user must not call host method.");
            }
        }

        [Fact]
        public void Should_Intercept_Method_For_Anonymous_Tenant_User()
        {
            using (AbpSession.Use(1, null))
            {
                Should.Throw<AbpAuthorizationException>(() => _multiTenancyClass.HostMethod()).Message.ShouldBe("Anonymous tenant user must not call host method.");
                _multiTenancyClass.BothMethod();
            }
        }
    }

    public class MyMultiTenancyClass : ITransientDependency
    {
        public MyMultiTenancyClass()
        {
        }

        [MultiTenancySide(MultiTenancySides.Host)]
        public virtual void HostMethod()
        {
        }

        [MultiTenancySide(MultiTenancySides.Tenant)]
        public virtual void TenantMethod()
        {
        }

        [MultiTenancySide(MultiTenancySides.Host | MultiTenancySides.Tenant)]
        public virtual void BothMethod()
        {
        }
    }

    [MultiTenancySide(MultiTenancySides.Tenant)]
    public class MyTenantClass : ITransientDependency
    {
        public MyTenantClass()
        {
        }

        [MultiTenancySide(MultiTenancySides.Host)]
        public virtual void HostMethod()
        {
        }

        public virtual void TenantMethod()
        {
        }

        public virtual void AnotherMethod()
        {
        }
    }
}
