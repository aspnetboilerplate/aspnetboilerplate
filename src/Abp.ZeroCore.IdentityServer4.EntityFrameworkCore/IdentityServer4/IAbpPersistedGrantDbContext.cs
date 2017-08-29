using Microsoft.EntityFrameworkCore;

namespace Abp.IdentityServer4
{
    public interface IAbpPersistedGrantDbContext
    {
        DbSet<PersistedGrantEntity> PersistedGrants { get; set; }
    }
}