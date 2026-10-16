using System;
using System.Linq;
using Abp.EntityFrameworkCore.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests;

public class AbpDbContextOptionsExtension_Tests
{
    [Fact]
    public void Should_Create_And_Delete_InMemory_Database()
    {
        var options = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning))
            .AddAbpDbContextOptionsExtension()
            .Options;

        using (var context = new DbContext(options))
        {
            context.GetService<IDatabase>().ShouldNotBeOfType<AbpEfCoreDatabase>();
            context.Database.EnsureCreated().ShouldBeTrue();
            context.Database.EnsureDeleted().ShouldBeTrue();
        }
    }

    [Fact]
    public void Should_Apply_Services_Once()
    {
        var services = new ServiceCollection();
        services.AddEntityFrameworkSqlite();

        new AbpDbContextOptionsExtension().ApplyServices(services);
        new AbpDbContextOptionsExtension().ApplyServices(services);

        services.Count(x => x.ServiceType == typeof(IDatabase) && x.IsKeyedService).ShouldBe(1);
        services.Count(x => x.ServiceType == typeof(AbpEfCoreCurrentDbContext)).ShouldBe(1);
    }

    [Fact]
    public void Should_Apply_Services_Again_After_The_Database_Provider()
    {
        var services = new ServiceCollection();
        var extension = new AbpDbContextOptionsExtension();

        extension.ApplyServices(services);
        services.AddEntityFrameworkSqlite();
        extension.ApplyServices(services);

        services.Count(x => x.ServiceType == typeof(IDatabase) && x.IsKeyedService).ShouldBe(1);
        services.Count(x => x.ServiceType == typeof(ICompiledQueryCacheKeyGenerator) && x.IsKeyedService).ShouldBe(1);
        services.Count(x => x.ServiceType == typeof(AbpEfCoreCurrentDbContext)).ShouldBe(1);
    }

    [Fact]
    public void Should_Decorate_Database_Registered_After_Applying_Services()
    {
        var services = new ServiceCollection();
        var extension = new AbpDbContextOptionsExtension();
        services.AddEntityFrameworkSqlite();
        extension.ApplyServices(services);
        services.AddScoped<IDatabase>(provider => ActivatorUtilities.CreateInstance<RelationalDatabase>(provider));
        extension.ApplyServices(services);

        using (var serviceProvider = services.BuildServiceProvider())
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=:memory:")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var context = new DbContext(options))
            {
                context.GetService<IDatabase>().ShouldBeOfType<AbpEfCoreDatabase>();
            }
        }
    }

    [Fact]
    public void Should_Keep_Using_And_Disposing_The_Custom_Database()
    {
        CompiledQueryTestDatabase database = null;
        var services = new ServiceCollection();
        services.AddEntityFrameworkSqlite();
        services.AddScoped<IDatabase>(provider => database = ActivatorUtilities.CreateInstance<CompiledQueryTestDatabase>(provider));
        new AbpDbContextOptionsExtension().ApplyServices(services);

        using (var serviceProvider = services.BuildServiceProvider())
        using (var connection = new SqliteConnection("Data Source=:memory:"))
        {
            connection.Open();
            var options = new DbContextOptionsBuilder<CustomDatabaseDbContext>()
                .UseSqlite(connection)
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var context = new CustomDatabaseDbContext(options))
            {
                context.GetService<IDatabase>().ShouldBeOfType<AbpEfCoreDatabase>();
                context.Database.EnsureCreated();
                context.Notes.Add(new CustomDatabaseNote { Title = "note" });
                context.SaveChanges();
                database.DisposeCount.ShouldBe(0);
            }

            database.SaveChangesCount.ShouldBe(1);
            database.DisposeCount.ShouldBe(1);
        }
    }

    public static TheoryData<Func<DbContextOptionsBuilder, DbContextOptionsBuilder>> ReplacedWrappedServices =>
        new TheoryData<Func<DbContextOptionsBuilder, DbContextOptionsBuilder>>
        {
            builder => builder.ReplaceService<IDatabase, CompiledQueryTestDatabase>(),
            builder => builder.ReplaceService<IDatabase, RelationalDatabase, CompiledQueryTestDatabase>(),
            builder => builder.ReplaceService<ICompiledQueryCacheKeyGenerator, RelationalCompiledQueryCacheKeyGenerator>()
        };

    [Theory]
    [MemberData(nameof(ReplacedWrappedServices))]
    public void Should_Not_Allow_Replacing_Wrapped_Services(Func<DbContextOptionsBuilder, DbContextOptionsBuilder> replaceService)
    {
        var options = replaceService(new DbContextOptionsBuilder().UseSqlite("Data Source=:memory:"))
            .AddAbpDbContextOptionsExtension()
            .Options;

        Should.Throw<AbpException>(() => new DbContext(options).Dispose()).Message.ShouldContain("ReplaceService");
    }

    public class CustomDatabaseDbContext : DbContext
    {
        public DbSet<CustomDatabaseNote> Notes { get; set; }

        public CustomDatabaseDbContext(DbContextOptions<CustomDatabaseDbContext> options)
            : base(options)
        {
        }
    }

    public class CustomDatabaseNote
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }
}
