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
using Abp.OpenIddict.Scopes;
using Microsoft.EntityFrameworkCore;

namespace Abp.OpenIddict.EntityFrameworkCore.Scopes;

public class EfCoreOpenIddictScopeRepository<TDbContext> : EfCoreRepositoryBase<TDbContext, OpenIddictScope, Guid>,
    IOpenIddictScopeRepository
    where TDbContext : DbContext, IOpenIddictDbContext
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public EfCoreOpenIddictScopeRepository(
        IDbContextProvider<TDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager)
        : base(dbContextProvider)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    protected async Task<DbSet<OpenIddictScope>> GetDbSetAsync()
    {
        return (await GetDbContextAsync()).Set<OpenIddictScope>();
    }

    public async Task<List<OpenIddictScope>> GetListAsync(string sorting, int skipCount, int maxResultCount,
        string filter = null,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x =>
                    x.Name.Contains(filter) ||
                    x.DisplayName.Contains(filter) ||
                    x.Description.Contains(filter))
                .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(OpenIddictScope.Name) : sorting)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(cancellationToken);
        });
    }

    public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x =>
                    x.Name.Contains(filter) ||
                    x.DisplayName.Contains(filter) ||
                    x.Description.Contains(filter))
                .LongCountAsync(cancellationToken);
        });
    }

    public virtual async Task<OpenIddictScope> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Id == id,
                cancellationToken);
        });
    }

    public virtual async Task<OpenIddictScope> FindByNameAsync(string name,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetQueryableAsync())
                .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictScope>> FindByNamesAsync(string[] names,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetQueryableAsync()).Where(x => names.Contains(x.Name))
                .ToListAsync(cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictScope>> FindByResourceAsync(string resource,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetQueryableAsync()).Where(x => x.Resources.Contains(resource))
                .ToListAsync(cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictScope>> ListAsync(int? count, int? offset,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        if (offset.HasValue)
        {
            query = query.Skip(offset.Value);
        }

        if (count.HasValue)
        {
            query = query.Take(count.Value);
        }

        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await query.ToListAsync(cancellationToken);
        });
    }
}