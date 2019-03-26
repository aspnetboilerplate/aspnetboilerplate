using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Auditing;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using Xunit;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests
{
    public class AbpODataEntityControllerTests
    {
        private readonly WebApplicationFactory<Startup> _factory;
        
        private IAuditingStore _auditingStore;

        public AbpODataEntityControllerTests()
        {
            _factory = new WebApplicationFactory<Startup>();
        }

//        [Fact]
//        public async Task RazorPage_RazorAuditPageFilter_Get_Test()
//        {
//            // Arrange
//            var client = _factory.CreateClient();

//            // Act
//            var response = await client.GetAsync("/" + pageName);

//            // Assert
//            response.EnsureSuccessStatusCode();

//#pragma warning disable 4014

//            if (shouldWriteAuditLog)
//            {
//                _auditingStore.Received().SaveAsync(Arg.Is<AuditInfo>(a => a.ServiceName.Contains(pageName)));
//            }
//            else
//            {
//                _auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
//            }

//#pragma warning restore 4014
//        }
    }
}
