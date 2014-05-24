using Abp.WebApi.Controllers.Dynamic.Builders;
using NUnit.Framework;

namespace Abp.Web.Api.Tests.DynamicApiController
{
    [TestFixture]
    public class ServiceNameValidation_Tests
    {
        [Test]
        public void Valid_Service_Names()
        {
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/task"));
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/tasks/task"));
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/taskService"));
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/tasks/taskService"));
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/tasks/task_service"));
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/myNameSpace1/MyNameSpace2/mynamespace3/myserviceName"));
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceName("taskever/myName_Space1/MyName_Space2/mynamespace_3/myservice"));
        }

        [Test]
        public void Valid_Service_WithAction_Names()
        {
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/task/create"));
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/tasks/task/update"));
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/taskService/delete"));
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/tasks/taskService/getAllTasks"));
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/tasks/task_service/deleteTask"));
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/myNameSpace1/MyNameSpace2/mynamespace3/myserviceName/CreateNew"));
            Assert.AreEqual(true, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("taskever/myName_Space1/MyName_Space2/mynamespace_3/myservice/test_action_Name"));
        }

        [Test]
        public void Invalid_Service_Names()
        {
            Assert.AreEqual(false, DynamicApiServiceNameHelper.IsValidServiceName(""));
            Assert.AreEqual(false, DynamicApiServiceNameHelper.IsValidServiceName("task"));
            Assert.AreEqual(false, DynamicApiServiceNameHelper.IsValidServiceName("task_service"));
            Assert.AreEqual(false, DynamicApiServiceNameHelper.IsValidServiceName("taskever/task service"));
            Assert.AreEqual(false, DynamicApiServiceNameHelper.IsValidServiceName("taskever/123task_service"));
            Assert.AreEqual(false, DynamicApiServiceNameHelper.IsValidServiceName(" taskever/task_service"));
            Assert.AreEqual(false, DynamicApiServiceNameHelper.IsValidServiceName("taskever/ task_service"));
        }

        [Test]
        public void Invalid_ServiceWithAction_Names()
        {
            Assert.AreEqual(false, DynamicApiServiceNameHelper.IsValidServiceNameWithAction(""));
            Assert.AreEqual(false, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("task"));
            Assert.AreEqual(false, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("task_service"));
            Assert.AreEqual(false, DynamicApiServiceNameHelper.IsValidServiceNameWithAction("task_service/create"));
        }
    }
}
