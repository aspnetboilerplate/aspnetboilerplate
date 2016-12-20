using Abp.Json;
using Abp.Localization;
using Abp.Notifications;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Abp.Tests.Notifications
{
    public class NotificationData_Serialization_Tests
    {
        [Fact]
        public void Should_Deserialize_And_Serialize_MessageNotificationData()
        {
            var data = JsonConvert
                .DeserializeObject(
                    new MessageNotificationData("Hello World!").ToJsonString(),
                    typeof(MessageNotificationData)
                ) as MessageNotificationData;

            Assert.NotNull(data);
            data.Message.ShouldBe("Hello World!");
        }

        [Fact]
        public void Should_Deserialize_And_Serialize_LocalizableMessageNotificationData()
        {
            var serialized = new LocalizableMessageNotificationData(new LocalizableString("Hello", "MySource")).ToJsonString();

            var data = JsonConvert
                .DeserializeObject(
                    serialized,
                    typeof(LocalizableMessageNotificationData)
                ) as LocalizableMessageNotificationData;

            Assert.NotNull(data);
            Assert.NotNull(data.Message);
            data.Message.Name.ShouldBe("Hello");
            data.Message.SourceName.ShouldBe("MySource");
        }
    }
}
