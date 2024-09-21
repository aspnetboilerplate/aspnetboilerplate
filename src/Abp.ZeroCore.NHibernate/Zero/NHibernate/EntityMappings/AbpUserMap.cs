using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.NHibernate.EntityMappings;

namespace Abp.Zero.NHibernate.EntityMappings;

public abstract class AbpUserMap<TUser> : EntityMap<TUser, long>
    where TUser : AbpUser<TUser>
{
    protected AbpUserMap() : base("AbpUsers")
    {
        Map(x => x.AccessFailedCount)
            .Not.Nullable();
        Map(x => x.AuthenticationSource)
            .Length(AbpUserBase.MaxAuthenticationSourceLength);
        Map(x => x.ConcurrencyStamp)
            .Length(AbpUser<TUser>.MaxConcurrencyStampLength);
        Map(x => x.EmailAddress)
            .Length(AbpUserBase.MaxEmailAddressLength)
            .Not.Nullable();
        Map(x => x.EmailConfirmationCode)
            .Length(AbpUserBase.MaxEmailConfirmationCodeLength);
        Map(x => x.IsActive)
            .Not.Nullable();
        Map(x => x.IsEmailConfirmed)
            .Not.Nullable();
        Map(x => x.IsLockoutEnabled)
            .Not.Nullable();
        Map(x => x.IsPhoneNumberConfirmed)
            .Not.Nullable();
        Map(x => x.IsTwoFactorEnabled)
            .Not.Nullable();
        Map(x => x.LockoutEndDateUtc);
        Map(x => x.Name)
            .Length(AbpUserBase.MaxNameLength)
            .Not.Nullable();
        Map(x => x.NormalizedEmailAddress)
            .Length(AbpUserBase.MaxEmailAddressLength)
            .Not.Nullable();
        Map(x => x.NormalizedUserName)
            .Length(AbpUserBase.MaxUserNameLength)
            .Not.Nullable();
        Map(x => x.Password)
            .Length(AbpUserBase.MaxPasswordLength)
            .Not.Nullable();
        Map(x => x.PasswordResetCode)
            .Length(AbpUserBase.MaxPasswordResetCodeLength);
        Map(x => x.PhoneNumber)
            .Length(AbpUserBase.MaxPhoneNumberLength);
        Map(x => x.SecurityStamp)
            .Length(AbpUserBase.MaxSecurityStampLength);
        Map(x => x.Surname)
            .Length(AbpUserBase.MaxSurnameLength)
            .Not.Nullable();
        Map(x => x.TenantId);
        Map(x => x.UserName)
            .Length(AbpUserBase.MaxUserNameLength)
            .Not.Nullable();


        HasMany(x => x.Tokens)
            .Inverse()
            .Cascade.AllDeleteOrphan()
            .KeyColumn(nameof(UserToken.UserId));
        HasMany(x => x.Logins)
            .Inverse()
            .Cascade.AllDeleteOrphan()
            .KeyColumn(nameof(UserLogin.UserId));
        HasMany(x => x.Roles)
            .Inverse()
            .Cascade.AllDeleteOrphan()
            .KeyColumn(nameof(UserRole.UserId));
        HasMany(x => x.Claims)
            .Inverse()
            .Cascade.AllDeleteOrphan()
            .KeyColumn(nameof(UserClaim.UserId));
        HasMany(x => x.Permissions)
            .Inverse()
            .Cascade.AllDeleteOrphan()
            .KeyColumn(nameof(UserPermissionSetting.UserId));
        HasMany(x => x.Settings)
            .Inverse()
            .Cascade.AllDeleteOrphan()
            .KeyColumn(nameof(Setting.UserId));

        this.MapFullAudited();

        Polymorphism.Explicit();
    }
}