using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Abp.OpenIddict.Authorizations;
using Abp.OpenIddict.Tokens;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace Abp.OpenIddict.EntityFrameworkCore.Authorizations;

public class EfCoreOpenIddictAuthorizationRepository<TDbContext> :
    EfCoreRepositoryBase<TDbContext, OpenIddictAuthorization, Guid>,
    IOpenIddictAuthorizationRepository
    where TDbContext : DbContext, IOpenIddictDbContext
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public EfCoreOpenIddictAuthorizationRepository(
        IDbContextProvider<TDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager) :
        base(dbContextProvider)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    protected async Task<DbSet<OpenIddictAuthorization>> GetDbSetAsync()
    {
        return (await GetDbContextAsync()).Set<OpenIddictAuthorization>();
    }

    public virtual async Task<List<OpenIddictAuthorization>> FindAsync(string subject, Guid client,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .Where(x => x.Subject == subject && x.ApplicationId == client)
                .ToListAsync(cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictAuthorization>> FindAsync(string subject, Guid client, string status,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .Where(x => x.Subject == subject && x.Status == status && x.ApplicationId == client)
                .ToListAsync(cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictAuthorization>> FindAsync(string subject, Guid client, string status,
        string type, CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .Where(x => x.Subject == subject && x.Status == status && x.Type == type &&
                            x.ApplicationId == client)
                .ToListAsync(cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictAuthorization>> FindByApplicationIdAsync(Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .Where(x => x.ApplicationId == applicationId)
                .ToListAsync(cancellationToken);
        });
    }

    public virtual async Task<OpenIddictAuthorization> FindByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictAuthorization>> FindBySubjectAsync(string subject,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .Where(x => x.Subject == subject)
                .ToListAsync(cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictAuthorization>> ListAsync(int? count, int? offset,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync())
            .OrderBy(authorization => authorization.Id!)
            .AsTracking();

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

    public virtual async Task<long> PruneAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var authorizations = await (from authorization in (await GetQueryableAsync())
            join token in (await GetDbContextAsync()).Set<OpenIddictToken>()
                on authorization.Id equals token.AuthorizationId into authorizationTokens
            from authorizationToken in authorizationTokens.DefaultIfEmpty()
            where authorization.CreationDate < date
            where authorization.Status != OpenIddictConstants.Statuses.Valid ||
                  (authorization.Type == OpenIddictConstants.AuthorizationTypes.AdHoc && authorizationToken == null)
            select authorization.Id).ToListAsync(cancellationToken);

        var count = await (from token in (await GetDbContextAsync()).Set<OpenIddictToken>()
                where token.AuthorizationId != null && authorizations.Contains(token.AuthorizationId.Value)
                select token)
            .ExecuteDeleteAsync(cancellationToken);

        return count + await (await GetDbSetAsync()).Where(x => authorizations.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<long> RevokeAsync(string subject, string client, string status, string type,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        if (!string.IsNullOrEmpty(subject))
        {
            query = query.Where(e => e.Subject == subject);
        }
        
        if (!string.IsNullOrEmpty(client))
        {
            query = query.Where(e => e.ApplicationId == Guid.Parse(client));
        }
        
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(e => e.Status == status);
        }
        
        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(e => e.Type == type);
        }
        
        return await query
            .ExecuteUpdateAsync(
                entity => entity.SetProperty(token => token.Status, OpenIddictConstants.Statuses.Revoked),
                cancellationToken);
    }

    public async Task<long> RevokeByApplicationIdAsync(Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query
            .Where(e => e.ApplicationId == applicationId)
            .ExecuteUpdateAsync(
                entity => entity.SetProperty(token => token.Status, OpenIddictConstants.Statuses.Revoked),
                cancellationToken);
    }

    public async Task<long> RevokeBySubjectAsync(string subject, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query
            .Where(e => e.Subject == subject)
            .ExecuteUpdateAsync(
                entity => entity.SetProperty(token => token.Status, OpenIddictConstants.Statuses.Revoked),
                cancellationToken);
    }
}