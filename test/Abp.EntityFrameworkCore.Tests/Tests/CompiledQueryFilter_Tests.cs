using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityFrameworkCore.Tests.Ef;
using Abp.Modules;
using Abp.TestBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests;

// Regression tests for https://github.com/aspnetboilerplate/aspnetboilerplate/issues/7217.
// EF.CompileQuery and EF.CompileAsyncQuery compile the query outside AbpEntityQueryProvider.
// Each filter is tested on its own entity, so a failure of one translation cannot hide another:
// TenantNote has SoftDelete and MayHaveTenant, TenantMemo has only MayHaveTenant and
// TenantOrder has only MustHaveTenant.
public class CompiledQueryFilter_WithoutAbpQueryCompiler_Tests
    : CompiledQueryFilter_Tests<CompiledQueryCacheWithoutAbpQueryCompilerTestModule>
{
    // The filters are parameters of the compiled query, so they are applied on every run.
    [Fact]
    public void Should_Apply_Filters_To_Compiled_Query()
    {
        Run(1, db => CompiledNotes(db).ToArray()).ShouldBe(new[] { "tenant-1-note" });
        Run(2, db => CompiledNotes(db).ToArray()).ShouldBe(new[] { "tenant-2-note" });
        Run(null, db => CompiledNotes(db).ToArray()).ShouldBe(new[] { "host-note" });

        Run(1, db => CompiledMemos(db).ToArray()).ShouldBe(new[] { "tenant-1-memo" });
        Run(null, db => CompiledMemos(db).ToArray()).ShouldBe(new[] { "host-memo" });

        Run(1, db => CompiledOrders(db).ToArray()).ShouldBe(new[] { "tenant-1-order" });
        Run(2, db => CompiledOrders(db).ToArray()).ShouldBe(new[] { "tenant-2-order" });
    }

    [Fact]
    public async Task Should_Apply_Filters_To_Compiled_Async_Query()
    {
        (await RunAsync(1, db => ToArrayAsync(CompiledNotesAsync(db)))).ShouldBe(new[] { "tenant-1-note" });
        (await RunAsync(2, db => ToArrayAsync(CompiledNotesAsync(db)))).ShouldBe(new[] { "tenant-2-note" });
    }
}

