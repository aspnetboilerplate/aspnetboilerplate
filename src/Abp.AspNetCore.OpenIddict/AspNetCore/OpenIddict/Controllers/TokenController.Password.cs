using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.OpenIddict.Claims;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;


namespace Abp.AspNetCore.OpenIddict.Controllers;

public partial class TokenController<TTenant, TRole, TUser>
{
    protected virtual async Task<IActionResult> HandlePasswordAsync(OpenIddictRequest request)
    {
        // TODO@OIDDICT: Should we use session here to switch to target tenant ?

        var session = IocManager.Instance.Resolve<IAbpSession>();
        var uowManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
        var userManager = IocManager.Instance.Resolve<AbpUserManager<TRole, TUser>>();
        var signInManager = IocManager.Instance.Resolve<AbpSignInManager<TTenant, TRole, TUser>>();

        return await uowManager.WithUnitOfWorkAsync(async () =>
        {
            var user = await userManager.FindByNameAsync(request.Username);
            if (user != null)
            {
                var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
                if (!result.Succeeded)
                {
                    string errorDescription;
                    if (result.IsLockedOut)
                    {
                        Logger.Warn(Strings.Format(
                            "Authentication failed for username: {username}, reason: locked out",
                            request.Username
                        ));
                        errorDescription =
                            "The user account has been locked out due to invalid login attempts. Please wait a while and try again.";
                    }
                    else if (result.IsNotAllowed)
                    {
                        Logger.Warn(string.Format(
                            "Authentication failed for username: {username}, reason: not allowed",
                            request.Username));
                        errorDescription =
                            "You are not allowed to login! Your account is inactive or needs to confirm your email/phone number.";
                    }
                    else
                    {
                        Logger.Warn(string.Format(
                            "Authentication failed for username: {username}, reason: invalid credentials",
                            request.Username
                        ));
                        errorDescription = "Invalid username or password!";
                    }

                    var properties = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = errorDescription
                    });

                    return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                return await SetSuccessResultAsync(request, user);
            }

            Logger.Error(string.Format("No user found matching username: {username}", request.Username));

            return Forbid(new AuthenticationProperties(new Dictionary<string, string>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                    OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "Invalid username or password!"
            }), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        });
    }

    protected virtual async Task<IActionResult> SetSuccessResultAsync(OpenIddictRequest request, TUser user)
    {
        var signInManager = IocManager.Instance.Resolve<AbpSignInManager<TTenant, TRole, TUser>>();
        var openIddictClaimsPrincipalManager = IocManager.Instance.Resolve<AbpOpenIddictClaimsPrincipalManager>();

        // Create a new ClaimsPrincipal containing the claims that
        // will be used to create an id_token, a token or a code.
        var principal = await signInManager.CreateUserPrincipalAsync(user);

        principal.SetScopes(request.GetScopes());
        principal.SetResources(await GetResourcesAsync(request.GetScopes()));

        // different from abp.io
        // ----------------------------------------------------------------------------------
        principal.SetClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString());
        if (user.TenantId.HasValue)
        {
            principal.SetClaim(AbpClaimTypes.TenantId, user.TenantId?.ToString());
        }
        // ----------------------------------------------------------------------------------

        await openIddictClaimsPrincipalManager.HandleAsync(request, principal);

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    protected override async Task<IEnumerable<string>> GetResourcesAsync(ImmutableArray<string> scopes)
    {
        var resources = new List<string>();
        if (!scopes.Any())
        {
            return resources;
        }

        var scopeManager = IocManager.Instance.Resolve<IOpenIddictScopeManager>();

        await foreach (var resource in scopeManager.ListResourcesAsync(scopes))
        {
            resources.Add(resource);
        }
        return resources;
    }
}