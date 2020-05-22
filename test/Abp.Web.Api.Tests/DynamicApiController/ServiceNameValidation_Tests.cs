using Abp.WebApi.Controllers.Dynamic.Builders;
using Xunit;

namespace Abp.Web.Api.Tests.DynamicApiController
{
    public class ServiceNameValidation_Tests
    {
        [Fact]
        public void Valid_Service_Names()
        {
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceName("taskever/task"));
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceName("taskever/tasks/task"));
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceName("taskever/taskService"));
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceName("taskever/tasks/taskService"));
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceName("taskever/tasks/task_service"));
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceName("taskever/myNameSpace1/MyNameSpace2/mynamespace3/myserviceName"));
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceName("taskever/myName_Space1/MyName_Space2/mynamespace_3/myservice"));
        }

        [Fact]
        public void Valid_Service_WithAction_Names()
        {
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/task/create"));
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/tasks/task/update"));
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/taskService/delete"));
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/tasks/taskService/getAllTasks"));
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/tasks/task_service/deleteTask"));
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/myNameSpace1/MyNameSpace2/mynamespace3/myserviceName/CreateNew"));
            Assert.True(DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/myName_Space1/MyName_Space2/mynamespace_3/myservice/test_action_Name"));
        }

        [Fact]
        public void Invalid_Service_Names()
        {
            Assert.False(DynamicApiServiceNameHelper.IsValidServiceName(""));
            Assert.False(DynamicApiServiceNameHelper.IsValidServiceName("task"));
            Assert.False(DynamicApiServiceNameHelper.IsValidServiceName("task_service"));
            Assert.False(DynamicApiServiceNameHelper.IsValidServiceName("taskever/task service"));
            Assert.False(DynamicApiServiceNameHelper.IsValidServiceName("taskever/123task_service"));
            Assert.False(DynamicApiServiceNameHelper.IsValidServiceName(" taskever/task_service"));
            Assert.False(DynamicApiServiceNameHelper.IsValidServiceName("taskever/ task_service"));
        }

        [Fact]
        public void Invalid_ServiceWithAction_Names()
        {
            Assert.False(DynamicApiServiceNameHelper.IsValidServiceNameWithAction(""));
            Assert.False(DynamicApiServiceNameHelper.IsValidServiceNameWithAction("task"));
            Assert.False(DynamicApiServiceNameHelper.IsValidServiceNameWithAction("task_service"));
            Assert.False(DynamicApiServiceNameHelper.IsValidServiceNameWithAction("task_service/create"));
        }
    }
}
