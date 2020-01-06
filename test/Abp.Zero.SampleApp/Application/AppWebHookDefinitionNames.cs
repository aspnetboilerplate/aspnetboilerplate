namespace Abp.Zero.SampleApp.Application
{
    public class AppWebHookDefinitionNames
    {
        public const string Test = "test";

        public class Users
        {
            public const string Created = "user_created";
        }

        public class Tenant
        {
            public const string Deleted = "tenant_deleted";
        }

        public class Chat
        {
            public const string NewMessageReceived = "chat.new_message_received";
        }
    }
}