public class CompiledQueryFilter_WithAbpQueryCompiler_Tests
    : CompiledQueryFilter_Tests<CompiledQueryCacheWithAbpQueryCompilerTestModule>
{
    // The filters cannot be applied, so the query must fail instead of returning all rows.
    [Fact]
    public void Should_Not_Run_Compiled_Query_Without_SoftDelete_Filter()
    {
        ShouldThrowFilterException(() => Run(1, db => CompiledNotes(db).ToArray()));
        ShouldThrowFilterException(() => Run(1, db => new[] { CompiledNoteCount(db).ToString() }));
    }

    [Fact]
    public void Should_Not_Run_Compiled_Query_Without_MayHaveTenant_Filter()
    {
        ShouldThrowFilterException(() => Run(1, db => CompiledMemos(db).ToArray()));
        ShouldThrowFilterException(() => Run(null, db => CompiledMemos(db).ToArray()));
    }

    [Fact]
    public void Should_Not_Run_Compiled_Query_Without_MustHaveTenant_Filter()
    {
        ShouldThrowFilterException(() => Run(1, db => CompiledOrders(db).ToArray()));
    }

    [Fact]
    public async Task Should_Not_Run_Compiled_Async_Query_Without_Filters()
    {
        var exception = await Should.ThrowAsync<AbpException>(() => RunAsync(1, db => ToArrayAsync(CompiledNotesAsync(db))));
        exception.Message.ShouldContain("EF.CompileQuery");
    }

    [Fact]
    public void Should_Still_Apply_Filters_To_Normal_Queries()
    {
        Run(1, db => db.TenantNotes.Select(n => n.Title).OrderBy(t => t).ToArray()).ShouldBe(new[] { "tenant-1-note" });
        Run(null, db => db.TenantNotes.Select(n => n.Title).OrderBy(t => t).ToArray()).ShouldBe(new[] { "host-note" });
        Run(1, db => db.TenantMemos.Select(m => m.Text).OrderBy(t => t).ToArray()).ShouldBe(new[] { "tenant-1-memo" });
        Run(1, db => db.TenantOrders.Select(o => o.Number).OrderBy(n => n).ToArray()).ShouldBe(new[] { "tenant-1-order" });
    }

    // The compiled query first runs while EF Core evaluates the parameters of the outer query.
    // EF Core wraps the exceptions thrown while it evaluates the parameters of the outer query.
    [Fact]
    public void Should_Not_Run_Compiled_Query_First_Run_Inside_Another_Query_Without_Filters()
    {
        var countMemos = EF.CompileQuery((QueryCacheDbContext db) => db.TenantMemos.Count());

        var exception = Should.Throw<InvalidOperationException>(() =>
            Run(1, db => db.TenantNotes.Where(n => n.Title.Length > countMemos(db)).Select(n => n.Title).ToArray()));
        exception.InnerException.ShouldBeOfType<AbpException>();

        ShouldThrowFilterException(() => Run(1, db => new[] { countMemos(db).ToString() }));
    }

    [Fact]
    public void Should_Not_Run_Compiled_Async_Query_First_Run_Inside_Another_Query_Without_Filters()
    {
        var countMemosAsync = EF.CompileAsyncQuery((QueryCacheDbContext db) => db.TenantMemos.Count());

        var exception = Should.Throw<InvalidOperationException>(() =>
            Run(1, db => db.TenantNotes.Where(n => n.Title.Length > countMemosAsync(db).GetAwaiter().GetResult()).Select(n => n.Title).ToArray()));
        exception.InnerException.ShouldBeOfType<AbpException>();

        ShouldThrowFilterException(() => Run(1, db => new[] { countMemosAsync(db).GetAwaiter().GetResult().ToString() }));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Should_Not_Run_Compiled_Query_First_Run_Inside_Another_Query_With_Disabled_Filters(bool asyncCompiledQuery)
    {
        var countMemos = EF.CompileQuery((QueryCacheDbContext db) => db.TenantMemos.Count());
        var countMemosAsync = EF.CompileAsyncQuery((QueryCacheDbContext db) => db.TenantMemos.Count());
        var unitOfWorkManager = Resolve<IUnitOfWorkManager>();

        var exception = Should.Throw<InvalidOperationException>(() => Run(1, db =>
        {
            using (unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete, AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                return asyncCompiledQuery
                    ? db.TenantNotes.Where(n => n.Title.Length > countMemosAsync(db).GetAwaiter().GetResult()).Select(n => n.Title).ToArray()
                    : db.TenantNotes.Where(n => n.Title.Length > countMemos(db)).Select(n => n.Title).ToArray();
            }
        }));
        exception.InnerException.ShouldBeOfType<AbpException>();

        ShouldThrowFilterException(() => Run(1, db =>
            new[] { (asyncCompiledQuery ? countMemosAsync(db).GetAwaiter().GetResult() : countMemos(db)).ToString() }));
    }

    [Fact]
    public void Should_Not_Run_Compiled_Query_First_Run_While_Another_Query_Is_Compiled_Without_Filters()
    {
        var countMemos = EF.CompileQuery((QueryCacheDbContext db) => db.TenantMemos.Count());
        var countMemosAsync = EF.CompileAsyncQuery((QueryCacheDbContext db) => db.TenantMemos.Count());
        var hooks = Resolve<CompiledQueryTestHooks>();

        hooks.OnQueryCompilationStarting = db =>
        {
            hooks.OnQueryCompilationStarting = null;
            countMemos(db);
        };
        ShouldThrowFilterException(() => Run(1, db => db.TenantNotes.TagWith(Guid.NewGuid().ToString()).Select(n => n.Title).ToArray()));

        hooks.OnQueryCompilationStarting = db =>
        {
            hooks.OnQueryCompilationStarting = null;
            countMemosAsync(db).GetAwaiter().GetResult();
        };
        ShouldThrowFilterException(() => Run(1, db => db.TenantNotes.TagWith(Guid.NewGuid().ToString()).Select(n => n.Title).ToArray()));

        ShouldThrowFilterException(() => Run(1, db => new[] { countMemos(db).ToString() }));
    }

    [Fact]
    public void Should_Not_Run_Compiled_Query_First_Run_While_Cache_Key_Is_Generated_Without_Filters()
    {
        var countMemos = EF.CompileQuery((QueryCacheDbContext db) => db.TenantMemos.Count());
        var hooks = Resolve<CompiledQueryTestHooks>();

        ShouldThrowFilterException(() => Run(1, db =>
        {
            hooks.OnGetCompiledQueryCacheKey = () =>
            {
                hooks.OnGetCompiledQueryCacheKey = null;
                countMemos(db);
            };

            return db.TenantNotes.TagWith(Guid.NewGuid().ToString()).Select(n => n.Title).ToArray();
        }));

        ShouldThrowFilterException(() => Run(1, db => new[] { countMemos(db).ToString() }));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Should_Not_Precompile_Query_While_Cache_Key_Is_Generated_Without_Filters(bool async)
    {
        var hooks = Resolve<CompiledQueryTestHooks>();

        ShouldThrowFilterException(() => Run(1, db =>
        {
            hooks.OnGetCompiledQueryCacheKey = () =>
            {
                hooks.OnGetCompiledQueryCacheKey = null;
#pragma warning disable EF1001, EF9100
                var queryCompiler = db.GetService<IQueryCompiler>();
                var query = db.TenantMemos.Select(m => m.Text).Expression;
                if (async)
                {
                    queryCompiler.PrecompileQuery<IAsyncEnumerable<string>>(query, true);
                }
                else
                {
                    queryCompiler.PrecompileQuery<IEnumerable<string>>(query, false);
                }
#pragma warning restore EF1001, EF9100
            };

            return db.TenantNotes.TagWith(Guid.NewGuid().ToString()).Select(n => n.Title).ToArray();
        }));
    }

    [Fact]
    public void Should_Provide_Current_DbContext_While_Cache_Key_Is_Generated()
    {
        var hooks = Resolve<CompiledQueryTestHooks>();

        Run(1, db =>
        {
            var currentDbContext = db.GetService<AbpEfCoreCurrentDbContext>();
            var contexts = new List<AbpDbContext>();
            hooks.OnGetCompiledQueryCacheKey = () => contexts.Add(currentDbContext.Context);
            try
            {
                db.TenantNotes.Select(n => n.Title).ToArray();
                db.TenantNotes.Select(n => n.Title).ToListAsync().GetAwaiter().GetResult();
            }
            finally
            {
                hooks.OnGetCompiledQueryCacheKey = null;
            }

            contexts.Count.ShouldBe(2);
            contexts.ShouldAllBe(context => context == db);
            return Array.Empty<string>();
        });
    }

    [Fact]
    public void Should_Run_Compiled_Query_That_Ignores_Filters()
    {
        var countAllMemos = EF.CompileQuery((QueryCacheDbContext db) => db.TenantMemos.IgnoreQueryFilters().Count());

        Run(1, db => new[] { countAllMemos(db).ToString() }).ShouldBe(new[] { "3" });
    }

    [Fact]
    public void Should_Not_Compile_Query_With_Cache_Key_Generated_Outside_Of_Query_Provider()
    {
        ShouldThrowFilterException(() => Run(1, db =>
        {
            var expression = ((IQueryable<TenantMemo>)db.TenantMemos).Expression;
            db.GetService<ICompiledQueryCacheKeyGenerator>().GenerateCacheKey(expression, false);
            db.GetService<IDatabase>().CompileQuery<IEnumerable<TenantMemo>>(expression, false);
            return Array.Empty<string>();
        }));
    }

    [Fact]
    public void Should_Apply_Filters_To_Normal_Query_After_Compiled_Query_Failed()
    {
        var countMemos = EF.CompileQuery((QueryCacheDbContext db) => db.TenantMemos.Count());

        Should.Throw<InvalidOperationException>(() =>
            Run(1, db => db.TenantNotes.Where(n => n.Title.Length > countMemos(db)).Select(n => n.Title).ToArray()));

        Run(1, db => db.TenantMemos.Select(m => m.Text).OrderBy(t => t).ToArray()).ShouldBe(new[] { "tenant-1-memo" });

        Run(1, db =>
        {
            Should.Throw<InvalidOperationException>(() => db.TenantNotes.Where(n => n.Title.Length > countMemos(db)).Select(n => n.Title).ToArray());
            return db.TenantMemos.Select(m => m.Text).OrderBy(t => t).ToArray();
        }).ShouldBe(new[] { "tenant-1-memo" });
    }

    // A library can also compile a query itself through IDatabase while the outer query is executed.
    [Fact]
    public void Should_Not_Compile_Query_Inside_Another_Query_Without_Filters()
    {
        var exception = Should.Throw<InvalidOperationException>(() =>
            Run(1, db => db.TenantNotes.Where(n => n.Title.Length > CompileMemosQuery(db)).Select(n => n.Title).ToArray()));
        exception.InnerException.ShouldBeOfType<AbpException>();
    }

    private static int CompileMemosQuery(QueryCacheDbContext db)
    {
        db.GetService<IDatabase>().CompileQuery<IEnumerable<TenantMemo>>(((IQueryable<TenantMemo>)db.TenantMemos).Expression, false);
        return 0;
    }

    private static void ShouldThrowFilterException(Action action)
    {
        Should.Throw<AbpException>(action).Message.ShouldContain("EF.CompileQuery");
    }
}

[Collection("Clock.Provider")]
public abstract class CompiledQueryFilter_Tests<TStartupModule> : AbpIntegratedTestBase<TStartupModule>
    where TStartupModule : AbpModule
{
    protected static readonly Func<QueryCacheDbContext, IEnumerable<string>> CompiledNotes =
        EF.CompileQuery((QueryCacheDbContext db) => db.TenantNotes.OrderBy(n => n.Title).Select(n => n.Title));

    protected static readonly Func<QueryCacheDbContext, int> CompiledNoteCount =
        EF.CompileQuery((QueryCacheDbContext db) => db.TenantNotes.Count());

    protected static readonly Func<QueryCacheDbContext, IAsyncEnumerable<string>> CompiledNotesAsync =
        EF.CompileAsyncQuery((QueryCacheDbContext db) => db.TenantNotes.OrderBy(n => n.Title).Select(n => n.Title));

    protected static readonly Func<QueryCacheDbContext, IEnumerable<string>> CompiledMemos =
        EF.CompileQuery((QueryCacheDbContext db) => db.TenantMemos.OrderBy(m => m.Text).Select(m => m.Text));

    protected static readonly Func<QueryCacheDbContext, IEnumerable<string>> CompiledOrders =
        EF.CompileQuery((QueryCacheDbContext db) => db.TenantOrders.OrderBy(o => o.Number).Select(o => o.Number));

    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IRepository<TenantNote> _noteRepository;

    protected CompiledQueryFilter_Tests()
    {
        _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        _noteRepository = Resolve<IRepository<TenantNote>>();

        using (var context = Resolve<QueryCacheDbContext>())
        {
            context.TenantNotes.AddRange(
                new TenantNote { Title = "host-note" },
                new TenantNote { Title = "tenant-1-note", TenantId = 1 },
                new TenantNote { Title = "tenant-1-deleted-note", TenantId = 1, IsDeleted = true },
                new TenantNote { Title = "tenant-2-note", TenantId = 2 }
            );

            context.TenantMemos.AddRange(
                new TenantMemo { Text = "host-memo" },
                new TenantMemo { Text = "tenant-1-memo", TenantId = 1 },
                new TenantMemo { Text = "tenant-2-memo", TenantId = 2 }
            );

            context.TenantOrders.AddRange(
                new TenantOrder { Number = "tenant-1-order", TenantId = 1 },
                new TenantOrder { Number = "tenant-2-order", TenantId = 2 }
            );

            context.SaveChanges();
        }
    }

    [Fact]
    public void Should_Apply_Filters_To_Other_Query_Entry_Points()
    {
        Run(1, db =>
        {
            db.TenantNotes.IgnoreQueryFilters().Count().ShouldBe(4);
            db.TenantNotes.Select(n => n.Title).ToQueryString().ShouldContain("IsDeleted");

            db.TenantNotes.Where(n => n.Title == "tenant-2-note").ExecuteUpdate(s => s.SetProperty(n => n.Title, "tenant-2-updated")).ShouldBe(0);
            db.TenantNotes.Where(n => n.Title == "tenant-1-note").ExecuteUpdate(s => s.SetProperty(n => n.Title, "tenant-1-updated")).ShouldBe(1);

            var note = db.TenantNotes.Single();
            note.Title = "tenant-1-saved";
            db.SaveChanges();

            return db.TenantNotes.AsNoTracking().Select(n => n.Title).ToArray();
        }).ShouldBe(new[] { "tenant-1-saved" });
    }

    protected string[] Run(int? tenantId, Func<QueryCacheDbContext, string[]> query)
    {
        using (var uow = _unitOfWorkManager.Begin())
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var result = query((QueryCacheDbContext)_noteRepository.GetDbContext());
            uow.Complete();
            return result;
        }
    }

    protected async Task<string[]> RunAsync(int? tenantId, Func<QueryCacheDbContext, Task<string[]>> query)
    {
        using (var uow = _unitOfWorkManager.Begin())
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var result = await query((QueryCacheDbContext)_noteRepository.GetDbContext());
            await uow.CompleteAsync();
            return result;
        }
    }

    protected static async Task<string[]> ToArrayAsync(IAsyncEnumerable<string> source)
    {
        var list = new List<string>();
        await foreach (var item in source)
        {
            list.Add(item);
        }

        return list.ToArray();
    }
}
