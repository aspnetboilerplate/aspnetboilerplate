using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.AspNetCore.OpenIddict.Claims;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace Abp.AspNetCore.OpenIddict.Controllers;

public abstract class AbpOpenIdDictControllerBase<TTenant, TRole, TUser> : AbpController
    where TTenant : AbpTenant<TUser>
    where TRole : AbpRole<TUser>, new()
    where TUser : AbpUser<TUser>
{
    protected readonly AbpSignInManager<TTenant, TRole, TUser> SignInManager;
    protected readonly AbpUserManager<TRole, TUser> UserManager;
    protected readonly IOpenIddictApplicationManager ApplicationManager;
    protected readonly IOpenIddictAuthorizationManager AuthorizationManager;
    protected readonly IOpenIddictScopeManager ScopeManager;
    protected readonly IOpenIddictTokenManager TokenManager;
    protected readonly AbpOpenIddictClaimsPrincipalManager OpenIddictClaimsPrincipalManager;

    protected AbpOpenIdDictControllerBase(
        AbpSignInManager<TTenant, TRole, TUser> signInManager,
        AbpUserManager<TRole, TUser> userManager,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        IOpenIddictTokenManager tokenManager,
        AbpOpenIddictClaimsPrincipalManager openIddictClaimsPrincipalManager)
    {
        SignInManager = signInManager;
        UserManager = userManager;
        ApplicationManager = applicationManager;
        AuthorizationManager = authorizationManager;
        ScopeManager = scopeManager;
        TokenManager = tokenManager;
        OpenIddictClaimsPrincipalManager = openIddictClaimsPrincipalManager;

        // TODO@OpenIddict: Handle this !!!
        // LocalizationSourceName = AbpZeroTemplateConsts.LocalizationSourceName;
    }

    protected virtual Task<OpenIddictRequest> GetOpenIddictServerRequestAsync(HttpContext httpContext)
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException(L("TheOpenIDConnectRequestCannotBeRetrieved"));

        return Task.FromResult(request);
    }

    protected virtual async Task<IEnumerable<string>> GetResourcesAsync(ImmutableArray<string> scopes)
    {
        var resources = new List<string>();
        if (!scopes.Any())
        {
            return resources;
        }

        await foreach (var resource in ScopeManager.ListResourcesAsync(scopes))
        {
            resources.Add(resource);
        }

        return resources;
    }

    protected virtual async Task<bool> HasFormValueAsync(string name)
    {
        if (Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync();
            if (!string.IsNullOrEmpty(form[name]))
            {
                return true;
            }
        }

        return false;
    }

    protected virtual async Task<bool> PreSignInCheckAsync(TUser user)
    {
        if (!await SignInManager.CanSignInAsync(user))
        {
            return false;
        }

        if (await UserManager.IsLockedOutAsync(user))
        {
            return false;
        }

        return true;
    }
}