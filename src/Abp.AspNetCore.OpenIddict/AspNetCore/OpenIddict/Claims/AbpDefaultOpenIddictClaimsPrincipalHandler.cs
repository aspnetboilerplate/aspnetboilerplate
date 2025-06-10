using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using OpenIddict.Abstractions;

namespace Abp.AspNetCore.OpenIddict.Claims;

public class AbpDefaultOpenIddictClaimsPrincipalHandler : IAbpOpenIddictClaimsPrincipalHandler, ITransientDependency
{
    public virtual Task HandleAsync(AbpOpenIddictClaimsPrincipalHandlerContext context)
    {
        var securityStampClaimType = context
            .ScopeServiceProvider
            .GetRequiredService<IOptions<IdentityOptions>>().Value
            .ClaimsIdentity.SecurityStampClaimType;

        AddSubClaim(context);

        foreach (var claim in context.Principal.Claims)
        {
            if (claim.Type == AbpClaimTypes.TenantId)
            {
                claim.SetDestinations(
                    OpenIddictConstants.Destinations.AccessToken,
                    OpenIddictConstants.Destinations.IdentityToken
                );
                continue;
            }

            switch (claim.Type)
            {
                case OpenIddictConstants.Claims.PreferredUsername:
                    claim.SetDestinations(OpenIddictConstants.Destinations.AccessToken);
                    if (context.Principal.HasScope(OpenIddictConstants.Scopes.Profile))
                    {
                        claim.SetDestinations(
                            OpenIddictConstants.Destinations.AccessToken,
                            OpenIddictConstants.Destinations.IdentityToken
                        );
                    }

                    break;

                case JwtRegisteredClaimNames.UniqueName:
                    claim.SetDestinations(OpenIddictConstants.Destinations.AccessToken);
                    if (context.Principal.HasScope(OpenIddictConstants.Scopes.Profile))
                    {
                        claim.SetDestinations(
                            OpenIddictConstants.Destinations.AccessToken,
                            OpenIddictConstants.Destinations.IdentityToken
                        );
                    }

                    break;

                case OpenIddictConstants.Claims.Email:
                    claim.SetDestinations(OpenIddictConstants.Destinations.AccessToken);
                    if (context.Principal.HasScope(OpenIddictConstants.Scopes.Email))
                    {
                        claim.SetDestinations(
                            OpenIddictConstants.Destinations.AccessToken,
                            OpenIddictConstants.Destinations.IdentityToken
                        );
                    }

                    break;

                case OpenIddictConstants.Claims.Role:
                    claim.SetDestinations(OpenIddictConstants.Destinations.AccessToken);
                    if (context.Principal.HasScope(OpenIddictConstants.Scopes.Roles))
                    {
                        claim.SetDestinations(
                            OpenIddictConstants.Destinations.AccessToken,
                            OpenIddictConstants.Destinations.IdentityToken
                        );
                    }

                    break;

                default:
                    // Never include the security stamp in the access and identity tokens, as it's a secret value.
                    if (claim.Type != securityStampClaimType)
                    {
                        claim.SetDestinations(OpenIddictConstants.Destinations.AccessToken);
                    }

                    break;
            }
        }

        return Task.CompletedTask;
    }

    protected virtual void AddSubClaim(AbpOpenIddictClaimsPrincipalHandlerContext context)
    {
        var nameIdClaim = context.Principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);

        if (context.Principal.Claims.All(c => c.Type != JwtRegisteredClaimNames.Sub))
        {
            context.Principal.AddClaim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value);
        }
    }
}