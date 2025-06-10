using System;
using System.Security.Claims;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Abp.Authorization.Users;

public class AbpLoginResult<TTenant, TUser>
    where TTenant : AbpTenant<TUser>
    where TUser : AbpUserBase
{
    public AbpLoginResultType Result { get; private set; }

    public ILocalizableString FailReason { get; private set; }

    public TTenant Tenant { get; private set; }

    public TUser User { get; private set; }

    public ClaimsIdentity Identity { get; private set; }

    public AbpLoginResult(AbpLoginResultType result, TTenant tenant = null, TUser user = null)
    {
        Result = result;
        Tenant = tenant;
        User = user;
    }

    public AbpLoginResult(TTenant tenant, TUser user, ClaimsIdentity identity)
        : this(AbpLoginResultType.Success, tenant)
    {
        User = user;
        Identity = identity;
    }

    /// <summary>
    /// This method can be used only when <see cref="Result"/> is <see cref="AbpLoginResultType.FailedForOtherReason"/>.
    /// </summary>
    /// <param name="failReason">Localizable fail reason message</param>
    public void SetFailReason(ILocalizableString failReason)
    {
        if (Result != AbpLoginResultType.FailedForOtherReason)
        {
            throw new Exception($"Can not set fail reason for result type {Result}, use this method only for AbpLoginResultType.FailedForOtherReason result type!");
        }

        FailReason = failReason;
    }

    public string GetFailReason(ILocalizationContext localizationContext)
    {
        return FailReason == null ? string.Empty : FailReason?.Localize(localizationContext);
    }
}