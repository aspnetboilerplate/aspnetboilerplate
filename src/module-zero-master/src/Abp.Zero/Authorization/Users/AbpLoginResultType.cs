namespace Abp.Authorization.Users
{
    public enum AbpLoginResultType
    {
        Success = 1,

        InvalidUserNameOrEmailAddress,
        
        InvalidPassword,
        
        UserIsNotActive,

        InvalidTenancyName,
        
        TenantIsNotActive,

        UserEmailIsNotConfirmed,

        UnknownExternalLogin
    }
}