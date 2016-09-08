using Abp.WebApi.Controllers.Dynamic.Builders;
using Xunit;

namespace Abp.Web.Api.Tests.DynamicApiController
{
    public class ServiceNameValidation_Tests
    {
        [Fact]
        public void Valid_Service_Names()
        {
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/task"));
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/tasks/task"));
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/taskService"));
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/tasks/taskService"));
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/tasks/task_service"));
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/myNameSpace1/MyNameSpace2/mynamespace3/myserviceName"));
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/myName_Space1/MyName_Space2/mynamespace_3/myservice"));
        }

        [Fact]
        public void Valid_Service_WithAction_Names()
        {
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/task/create"));
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/tasks/task/update"));
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/taskService/delete"));
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/tasks/taskService/getAllTasks"));
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/tasks/task_service/deleteTask"));
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/myNameSpace1/MyNameSpace2/mynamespace3/myserviceName/CreateNew"));
            Assert.Equal(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/myName_Space1/MyName_Space2/mynamespace_3/myservice/test_action_Name"));
        }

        [Fact]
        public void Invalid_Service_Names()
        {
            Assert.Equal(false, DynamicApiServiceNameHelper.IsValidServiceName(""));
            Assert.Equal(false, DynamicApiServiceNameHelper.IsValidServiceName("task"));
            Assert.Equal(false, DynamicApiServiceNameHelper.IsValidServiceName("task_service"));
            Assert.Equal(false, DynamicApiServiceNameHelper.IsValidServiceName("taskever/task service"));
            Assert.Equal(false, DynamicApiServiceNameHelper.IsValidServiceName("taskever/123task_service"));
            Assert.Equal(false, DynamicApiServiceNameHelper.IsValidServiceName(" taskever/task_service"));
            Assert.Equal(false, DynamicApiServiceNameHelper.IsValidServiceName("taskever/ task_service"));
        }

        [Fact]
        public void Invalid_ServiceWithAction_Names()
        {
            Assert.Equal(false, DynamicApiServiceNameHelper.IsValidServiceNameWithAction(""));
            Assert.Equal(false, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("task"));
            Assert.Equal(false, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("task_service"));
            Assert.Equal(false, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("task_service/create"));
        }
    }
}
