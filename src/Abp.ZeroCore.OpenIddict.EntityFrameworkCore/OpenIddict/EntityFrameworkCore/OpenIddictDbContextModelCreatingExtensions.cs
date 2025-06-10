using Abp.OpenIddict.Applications;
using Abp.OpenIddict.Authorizations;
using Abp.OpenIddict.Scopes;
using Abp.OpenIddict.Tokens;
using Microsoft.EntityFrameworkCore;

namespace Abp.OpenIddict.EntityFrameworkCore;

public static class OpenIddictDbContextModelCreatingExtensions
{
    public static void ConfigureOpenIddict(
        this ModelBuilder builder)
    {
        var dbTablePrefix = "OpenIddict";
        string dbSchema = null;

        Check.NotNull(builder, nameof(builder));

        builder.Entity<OpenIddictApplication>(b =>
        {
            b.ToTable(dbTablePrefix + "Applications", dbSchema);

            b.HasIndex(x => x.ClientId);

            b.Property(x => x.ClientId)
                .HasMaxLength(OpenIddictApplicationConsts.ClientIdMaxLength);

            b.Property(x => x.ApplicationType)
            .HasMaxLength(OpenIddictApplicationConsts.ApplicationTypeMaxLength);

            b.Property(x => x.ConsentType)
                .HasMaxLength(OpenIddictApplicationConsts.ConsentTypeMaxLength);

            b.Property(x => x.ClientType)
                .HasMaxLength(OpenIddictApplicationConsts.ClientTypeMaxLength);
        });

        builder.Entity<OpenIddictAuthorization>(b =>
        {
            b.ToTable(dbTablePrefix + "Authorizations",
                dbSchema);

            b.HasIndex(x => new
            {
                x.ApplicationId,
                x.Status,
                x.Subject,
                x.Type
            });

            b.Property(x => x.Status)
                .HasMaxLength(OpenIddictAuthorizationConsts.StatusMaxLength);

            b.Property(x => x.Subject)
                .HasMaxLength(OpenIddictAuthorizationConsts.SubjectMaxLength);

            b.Property(x => x.Type)
                .HasMaxLength(OpenIddictAuthorizationConsts.TypeMaxLength);

            b.HasOne<OpenIddictApplication>().WithMany().HasForeignKey(x => x.ApplicationId).IsRequired(false);
        });

        builder.Entity<OpenIddictScope>(b =>
        {
            b.ToTable(dbTablePrefix + "Scopes", dbSchema);

            b.HasIndex(x => x.Name);

            b.Property(x => x.Name)
                .HasMaxLength(OpenIddictScopeConsts.NameMaxLength);
        });

        builder.Entity<OpenIddictToken>(b =>
        {
            b.ToTable(dbTablePrefix + "Tokens", dbSchema);

            b.HasIndex(x => x.ReferenceId);

            b.HasIndex(x => new
            {
                x.ApplicationId,
                x.Status,
                x.Subject,
                x.Type
            });

            b.Property(x => x.ReferenceId)
                .HasMaxLength(OpenIddictTokenConsts.ReferenceIdMaxLength);

            b.Property(x => x.Status)
                .HasMaxLength(OpenIddictTokenConsts.StatusMaxLength);

            b.Property(x => x.Subject)
                .HasMaxLength(OpenIddictTokenConsts.SubjectMaxLength);

            b.Property(x => x.Type)
                .HasMaxLength(OpenIddictTokenConsts.TypeMaxLength);

            b.HasOne<OpenIddictApplication>().WithMany().HasForeignKey(x => x.ApplicationId).IsRequired(false);
            b.HasOne<OpenIddictAuthorization>().WithMany().HasForeignKey(x => x.AuthorizationId).IsRequired(false);
        });
    }
}

public class OpenIddictApplicationConsts
{
    public static int ApplicationTypeMaxLength { get; set; } = 50;
    public static int ClientIdMaxLength { get; set; } = 100;

    public static int ConsentTypeMaxLength { get; set; } = 50;

    public static int ClientTypeMaxLength { get; set; } = 50;
}

public class OpenIddictAuthorizationConsts
{
    public static int StatusMaxLength { get; set; } = 50;

    public static int SubjectMaxLength { get; set; } = 400;

    public static int TypeMaxLength { get; set; } = 50;
}

public class OpenIddictScopeConsts
{
    public static int NameMaxLength { get; set; } = 200;
}

public class OpenIddictTokenConsts
{
    public static int ReferenceIdMaxLength { get; set; } = 100;

    public static int StatusMaxLength { get; set; } = 50;

    public static int SubjectMaxLength { get; set; } = 400;

    public static int TypeMaxLength { get; set; } = 50;
}