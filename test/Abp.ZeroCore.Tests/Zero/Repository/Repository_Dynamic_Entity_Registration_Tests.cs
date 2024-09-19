using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.ZeroCore.SampleApp.Core.EntityHistory;
using Shouldly;
using Xunit;

namespace Abp.Zero.Repository;

public class Generic_Repository_Registration_Tests : AbpZeroTestBase
{
    private readonly IRepository<CustomEntity> _customEntityRepository;
    private readonly IRepository<CustomEntityWithGuidId, Guid> _customEntityWithGuidRepository;

    public Generic_Repository_Registration_Tests()
    {
        _customEntityRepository = LocalIocManager.Resolve<IRepository<CustomEntity>>();
        _customEntityWithGuidRepository = LocalIocManager.Resolve<IRepository<CustomEntityWithGuidId, Guid>>();
    }

    [Fact]
    public async Task Generic_Repository_Registration_Test_For_Entity_With_Int_Id()
    {
        var items = await _customEntityRepository.GetAllListAsync();
        items.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Generic_Repository_Registration_Test_For_Entity_With_Guid_Id()
    {
        var items = await _customEntityWithGuidRepository.GetAllListAsync();
        items.Count.ShouldBe(0);
    }
}