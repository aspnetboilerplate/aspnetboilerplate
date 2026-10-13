using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Configuration;
using Abp.EntityFrameworkCore.Extensions;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityFrameworkCore.Tests.Ef;
using Abp.Modules;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests;

// Regression tests for https://github.com/aspnetzero/aspnet-zero-core/issues/6059.
// The current tenant id is passed to SQL as a parameter in both filter modes, so a
// compiled query must be shared between tenants. When UseAbpQueryCompiler is false, the
// filter states are parameters too, so changing them must not recompile the query either.
public class CompiledQueryCache_WithoutAbpQueryCompiler_Tests
    : CompiledQueryCache_Tests<CompiledQueryCacheWithoutAbpQueryCompilerTestModule>
{
    [Fact]
    public void Should_Not_Recompile_Query_When_Filter_State_Changes()
    {
        QueryNotes(1).Compilations.ShouldBeGreaterThan(0);

        QueryNotes(1, AbpDataFilters.SoftDelete).Compilations.ShouldBe(0);
        QueryNotes(1, AbpDataFilters.MayHaveTenant).Compilations.ShouldBe(0);
    }
}

public class CompiledQueryCache_WithAbpQueryCompiler_Tests
    : CompiledQueryCache_Tests<CompiledQueryCacheWithAbpQueryCompilerTestModule>
{
}

[Collection("Clock.Provider")]
public abstract class CompiledQueryCache_Tests<TStartupModule> : AbpIntegratedTestBase<TStartupModule>
    where TStartupModule : AbpModule
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IRepository<TenantNote> _noteRepository;
    private readonly IRepository<TenantOrder> _orderRepository;

    protected CompiledQueryCache_Tests()
    {
        _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        _noteRepository = Resolve<IRepository<TenantNote>>();
        _orderRepository = Resolve<IRepository<TenantOrder>>();

        using (var context = Resolve<QueryCacheDbContext>())
        {
            context.TenantNotes.AddRange(
                new TenantNote { Title = "host-note" },
                new TenantNote { Title = "tenant-1-note", TenantId = 1 },
                new TenantNote { Title = "tenant-1-deleted-note", TenantId = 1, IsDeleted = true },
                new TenantNote { Title = "tenant-2-note", TenantId = 2 }
            );

            context.TenantOrders.AddRange(
                new TenantOrder { Number = "tenant-1-order", TenantId = 1 },
                new TenantOrder { Number = "tenant-2-order", TenantId = 2 }
            );

            context.SaveChanges();
        }
    }

    [Fact]
    public void Should_Compile_MayHaveTenant_Query_Once_For_Different_Tenants()
    {
        QueryNotes(1).Compilations.ShouldBeGreaterThan(0);

        QueryNotes(1).Compilations.ShouldBe(0);
        QueryNotes(2).Compilations.ShouldBe(0);
        QueryNotes(3).Compilations.ShouldBe(0);
    }

    [Fact]
    public void Should_Compile_MustHaveTenant_Query_Once_For_Different_Tenants()
    {
        QueryOrders(1).Compilations.ShouldBeGreaterThan(0);

        QueryOrders(1).Compilations.ShouldBe(0);
        QueryOrders(2).Compilations.ShouldBe(0);
        QueryOrders(3).Compilations.ShouldBe(0);
    }

    [Fact]
    public void Should_Apply_Filters_Correctly_When_Queries_Run_For_Different_Tenants_And_Filter_States()
    {
        QueryNotes(1).Titles.ShouldBe(new[] { "tenant-1-note" });
        QueryNotes(2).Titles.ShouldBe(new[] { "tenant-2-note" });
        QueryNotes(null).Titles.ShouldBe(new[] { "host-note" });
        QueryNotes(1, AbpDataFilters.SoftDelete).Titles.ShouldBe(new[] { "tenant-1-deleted-note", "tenant-1-note" });
        QueryNotes(1, AbpDataFilters.MayHaveTenant).Titles.ShouldBe(new[] { "host-note", "tenant-1-note", "tenant-2-note" });
        QueryNotes(2).Titles.ShouldBe(new[] { "tenant-2-note" });

        QueryOrders(1).Titles.ShouldBe(new[] { "tenant-1-order" });
        QueryOrders(2).Titles.ShouldBe(new[] { "tenant-2-order" });
        QueryOrders(null).Titles.ShouldBe(new[] { "tenant-1-order", "tenant-2-order" });
        QueryOrders(1).Titles.ShouldBe(new[] { "tenant-1-order" });
    }

    protected (string[] Titles, int Compilations) QueryNotes(int? tenantId, params string[] disabledFilters)
    {
        return Query(tenantId, disabledFilters, () => _noteRepository.GetAllList().Select(n => n.Title));
    }

    protected (string[] Titles, int Compilations) QueryOrders(int? tenantId, params string[] disabledFilters)
    {
        return Query(tenantId, disabledFilters, () => _orderRepository.GetAllList().Select(o => o.Number));
    }

    private (string[] Titles, int Compilations) Query(int? tenantId, string[] disabledFilters, Func<IEnumerable<string>> query)
    {
        using (var counter = new QueryCompilationCounter())
        using (var uow = _unitOfWorkManager.Begin())
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        using (_unitOfWorkManager.Current.DisableFilter(disabledFilters))
        {
            var titles = query().OrderBy(t => t).ToArray();
            uow.Complete();
            return (titles, counter.Count);
        }
    }

    private sealed class QueryCompilationCounter :
        IObserver<DiagnosticListener>,
        IObserver<KeyValuePair<string, object>>,
        IDisposable
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private int _count;

        public int Count => Volatile.Read(ref _count);

        public QueryCompilationCounter()
        {
            var subscription = DiagnosticListener.AllListeners.Subscribe(this);
            lock (_subscriptions)
            {
                _subscriptions.Add(subscription);
            }
        }

        public void OnNext(DiagnosticListener listener)
        {
            if (listener.Name != DbLoggerCategory.Name)
            {
                return;
            }

            var subscription = listener.Subscribe(this);
            lock (_subscriptions)
            {
                _subscriptions.Add(subscription);
            }
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            if (value.Key == CoreEventId.QueryCompilationStarting.Name &&
                value.Value is DbContextEventData eventData &&
                eventData.Context is QueryCacheDbContext)
            {
                Interlocked.Increment(ref _count);
            }
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void Dispose()
        {
            lock (_subscriptions)
            {
                _subscriptions.ForEach(s => s.Dispose());
                _subscriptions.Clear();
            }
        }
    }
}

