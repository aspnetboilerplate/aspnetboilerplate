using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.OpenIddict.Applications;
using Microsoft.EntityFrameworkCore;

namespace Abp.OpenIddict.EntityFrameworkCore.Applications;

public class EfCoreOpenIddictApplicationRepository<TDbContext> : EfCoreRepositoryBase<TDbContext, OpenIddictApplication, Guid>, IOpenIddictApplicationRepository
    where TDbContext : DbContext, IOpenIddictDbContext
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public EfCoreOpenIddictApplicationRepository(
        IDbContextProvider<TDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager) :
        base(dbContextProvider)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    protected async Task<DbSet<OpenIddictApplication>> GetDbSetAsync()
    {
        return (await GetDbContextAsync()).Set<OpenIddictApplication>();
    }

    public async Task<List<OpenIddictApplication>> GetListAsync(string sorting, int skipCount, int maxResultCount,
        string filter = null,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.ClientId.Contains(filter))
                .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(OpenIddictApplication.ClientId) : sorting)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(cancellationToken);
        });
    }

    public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.ClientId.Contains(filter))
                .LongCountAsync(cancellationToken);
        });
    }

    public virtual async Task<OpenIddictApplication> FindByClientIdAsync(string clientId,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .FirstOrDefaultAsync(x => x.ClientId == clientId, cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictApplication>> FindByPostLogoutRedirectUriAsync(string address,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .Where(x => x.PostLogoutRedirectUris.Contains(address))
                .ToListAsync(cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictApplication>> FindByRedirectUriAsync(string address,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .Where(x => x.RedirectUris.Contains(address)).ToListAsync(cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictApplication>> ListAsync(int? count, int? offset,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetQueryableAsync());
        query = query.OrderBy(x => x.Id);

        if (offset.HasValue)
        {
            query = query.Skip(offset.Value);
        }

        if (count.HasValue)
        {
            query = query.Take(count.Value);
        }

        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () => await query.ToListAsync(cancellationToken));
    }
}