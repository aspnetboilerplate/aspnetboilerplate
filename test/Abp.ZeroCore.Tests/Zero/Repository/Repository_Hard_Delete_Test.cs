using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ZeroCore.SampleApp.Core;
using Abp.ZeroCore.SampleApp.Core.EntityHistory;
using Shouldly;
using Xunit;

namespace Abp.Zero.Repository
{
    public class Repository_Hard_Delete_Test : AbpZeroTestBase
    {
        private readonly IRepository<Role> _roleRepository;

        public Repository_Hard_Delete_Test()
        {
            _roleRepository = LocalIocManager.Resolve<IRepository<Role>>();
        }

        [Fact]
        public async Task Should_Permanently_Delete_SoftDelete_Entity_With_HarDelete_Method()
        {
            LoginAsDefaultTenantAdmin();

            var uowManager = Resolve<IUnitOfWorkManager>();

            // Soft-Delete admin
            using (var uow = uowManager.Begin())
            {
                var admin = await _roleRepository.FirstOrDefaultAsync(u => u.NormalizedName == "ADMIN");
                await _roleRepository.DeleteAsync(admin);

                await uow.CompleteAsync();
            }

            using (var uow = uowManager.Begin())
            {
                var roles = await _roleRepository.GetAllListAsync();

                foreach (var role in roles)
                {
                    await _roleRepository.HardDeleteAsync(role);
                }

                await uow.CompleteAsync();
            }

            using (var uow = uowManager.Begin())
            {
                using (uowManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    var roles = await _roleRepository.GetAllListAsync();
                    roles.Count.ShouldBe(1);
                    roles.First().NormalizedName.ShouldBe("ADMIN");
                }

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Permanently_Delete_Multiple_SoftDelete_Entities_With_HarDelete_Method()
        {
            LoginAsDefaultTenantAdmin();

            var uowManager = Resolve<IUnitOfWorkManager>();

            // Soft-Delete admin
            using (var uow = uowManager.Begin())
            {
                var admin = await _roleRepository.FirstOrDefaultAsync(u => u.NormalizedName == "ADMIN");
                await _roleRepository.DeleteAsync(admin);

                await uow.CompleteAsync();
            }

            using (var uow = uowManager.Begin())
            {
                await _roleRepository.HardDeleteAsync(r => r.Id > 0);
                await uow.CompleteAsync();
            }

            using (var uow = uowManager.Begin())
            {
                using (uowManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    var roles = await _roleRepository.GetAllListAsync();
                    roles.Count.ShouldBe(1);
                    roles.First().NormalizedName.ShouldBe("ADMIN");
                }

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Throw_Exception_If_There_No_UnitOfWork()
        {
            var admin = await _roleRepository.FirstOrDefaultAsync(u => u.NormalizedName == "ADMIN");

            Assert.Throws<AbpException>(() => _roleRepository.HardDelete(admin));
            Assert.Throws<AbpException>(() => _roleRepository.HardDelete(u => u.Id > 0));

            await Assert.ThrowsAsync<AbpException>(async () => await _roleRepository.HardDeleteAsync(admin));
            await Assert.ThrowsAsync<AbpException>(async () => await _roleRepository.HardDeleteAsync(u => u.Id > 0));
        }

        [Fact]
        public async Task Should_Not_Throw_Exception_When_Deleting_ValueObject_And_HardDeleting_Entity()
        {
            LoginAsDefaultTenantAdmin();

            var uowManager = Resolve<IUnitOfWorkManager>();
            
            UsingDbContext(context =>
            {
                context.Categories.Add(new Category
                {
                    Id = 42,
                    DisplayName = "Soft Drinks"
                });
            });
            
            using (var uow = uowManager.Begin())
            {
                await _roleRepository.HardDeleteAsync(r => r.Id > 0);
                
                UsingDbContext(context =>
                {
                    var category = context.Categories.Single(e=> e.Id == 42);
                    context.Categories.Remove(category);
                });
                
                await uow.CompleteAsync();
            }
        }
    }
}