[DependsOn(typeof(AbpEntityFrameworkCoreModule), typeof(AbpTestBaseModule))]
public class CompiledQueryCacheWithoutAbpQueryCompilerTestModule : CompiledQueryCacheTestModuleBase
{
    protected override bool UseAbpQueryCompiler => false;
}

[DependsOn(typeof(AbpEntityFrameworkCoreModule), typeof(AbpTestBaseModule))]
public class CompiledQueryCacheWithAbpQueryCompilerTestModule : CompiledQueryCacheTestModuleBase
{
    protected override bool UseAbpQueryCompiler => true;
}

public abstract class CompiledQueryCacheTestModuleBase : AbpModule
{
    protected abstract bool UseAbpQueryCompiler { get; }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsolationLevel = IsolationLevel.Unspecified;
        Configuration.MultiTenancy.IsEnabled = true;
        Configuration.Modules.AbpEfCore().UseAbpQueryCompiler = UseAbpQueryCompiler;

        // A private memory cache gives every test its own EF Core service provider, so the
        // model (built for this filter mode) and the compiled query cache are not shared
        // with other tests.
        var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
        var options = new DbContextOptionsBuilder<QueryCacheDbContext>()
            .UseSqlite(inMemorySqlite)
            .UseMemoryCache(new MemoryCache(new MemoryCacheOptions()))
            .AddAbpDbContextOptionsExtension()
            .Options;

        IocManager.IocContainer.Register(
            Component
                .For<DbContextOptions<QueryCacheDbContext>>()
                .Instance(options)
                .LifestyleSingleton()
        );

        inMemorySqlite.Open();
        using (var context = new QueryCacheDbContext(options))
        {
            context.AbpEfCoreConfiguration = new AbpEfCoreConfiguration(IocManager) { UseAbpQueryCompiler = UseAbpQueryCompiler };
            context.Database.EnsureCreated();
        }
    }

    public override void Initialize()
    {
        IocManager.Register<QueryCacheDbContext>(DependencyLifeStyle.Transient);
    }
}
