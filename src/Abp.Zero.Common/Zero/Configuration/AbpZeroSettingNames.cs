namespace Abp.Zero.Configuration
{
    public static class AbpZeroSettingNames
    {
        public static class UserManagement
        {
            /// <summary>
            /// "Abp.Zero.UserManagement.IsEmailConfirmationRequiredForLogin".
            /// </summary>
            public const string IsEmailConfirmationRequiredForLogin = "Abp.Zero.UserManagement.IsEmailConfirmationRequiredForLogin";

            public static class UserLockOut
            {
                /// <summary>
                /// "Abp.Zero.UserManagement.UserLockOut.IsEnabled".
                /// </summary>
                public const string IsEnabled = "Abp.Zero.UserManagement.UserLockOut.IsEnabled";

                /// <summary>
                /// "Abp.Zero.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout".
                /// </summary>
                public const string MaxFailedAccessAttemptsBeforeLockout = "Abp.Zero.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout";

                /// <summary>
                /// "Abp.Zero.UserManagement.UserLockOut.DefaultAccountLockoutSeconds".
                /// </summary>
                public const string DefaultAccountLockoutSeconds = "Abp.Zero.UserManagement.UserLockOut.DefaultAccountLockoutSeconds";
            }

            public static class TwoFactorLogin
            {
                /// <summary>
                /// "Abp.Zero.UserManagement.TwoFactorLogin.IsEnabled".
                /// </summary>
                public const string IsEnabled = "Abp.Zero.UserManagement.TwoFactorLogin.IsEnabled";

                /// <summary>
                /// "Abp.Zero.UserManagement.TwoFactorLogin.IsEmailProviderEnabled".
                /// </summary>
                public const string IsEmailProviderEnabled = "Abp.Zero.UserManagement.TwoFactorLogin.IsEmailProviderEnabled";

                /// <summary>
                /// "Abp.Zero.UserManagement.TwoFactorLogin.IsSmsProviderEnabled".
                /// </summary>
                public const string IsSmsProviderEnabled = "Abp.Zero.UserManagement.TwoFactorLogin.IsSmsProviderEnabled";

                /// <summary>
                /// "Abp.Zero.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled".
                /// </summary>
                public const string IsRememberBrowserEnabled = "Abp.Zero.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled";
            }

            public static class PasswordComplexity
            {
                /// <summary>
                /// "Abp.Zero.UserManagement.PasswordComplexity.RequiredLength"
                /// </summary>
                public const string RequiredLength = "Abp.Zero.UserManagement.PasswordComplexity.RequiredLength";

                /// <summary>
                /// "Abp.Zero.UserManagement.PasswordComplexity.RequireNonAlphanumeric"
                /// </summary>
                public const string RequireNonAlphanumeric = "Abp.Zero.UserManagement.PasswordComplexity.RequireNonAlphanumeric";

                /// <summary>
                /// "Abp.Zero.UserManagement.PasswordComplexity.RequireLowercase"
                /// </summary>
                public const string RequireLowercase = "Abp.Zero.UserManagement.PasswordComplexity.RequireLowercase";

                /// <summary>
                /// "Abp.Zero.UserManagement.PasswordComplexity.RequireUppercase"
                /// </summary>
                public const string RequireUppercase = "Abp.Zero.UserManagement.PasswordComplexity.RequireUppercase";

                /// <summary>
                /// "Abp.Zero.UserManagement.PasswordComplexity.RequireDigit"
                /// </summary>
                public const string RequireDigit = "Abp.Zero.UserManagement.PasswordComplexity.RequireDigit";
            }
        }

        public static class OrganizationUnits
        {
            /// <summary>
            /// "Abp.Zero.OrganizationUnits.MaxUserMembershipCount".
            /// </summary>
            public const string MaxUserMembershipCount = "Abp.Zero.OrganizationUnits.MaxUserMembershipCount";
        }
    }
}