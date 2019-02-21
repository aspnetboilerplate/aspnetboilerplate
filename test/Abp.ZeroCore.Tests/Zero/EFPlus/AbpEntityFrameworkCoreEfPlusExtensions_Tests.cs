using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.EFPlus;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.EFPlus
{
    public class AbpEntityFrameworkCoreEfPlusExtensions_Tests : AbpZeroTestBase
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public AbpEntityFrameworkCoreEfPlusExtensions_Tests()
        {
            _roleRepository = Resolve<IRepository<Role>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task Should_Delete_All_Tenant_Roles_When_Using_BatchDeleteAll()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                // Act
                await _roleRepository.BatchDeleteAsync(r => r.Id > 0);

                // Assert
                var roleCount = _roleRepository.Count();
                roleCount.ShouldBe(0);

                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    // Assert
                    roleCount = _roleRepository.Count();
                    roleCount.ShouldBe(1);
                }

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Delete_Filtered_Entities_Using_BatchDelete()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                await _roleRepository.BatchDeleteAsync(e => e.Name != "ADMIN");

                var roleCount = _roleRepository.Count();
                roleCount.ShouldBe(1);

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Use_MayHaveTenant_Filter_When_Deleting_Entities_Using_BatchDelete()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                await _roleRepository.BatchDeleteAsync(r => r.Id > 0);

                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    var admin = await _roleRepository.FirstOrDefaultAsync(r => r.Name == "Admin");
                    admin.ShouldNotBeNull();
                }

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Delete_All_Roles_When_MayHaveTenant_Filter_Is_Disabled_When_Using_BatchDelete()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    await _roleRepository.BatchDeleteAsync(r => r.Id > 0);

                    var roleCount = _roleRepository.Count();
                    roleCount.ShouldBe(0);
                }

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Update_All_Tenant_Roles_When_Using_BatchUpdate()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                // Act
                await _roleRepository.BatchUpdateAsync(r => new Role { DisplayName = "Test" }, r => r.Id > 0);

                // Assert
                var roleCount = _roleRepository.Count(r => r.DisplayName == "Test");
                roleCount.ShouldBe(4);

                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    // Assert
                    roleCount = _roleRepository.Count(r => r.DisplayName == "Test");
                    roleCount.ShouldBe(0);
                }

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Update_Filtered_Entities_Using_BatchUpdate()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                // Act
                await _roleRepository.BatchUpdateAsync(r => new Role { DisplayName = "Test" }, e => e.Name != "ADMIN");

                // Assert
                var roleCount = _roleRepository.Count(r => r.DisplayName == "Test");
                roleCount.ShouldBe(3);

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Use_MayHaveTenant_Filter_When_Updating_Entities_Using_BatchUpdate()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                await _roleRepository.BatchUpdateAsync(r => new Role { DisplayName = "Test" }, r => r.Id > 0);

                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    var admin = await _roleRepository.FirstOrDefaultAsync(r => r.DisplayName == "Admin");
                    admin.ShouldNotBeNull();
                }

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Update_All_Roles_When_MayHaveTenant_Filter_Is_Disabled_When_Using_BatchUpdate()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    await _roleRepository.BatchUpdateAsync(r => new Role { DisplayName = "Test" }, r => r.Id > 0);

                    var roleCount = _roleRepository.Count(r => r.DisplayName == "Test");
                    roleCount.ShouldBe(5);
                }

                await uow.CompleteAsync();
            }
        }
    }
}
