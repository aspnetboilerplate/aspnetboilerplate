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
        exception.Message.ShouldContain("AbpEntityQueryProvider");
    }

    [Fact]
    public void Should_Still_Apply_Filters_To_Normal_Queries()
    {
        Run(1, db => db.TenantNotes.Select(n => n.Title).OrderBy(t => t).ToArray()).ShouldBe(new[] { "tenant-1-note" });
        Run(null, db => db.TenantNotes.Select(n => n.Title).OrderBy(t => t).ToArray()).ShouldBe(new[] { "host-note" });
        Run(1, db => db.TenantMemos.Select(m => m.Text).OrderBy(t => t).ToArray()).ShouldBe(new[] { "tenant-1-memo" });
        Run(1, db => db.TenantOrders.Select(o => o.Number).OrderBy(n => n).ToArray()).ShouldBe(new[] { "tenant-1-order" });
    }

    private static void ShouldThrowFilterException(Action action)
    {
        Should.Throw<AbpException>(action).Message.ShouldContain("AbpEntityQueryProvider");
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
