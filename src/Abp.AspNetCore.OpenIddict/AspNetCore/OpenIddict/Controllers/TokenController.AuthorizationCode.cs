using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Dependency;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Abp.AspNetCore.OpenIddict.Controllers;

public partial class TokenController<TTenant, TRole, TUser>
{
    protected virtual async Task<IActionResult> HandleAuthorizationCodeAsync(OpenIddictRequest request)
    {
        var userManager = IocManager.Instance.Resolve<AbpUserManager<TRole, TUser>>();

        // Retrieve the claims principal stored in the authorization code/device code/refresh token.
        var principal = (await HttpContext.AuthenticateAsync(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        )).Principal;

        using (CurrentUnitOfWork.SetTenantId(FindTenantId(principal)))
        {
            // Retrieve the user profile corresponding to the authorization code/refresh token.
            // Note: if you want to automatically invalidate the authorization code/refresh token
            // when the user password/roles change, use the following line instead:
            // var user = _signInManager.ValidateSecurityStampAsync(info.Principal);
            var user = await userManager.GetUserAsync(principal);
            if (user == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The token is no longer valid."
                    }));
            }

            // Ensure the user is still allowed to sign in.
            if (!await PreSignInCheckAsync(user))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The user is no longer allowed to sign in."
                    }));
            }

            await OpenIddictClaimsPrincipalManager.HandleAsync(request, principal);

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}