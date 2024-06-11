using Abp.Collections;

namespace Abp.AspNetCore.OpenIddict.Claims;

public class AbpOpenIddictClaimsPrincipalOptions
{
    public ITypeList<IAbpOpenIddictClaimsPrincipalHandler> ClaimsPrincipalHandlers { get; }

    public AbpOpenIddictClaimsPrincipalOptions()
    {
        ClaimsPrincipalHandlers = new TypeList<IAbpOpenIddictClaimsPrincipalHandler>();
    }
}