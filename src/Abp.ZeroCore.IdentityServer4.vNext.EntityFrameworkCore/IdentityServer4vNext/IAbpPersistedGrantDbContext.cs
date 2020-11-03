using Microsoft.EntityFrameworkCore;

namespace Abp.IdentityServer4vNext
{
    public interface IAbpPersistedGrantDbContext
    {
        DbSet<PersistedGrantEntity> PersistedGrants { get; set; }
    }
}
