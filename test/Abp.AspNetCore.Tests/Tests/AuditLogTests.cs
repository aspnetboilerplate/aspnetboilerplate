using System.Net;
using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Abp.AspNetCore.App.Models;
using Abp.AspNetCore.Mocks;
using Abp.Auditing;
using Abp.Web.Models;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests
{
    public class AuditLogTests : AppTestBase
    {
        private readonly MockAuditingStore _mockAuditingStore;

        public AuditLogTests()
        {
            _mockAuditingStore = Resolve<MockAuditingStore>();
        }

        [Fact]
        public async Task Should_Write_Audit_Logs()
        {
            _mockAuditingStore.Logs.Count.ShouldBe(0);

            //Act

            await GetResponseAsObjectAsync<AjaxResponse<SimpleViewModel>>(
                   GetUrl<SimpleTestController>(
                       nameof(SimpleTestController.SimpleJsonException),
                       new
                       {
                           message = "A test message",
                           userFriendly = true
                       }),
                   HttpStatusCode.InternalServerError
               );

            //Assert

            _mockAuditingStore.Logs.Count.ShouldBe(1);
            var auditLog = _mockAuditingStore.Logs.ToArray()[0];
            auditLog.MethodName.ShouldBe(nameof(SimpleTestController.SimpleJsonException));
        }

        [Theory]
        [InlineData(nameof(SimpleTestController.SimpleJson))]
        [InlineData(nameof(SimpleTestController.SimpleObject))]
        public async Task Audit_Logs_Should_Set_ReturnValue_When_Return_JsonResult_Or_ObjectResult(string url)
        {
            Resolve<IAuditingConfiguration>().SaveReturnValues = true;
            _mockAuditingStore.Logs.Count.ShouldBe(0);

            //Act

            await GetResponseAsObjectAsync<AjaxResponse<SimpleViewModel>>(GetUrl<SimpleTestController>(url));

            //Assert

            _mockAuditingStore.Logs.Count.ShouldBe(1);
            var auditLog = _mockAuditingStore.Logs.ToArray()[0];
            auditLog.MethodName.ShouldBe(url);
            auditLog.ReturnValue.ShouldBe("{\"strValue\":\"Forty Two\",\"intValue\":42}");
        }

        [Fact]
        public async Task Audit_Logs_Should_Set_ReturnValue_When_Return_StringResult()
        {
            Resolve<IAuditingConfiguration>().SaveReturnValues = true;
            _mockAuditingStore.Logs.Count.ShouldBe(0);

            //Act

            await GetResponseAsStringAsync(GetUrl<SimpleTestController>(nameof(SimpleTestController.SimpleString)));

            //Assert

            _mockAuditingStore.Logs.Count.ShouldBe(1);
            var auditLog = _mockAuditingStore.Logs.ToArray()[0];
            auditLog.MethodName.ShouldBe(nameof(SimpleTestController.SimpleString));
            auditLog.ReturnValue.ShouldBe("\"test\"");
        }

        [Fact]
        public async Task Audit_Logs_Should_Set_ReturnValue_When_Return_ContentResult()
        {
            Resolve<IAuditingConfiguration>().SaveReturnValues = true;
            _mockAuditingStore.Logs.Count.ShouldBe(0);

            //Act

            await GetResponseAsStringAsync(GetUrl<SimpleTestController>(nameof(SimpleTestController.SimpleContent)));

            //Assert

            _mockAuditingStore.Logs.Count.ShouldBe(1);
            var auditLog = _mockAuditingStore.Logs.ToArray()[0];
            auditLog.MethodName.ShouldBe(nameof(SimpleTestController.SimpleContent));
            auditLog.ReturnValue.ShouldBe("Hello world...");
        }

        
    }
}