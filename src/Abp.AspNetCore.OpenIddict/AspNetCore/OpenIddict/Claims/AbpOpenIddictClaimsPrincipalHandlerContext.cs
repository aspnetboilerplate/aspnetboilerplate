using System;
using System.Security.Claims;
using OpenIddict.Abstractions;

namespace Abp.AspNetCore.OpenIddict.Claims;

public class AbpOpenIddictClaimsPrincipalHandlerContext
{
    public IServiceProvider ScopeServiceProvider { get; }

    public OpenIddictRequest OpenIddictRequest { get; }

    public ClaimsPrincipal Principal { get; }

    public AbpOpenIddictClaimsPrincipalHandlerContext(IServiceProvider scopeServiceProvider, OpenIddictRequest openIddictRequest, ClaimsPrincipal principal)
    {
        ScopeServiceProvider = scopeServiceProvider;
        OpenIddictRequest = openIddictRequest;
        Principal = principal;
    }
}