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
public class CompiledQueryFilter_WithoutAbpQueryCompiler_Tests
    : CompiledQueryFilter_Tests<CompiledQueryCacheWithoutAbpQueryCompilerTestModule>
{
    // The filters are parameters of the compiled query, so they are applied on every run.
    [Fact]
    public void Should_Apply_Filters_To_Compiled_Query()
    {
        RunNotes(1, db => CompiledNotes(db).ToArray()).ShouldBe(new[] { "tenant-1-note" });
        RunNotes(2, db => CompiledNotes(db).ToArray()).ShouldBe(new[] { "tenant-2-note" });
        RunNotes(null, db => CompiledNotes(db).ToArray()).ShouldBe(new[] { "host-note" });
    }

    [Fact]
    public async Task Should_Apply_Filters_To_Compiled_Async_Query()
    {
        (await RunNotesAsync(1, db => ToArrayAsync(CompiledNotesAsync(db)))).ShouldBe(new[] { "tenant-1-note" });
        (await RunNotesAsync(2, db => ToArrayAsync(CompiledNotesAsync(db)))).ShouldBe(new[] { "tenant-2-note" });
    }
}

public class CompiledQueryFilter_WithAbpQueryCompiler_Tests
    : CompiledQueryFilter_Tests<CompiledQueryCacheWithAbpQueryCompilerTestModule>
{
    // The filters cannot be applied, so the query must fail instead of returning all rows.
    [Fact]
    public void Should_Not_Run_Compiled_Query_Without_Filters()
    {
        Should.Throw<AbpException>(() => RunNotes(1, db => CompiledNotes(db).ToArray()));
        Should.Throw<AbpException>(() => RunNotes(1, db => new[] { CompiledNoteCount(db).ToString() }));
    }

    [Fact]
    public async Task Should_Not_Run_Compiled_Async_Query_Without_Filters()
    {
        await Should.ThrowAsync<AbpException>(() => RunNotesAsync(1, db => ToArrayAsync(CompiledNotesAsync(db))));
    }

    [Fact]
    public void Should_Still_Apply_Filters_To_Normal_Queries()
    {
        RunNotes(1, db => db.TenantNotes.Select(n => n.Title).ToArray()).ShouldBe(new[] { "tenant-1-note" });
        RunNotes(null, db => db.TenantNotes.Select(n => n.Title).ToArray()).ShouldBe(new[] { "host-note" });
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

            context.SaveChanges();
        }
    }

    protected string[] RunNotes(int? tenantId, Func<QueryCacheDbContext, string[]> query)
    {
        using (var uow = _unitOfWorkManager.Begin())
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var result = query((QueryCacheDbContext)_noteRepository.GetDbContext());
            uow.Complete();
            return result;
        }
    }

    protected async Task<string[]> RunNotesAsync(int? tenantId, Func<QueryCacheDbContext, Task<string[]>> query)
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
