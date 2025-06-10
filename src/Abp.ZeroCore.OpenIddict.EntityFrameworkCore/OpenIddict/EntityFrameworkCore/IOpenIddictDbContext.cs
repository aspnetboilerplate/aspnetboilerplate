using Abp.OpenIddict.Applications;
using Abp.OpenIddict.Authorizations;
using Abp.OpenIddict.Scopes;
using Abp.OpenIddict.Tokens;
using Microsoft.EntityFrameworkCore;

namespace Abp.OpenIddict.EntityFrameworkCore;

public interface IOpenIddictDbContext
{
    DbSet<OpenIddictApplication> Applications { get; }

    DbSet<OpenIddictAuthorization> Authorizations { get; }

    DbSet<OpenIddictScope> Scopes { get; }

    DbSet<OpenIddictToken> Tokens { get; }
}