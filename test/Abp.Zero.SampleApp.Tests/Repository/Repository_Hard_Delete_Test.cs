using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.SampleApp.EntityHistory;
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

                await uow.CompleteAsync();
            }

            using (var uow = uowManager.Begin())
            {
                var users = await _useRepository.GetAllListAsync();

                foreach (var user in users)
                {
                    await _useRepository.HardDeleteAsync(user);
                }
                
                await uow.CompleteAsync();
            }

            using (var uow = uowManager.Begin())
            {
                using (uowManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    var users = await _useRepository.GetAllListAsync();
                    users.Count.ShouldBe(1);
                    users.First().UserName.ShouldBe("admin");
                }

                await uow.CompleteAsync();
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

                await uow.CompleteAsync();
            }

            using (var uow = uowManager.Begin())
            {
                await _useRepository.HardDeleteAsync(u => u.Id > 0);

                await uow.CompleteAsync();
            }

            using (var uow = uowManager.Begin())
            {
                using (uowManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    var users = await _useRepository.GetAllListAsync();
                    users.Count.ShouldBe(1);
                    users.First().UserName.ShouldBe("admin");
                }

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Throw_Exception_If_There_No_UnitOfWork()
        {
            var admin = await _useRepository.FirstOrDefaultAsync(u => u.UserName == "admin");

            Assert.Throws<AbpException>(() => _useRepository.HardDelete(admin));
            Assert.Throws<AbpException>(() => _useRepository.HardDelete(u => u.Id > 0));

            await Assert.ThrowsAsync<AbpException>(async () => await _useRepository.HardDeleteAsync(admin));
            await Assert.ThrowsAsync<AbpException>(async () => await _useRepository.HardDeleteAsync(u => u.Id > 0));
        }
        
        [Fact]
        public async Task Should_Not_Throw_Exception_When_Deleting_ValueObject_And_HardDeleting_Entity()
        {
            AbpSession.TenantId = 1;

            var uowManager = Resolve<IUnitOfWorkManager>();
            
            UsingDbContext(context =>
            {
                context.Categories.Add(new Category
                {
                    DisplayName = "Soft Drinks"
                });
                
                context.SaveChanges();
            });
            
            using (var uow = uowManager.Begin())
            {
                var admin = await _useRepository.FirstOrDefaultAsync(u => u.UserName == "admin");
                await _useRepository.HardDeleteAsync(admin);
 
                UsingDbContext(context =>
                {
                    var category = context.Categories.Single(e=> e.DisplayName == "Soft Drinks");
                    context.Categories.Remove(category);
                });
                
                await uow.CompleteAsync();
            }
        }
    }
}
