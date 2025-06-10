using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Sessions;

public class Session_Override_Tests : AbpZeroTestBase
{
    private readonly UserStore _userStore;
    private readonly IRepository<Restaurant> _restaurantRepository;

    public Session_Override_Tests()
    {
        _userStore = Resolve<UserStore>();
        _restaurantRepository = Resolve<IRepository<Restaurant>>();
    }

    [Fact]
    public async Task Should_Override_Session_Values()
    {
        // Arrange
        await _userStore.CreateAsync(new User
        {
            Name = "john",
            Surname = "nash",
            UserName = "john",
            NormalizedUserName = "JOHN",
            EmailAddress = "john.nash@acme.com",
            NormalizedEmailAddress = "JOHN.NASH@ACME.COM",
            Password = "123qwe"
        });

        // Arrange
        using (var uowManager = LocalIocManager.ResolveAsDisposable<IUnitOfWorkManager>())
        {
            using (var uow = uowManager.Object.Begin(new UnitOfWorkOptions()))
            {
                using (AbpSession.Use(1, 3))
                {
                    await _restaurantRepository.InsertAsync(new Restaurant
                    {
                        Name = "Carluccio's",
                        Cuisine = "Italian"
                    });

                    await uow.CompleteAsync();
                }
            }
        }

        // Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var restaurant = await _restaurantRepository.FirstOrDefaultAsync(r => r.Name == "Carluccio's");
            restaurant.ShouldNotBeNull();
            restaurant.TenantId.ShouldBe(1);
            restaurant.CreatorUserId.ShouldBe(3);
        });
    }
}