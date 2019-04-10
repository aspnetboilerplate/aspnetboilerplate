using System.Net.Http;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Dependency;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using NSubstitute;
using System.Collections.Generic;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests
{
    public class RazorAuditPageFilterTests
    {
        private readonly WebApplicationFactory<Startup> _factory;
        
        private IAuditingStore _auditingStore;

        public RazorAuditPageFilterTests()
        {
            _factory = new WebApplicationFactory<Startup>();
            
            RegisterFakeAuditingStore();
        }

        private void RegisterFakeAuditingStore()
        {
            Startup.IocManager.Value = new IocManager();

            _auditingStore = Substitute.For<IAuditingStore>();
            Startup.IocManager.Value.IocContainer.Register(
                Component.For<IAuditingStore>().Instance(
                    _auditingStore
                ).IsDefault()
            );
        }

        [Theory]
        [InlineData("AuditFilterPageDemo", true)]
        [InlineData("AuditFilterPageDemo2", false)]
        [InlineData("AuditFilterPageDemo3", false)]
        public async Task RazorPage_RazorAuditPageFilter_Get_Test(string pageName, bool shouldWriteAuditLog)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/" + pageName);

            // Assert
            response.EnsureSuccessStatusCode();

#pragma warning disable 4014

            if (shouldWriteAuditLog)
            {
                _auditingStore.Received().SaveAsync(Arg.Is<AuditInfo>(a => a.ServiceName.Contains(pageName)));
            }
            else
            {
                _auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
            }

#pragma warning restore 4014
        }

        [Theory]
        [InlineData("AuditFilterPageDemo", true)]
        [InlineData("AuditFilterPageDemo2", false)]
        [InlineData("AuditFilterPageDemo3", false)]
        public async Task RazorPage_RazorAuditPageFilter_Post_Test(string pageName, bool shouldWriteAuditLog)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync("/" + pageName, new StringContent(""));

            // Assert
            response.EnsureSuccessStatusCode();

#pragma warning disable 4014

            if (shouldWriteAuditLog)
            {
                _auditingStore.Received().SaveAsync(Arg.Is<AuditInfo>(a => a.ServiceName.Contains(pageName)));
            }
            else
            {
                _auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
            }

#pragma warning restore 4014
        }

        [Fact]
        public async Task RazorPage_RazorAuditPageFilter_Post_With_Args_Test()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act

            var response = await client.PostAsync("/AuditFilterPageDemo", new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("Message","My test message"),
            }));

            // Assert
            response.EnsureSuccessStatusCode();

#pragma warning disable 4014

            _auditingStore.Received().SaveAsync(Arg.Is<AuditInfo>(a => a.ServiceName.Contains("AuditFilterPageDemo") && a.Parameters.Contains("My test message")));

#pragma warning restore 4014
        }
    }
}
