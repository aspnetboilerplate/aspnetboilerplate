namespace Abp.Zero.SampleApp.Application
{
    public class AppWebhookDefinitionNames
    {
        public const string Test = "test";

        public class Users
        {
            public const string Created = "user.new_user_created";
            public const string Deleted = "user.user_deleted";
        }

        public class Theme
        {
            public const string DefaultThemeChanged = "theme.default_theme_changed";
        }
    }
}
