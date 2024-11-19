using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.AspNetCore.OpenIddict.Claims;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace Abp.AspNetCore.OpenIddict.Controllers;

[Route("connect/token")]
[IgnoreAntiforgeryToken]
[ApiExplorerSettings(IgnoreApi = true)]
public partial class TokenController<TTenant, TRole, TUser> : AbpOpenIdDictControllerBase<TTenant, TRole, TUser>
    where TTenant : AbpTenant<TUser>
    where TRole : AbpRole<TUser>, new()
    where TUser : AbpUser<TUser>
{
    public TokenController(
        AbpSignInManager<TTenant, TRole, TUser> signInManager,
        AbpUserManager<TRole, TUser> userManager,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        IOpenIddictTokenManager tokenManager,
        AbpOpenIddictClaimsPrincipalManager openIddictClaimsPrincipalManager) :
        base(
            signInManager,
            userManager,
            applicationManager,
            authorizationManager,
            scopeManager,
            tokenManager,
            openIddictClaimsPrincipalManager
        )
    {
    }

    [HttpGet, HttpPost, Produces("application/json")]
    public async Task<IActionResult> HandleAsync()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request == null)
        {
            throw new InvalidOperationException("The OpenIDConnect request cannot retrieved!");
        }

        if (request.IsPasswordGrantType())
        {
            return await HandlePasswordAsync(request);
        }

        if (request.IsAuthorizationCodeGrantType())
        {
            return await HandleAuthorizationCodeAsync(request);
        }

        throw new AbpException($"The specified grant type {request.GrantType} is not implemented!");
    }

    private int? FindTenantId(ClaimsPrincipal principal)
    {
        Check.NotNull(principal, nameof(principal));

        var tenantIdOrNull = principal.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.TenantId);
        if (tenantIdOrNull == null || tenantIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }

        if (int.TryParse(tenantIdOrNull.Value, out var guid))
        {
            return guid;
        }

        return null;
    }
}