using Microsoft.EntityFrameworkCore;

namespace Abp.IdentityServer4
{
    public static class AbpZeroCoreIdentityServerEntityFrameworkCoreConfigurationExtensions
    {
        public static void ConfigurePersistedGrantEntity(this ModelBuilder modelBuilder, string prefix = null, string schemaName = null)
        {
            prefix ??= "Abp";
            var tableName = prefix + "PersistedGrants";

            modelBuilder.Entity<PersistedGrantEntity>(grant =>
            {
                if (schemaName == null)
                {
                    grant.ToTable(tableName);
                }
                else
                {
                    grant.ToTable(tableName, schemaName);
                }

                grant.Property(x => x.Id).HasMaxLength(200).ValueGeneratedNever();
                grant.Property(x => x.Type).HasMaxLength(50).IsRequired();
                grant.Property(x => x.SubjectId).HasMaxLength(200);
                grant.Property(x => x.SessionId).HasMaxLength(100);
                grant.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
                grant.Property(x => x.Description).HasMaxLength(200);
                grant.Property(x => x.CreationTime).IsRequired();
                // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
                // apparently anything over 4K converts to nvarchar(max) on SqlServer
                grant.Property(x => x.Data).HasMaxLength(50000).IsRequired();

                grant.HasKey(x => x.Id);

                grant.HasIndex(x => new { x.SubjectId, x.ClientId, x.Type });
                grant.HasIndex(x => new { x.SubjectId, x.SessionId, x.Type });
                grant.HasIndex(x => x.Expiration);
            });
        }
    }
}