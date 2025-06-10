using Abp.IdentityServer4vNext;
using Abp.Zero.EntityFrameworkCore;
using Abp.ZeroCore.SampleApp.Core;
using Abp.ZeroCore.SampleApp.Core.BookStore;
using Abp.ZeroCore.SampleApp.Core.EntityHistory;
using Abp.ZeroCore.SampleApp.Core.Shop;
using Microsoft.EntityFrameworkCore;

namespace Abp.ZeroCore.SampleApp.EntityFramework;

public class SampleAppDbContext : AbpZeroDbContext<Tenant, Role, User, SampleAppDbContext>, IAbpPersistedGrantDbContext
{
    public DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

    public DbSet<Advertisement> Advertisements { get; set; }

    public DbSet<Blog> Blogs { get; set; }

    public DbSet<Post> Posts { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ProductTranslation> ProductTranslations { get; set; }

    public DbSet<Office> Offices { get; set; }

    public DbSet<OfficeTranslation> OfficeTranslations { get; set; }

    public DbSet<Author> Authors { get; set; }

    public DbSet<Store> Stores { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderTranslation> OrderTranslations { get; set; }

    public DbSet<UserTestEntity> UserTestEntities { get; set; }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Foo> Foo { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Restaurant> Restaurants { get; set; }

    public SampleAppDbContext(DbContextOptions<SampleAppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigurePersistedGrantEntity();

        // EF property mapped directly to a field


        modelBuilder.Entity<Blog>().OwnsOne(x => x.More);

        modelBuilder.Entity<Blog>().OwnsMany(x => x.Promotions, b =>
        {
            b.WithOwner().HasForeignKey(bp => bp.BlogId);
            b.Property<int>("Id");
            b.HasKey("Id");

            b.HasOne<Blog>()
             .WithOne()
             .HasForeignKey<BlogPromotion>(bp => bp.AdvertisementId)
             .IsRequired();
        });

        modelBuilder.Entity<Advertisement>().OwnsMany(a => a.Feedbacks, b =>
        {
            b.WithOwner().HasForeignKey(af => af.AdvertisementId);
            b.Property<int>("Id");
            b.HasKey("Id");

            b.HasOne<Comment>()
             .WithOne()
             .HasForeignKey<AdvertisementFeedback>(af => af.CommentId);
        });

        modelBuilder.Entity<Book>().ToTable("Books");
        modelBuilder.Entity<Book>().Property(e => e.Id).ValueGeneratedNever();

        modelBuilder.Entity<Store>().Property(e => e.Id).HasColumnName("StoreId");

        // Register custom entity which is not in DbContext
        modelBuilder.Entity(typeof(CustomEntity));
        modelBuilder.Entity(typeof(CustomEntityWithGuidId));
    }
}