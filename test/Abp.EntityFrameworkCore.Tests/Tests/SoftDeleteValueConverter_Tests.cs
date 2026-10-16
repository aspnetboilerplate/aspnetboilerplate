using System.Linq;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityFrameworkCore.Tests.Ef;
using Abp.Modules;
using Abp.TestBase;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests;

public class SoftDeleteValueConverter_WithoutAbpQueryCompiler_Tests
    : SoftDeleteValueConverter_Tests<CompiledQueryCacheWithoutAbpQueryCompilerTestModule>
{
}

public class SoftDeleteValueConverter_WithAbpQueryCompiler_Tests
    : SoftDeleteValueConverter_Tests<CompiledQueryCacheWithAbpQueryCompilerTestModule>
{
}

[Collection("Clock.Provider")]
public abstract class SoftDeleteValueConverter_Tests<TStartupModule> : AbpIntegratedTestBase<TStartupModule>
    where TStartupModule : AbpModule
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IRepository<IntSoftDeleteNote> _noteRepository;

    protected SoftDeleteValueConverter_Tests()
    {
        _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        _noteRepository = Resolve<IRepository<IntSoftDeleteNote>>();

        using (var context = Resolve<QueryCacheDbContext>())
        {
            context.IntSoftDeleteNotes.AddRange(
                new IntSoftDeleteNote { Title = "kept" },
                new IntSoftDeleteNote { Title = "removed", IsDeleted = true }
            );

            context.SaveChanges();
        }
    }

    [Fact]
    public void Should_Apply_SoftDelete_Filter_To_Converted_Column()
    {
        using (var uow = _unitOfWorkManager.Begin())
        {
            _noteRepository.GetAllList().Select(n => n.Title).ShouldBe(new[] { "kept" });

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                _noteRepository.GetAllList().Select(n => n.Title).OrderBy(t => t).ShouldBe(new[] { "kept", "removed" });
            }

            uow.Complete();
        }
    }
}
