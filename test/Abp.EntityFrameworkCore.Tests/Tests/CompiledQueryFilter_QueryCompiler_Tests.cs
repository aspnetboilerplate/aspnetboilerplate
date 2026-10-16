using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Tests.Ef;
using Abp.Modules;
using Abp.TestBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests;

#pragma warning disable EF1001
public class CompiledQueryFilter_QueryCompiler_Tests
    : CompiledQueryFilter_QueryCompiler_Tests<CompiledQueryCacheWithQueryCompilerTestModule>
{
}

public class CompiledQueryFilter_QueryCompilerImplementation_Tests
    : CompiledQueryFilter_QueryCompiler_Tests<CompiledQueryCacheWithQueryCompilerImplementationTestModule>
{
}

public abstract class CompiledQueryFilter_QueryCompiler_Tests<TStartupModule> : CompiledQueryFilter_Tests<TStartupModule>
    where TStartupModule : AbpModule
{
    [Fact]
    public void Should_Use_The_Replaced_Query_Compiler()
    {
        Run(1, db =>
        {
            var queryCompiler = db.GetService<IQueryCompiler>().ShouldBeOfType<CompiledQueryTestQueryCompiler>();
            var titles = db.TenantNotes.Select(n => n.Title).ToArray();
            queryCompiler.ExecuteCount.ShouldBe(1);
            return titles;
        }).ShouldBe(new[] { "tenant-1-note" });
    }

    [Fact]
    public void Should_Not_Run_Compiled_Query_First_Run_Inside_Another_Query_Without_Filters()
    {
        var countMemos = EF.CompileQuery((QueryCacheDbContext db) => db.TenantMemos.Count());
        var unitOfWorkManager = Resolve<IUnitOfWorkManager>();

        Run(1, db =>
        {
            var queryCompiler = db.GetService<IQueryCompiler>().ShouldBeOfType<CompiledQueryTestQueryCompiler>();
            using (unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete, AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                Should.Throw<InvalidOperationException>(() =>
                        db.TenantNotes.Where(n => n.Title.Length > countMemos(db)).Select(n => n.Title).ToArray())
                    .InnerException.ShouldBeOfType<AbpException>();
            }

            queryCompiler.CreateCompiledQueryCount.ShouldBe(1);
            return db.TenantMemos.Select(m => m.Text).ToArray();
        }).ShouldBe(new[] { "tenant-1-memo" });
    }
}

[DependsOn(typeof(AbpEntityFrameworkCoreModule), typeof(AbpTestBaseModule))]
public class CompiledQueryCacheWithQueryCompilerTestModule : CompiledQueryCacheTestModuleBase
{
    protected override bool UseAbpQueryCompiler => true;

    protected override void ConfigureOptions(DbContextOptionsBuilder<QueryCacheDbContext> optionsBuilder)
    {
        optionsBuilder.ReplaceService<IQueryCompiler, CompiledQueryTestQueryCompiler>();
    }
}

[DependsOn(typeof(AbpEntityFrameworkCoreModule), typeof(AbpTestBaseModule))]
public class CompiledQueryCacheWithQueryCompilerImplementationTestModule : CompiledQueryCacheTestModuleBase
{
    protected override bool UseAbpQueryCompiler => true;

    protected override void ConfigureOptions(DbContextOptionsBuilder<QueryCacheDbContext> optionsBuilder)
    {
        optionsBuilder.ReplaceService<IQueryCompiler, QueryCompiler, CompiledQueryTestQueryCompiler>();
    }
}

// A custom query compiler that an application registers with ReplaceService.
public class CompiledQueryTestQueryCompiler : IQueryCompiler
{
    private readonly QueryCompiler _inner;

    public int ExecuteCount { get; private set; }

    public int CreateCompiledQueryCount { get; private set; }

    public CompiledQueryTestQueryCompiler(IServiceProvider serviceProvider)
    {
        _inner = ActivatorUtilities.CreateInstance<QueryCompiler>(serviceProvider);
    }

    public TResult Execute<TResult>(Expression query)
    {
        ExecuteCount++;
        return _inner.Execute<TResult>(query);
    }

    public TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
    {
        ExecuteCount++;
        return _inner.ExecuteAsync<TResult>(query, cancellationToken);
    }

    public Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query)
    {
        CreateCompiledQueryCount++;
        return _inner.CreateCompiledQuery<TResult>(query);
    }

    public Func<QueryContext, TResult> CreateCompiledAsyncQuery<TResult>(Expression query)
    {
        CreateCompiledQueryCount++;
        return _inner.CreateCompiledAsyncQuery<TResult>(query);
    }

#pragma warning disable EF9100
    public Expression<Func<QueryContext, TResult>> PrecompileQuery<TResult>(Expression query, bool async)
    {
        return _inner.PrecompileQuery<TResult>(query, async);
    }
#pragma warning restore EF9100
}
#pragma warning restore EF1001
