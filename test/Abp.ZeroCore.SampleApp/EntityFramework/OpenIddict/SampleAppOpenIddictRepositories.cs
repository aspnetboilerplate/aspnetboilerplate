using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.OpenIddict.EntityFrameworkCore.Applications;
using Abp.OpenIddict.EntityFrameworkCore.Authorizations;
using Abp.OpenIddict.EntityFrameworkCore.Scopes;
using Abp.OpenIddict.EntityFrameworkCore.Tokens;

namespace Abp.ZeroCore.SampleApp.EntityFramework.OpenIddict;

public class OpenIddictApplicationRepository : EfCoreOpenIddictApplicationRepository<SampleAppDbContext>
{
    public OpenIddictApplicationRepository(
        IDbContextProvider<SampleAppDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager)
        : base(dbContextProvider, unitOfWorkManager)
    {
    }
}

public class OpenIddictAuthorizationRepository : EfCoreOpenIddictAuthorizationRepository<SampleAppDbContext>
{
    public OpenIddictAuthorizationRepository(
        IDbContextProvider<SampleAppDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager)
        : base(dbContextProvider, unitOfWorkManager)
    {
    }
}

public class OpenIddictScopeRepository : EfCoreOpenIddictScopeRepository<SampleAppDbContext>
{
    public OpenIddictScopeRepository(
        IDbContextProvider<SampleAppDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager)
        : base(dbContextProvider, unitOfWorkManager)
    {
    }
}

public class OpenIddictTokenRepository : EfCoreOpenIddictTokenRepository<SampleAppDbContext>
{
    public OpenIddictTokenRepository(
        IDbContextProvider<SampleAppDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager)
        : base(dbContextProvider, unitOfWorkManager)
    {
    }
}
