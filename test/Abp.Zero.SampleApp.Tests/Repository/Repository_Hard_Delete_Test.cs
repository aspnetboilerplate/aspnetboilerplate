using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Repository
{
    public class Repository_Hard_Delete_Test : SampleAppTestBase
    {
        private readonly IRepository<User, long> _useRepository;

        public Repository_Hard_Delete_Test()
        {
            _useRepository = LocalIocManager.Resolve<IRepository<User, long>>();
        }

        [Fact]
        public async Task Should_Permanently_Delete_SoftDelete_Entity_With_HarDelete_Method()
        {
            AbpSession.TenantId = 1;

            var uowManager = Resolve<IUnitOfWorkManager>();

            // Soft-Delete admin
            using (var uow = uowManager.Begin())
            {
                var admin = await _useRepository.FirstOrDefaultAsync(u => u.UserName == "admin");
                await _useRepository.DeleteAsync(admin);

                uow.Complete();
            }

            using (var uow = uowManager.Begin())
            {
                var users = _useRepository.GetAllList();

                foreach (var user in users)
                {
                    await _useRepository.HardDeleteAsync(user);
                }

                uow.Complete();
            }

            using (var uow = uowManager.Begin())
            {
                using (uowManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    var users = _useRepository.GetAllList();
                    users.Count.ShouldBe(1);
                    users.First().UserName.ShouldBe("admin");
                }

                uow.Complete();
            }
        }

        [Fact]
        public async Task Should_Permanently_Delete_Multiple_SoftDelete_Entities_With_HarDelete_Method()
        {
            AbpSession.TenantId = 1;

            var uowManager = Resolve<IUnitOfWorkManager>();

            // Soft-Delete admin
            using (var uow = uowManager.Begin())
            {
                var admin = await _useRepository.FirstOrDefaultAsync(u => u.UserName == "admin");
                await _useRepository.DeleteAsync(admin);

                uow.Complete();
            }

            using (var uow = uowManager.Begin())
            {
                await _useRepository.HardDeleteAsync(u => u.Id > 0);

                uow.Complete();
            }

            using (var uow = uowManager.Begin())
            {
                using (uowManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    var users = _useRepository.GetAllList();
                    users.Count.ShouldBe(1);
                    users.First().UserName.ShouldBe("admin");
                }

                uow.Complete();
            }
        }
    }
}
