using System;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityHistory;
using Abp.ZeroCore.SampleApp.Core.EntityHistory;
using Shouldly;
using Xunit;

namespace Abp.Zero.EntityHistory
{
    public class EntitySnapshotManager_Tests : AbpZeroTestBase
    {
        private readonly IRepository<UserTestEntity> _userRepository;
        private readonly IEntitySnapshotManager _entitySnapshotManager;

        public EntitySnapshotManager_Tests()
        {
            _userRepository = Resolve<IRepository<UserTestEntity>>();
            _entitySnapshotManager = Resolve<IEntitySnapshotManager>();
        }

        [Fact]
        [UnitOfWork]
        public async Task Test_GetEntitySnapshotAsync()
        {
            var id = CreateUserAndGetId();

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var snapshot = await _entitySnapshotManager.GetSnapshotAsync<UserTestEntity, int>(id, DateTime.Now);
                snapshot.ChangedPropertiesSnapshots.Count.ShouldBe(0);
                snapshot.PropertyChangesStackTree.Count.ShouldBe(0);

                uow.Complete();
            }

            Thread.Sleep(3 * 1000);
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var user = _userRepository.Get(id);
                user.Name = "test-user-name-updated";
                user.Surname = "test-user-surname-updated";
                
                uow.Complete();
            }

            Thread.Sleep(3 * 1000);
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var user = _userRepository.Get(id);
                user.Name = "test-user-name-updated-2";
                
                uow.Complete();
            }

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                //undo last update
                var snapshot = await _entitySnapshotManager.GetSnapshotAsync<UserTestEntity, int>(id, DateTime.Now.AddSeconds(-2));

                snapshot.ChangedPropertiesSnapshots.Count.ShouldBe(1);
                snapshot.PropertyChangesStackTree.Count.ShouldBe(1);

                snapshot["Name"].ShouldBe("\"test-user-name-updated\"");
                snapshot.PropertyChangesStackTree["Name"].ShouldBe("\"test-user-name-updated-2\" -> \"test-user-name-updated\"");



                //undo all changes
                var snapshot2 = await _entitySnapshotManager.GetSnapshotAsync<UserTestEntity>(id, DateTime.Now.AddDays(-1));

                snapshot2.ChangedPropertiesSnapshots.Count.ShouldBe(2);
                snapshot2.PropertyChangesStackTree.Count.ShouldBe(2);

                snapshot2["Name"].ShouldBe("\"test-user-name-start\"");
                snapshot2["Surname"].ShouldBe("\"test-user-surname-start\"");
                snapshot2.PropertyChangesStackTree["Name"].ShouldBe("\"test-user-name-updated-2\" -> \"test-user-name-updated\" -> \"test-user-name-start\"");
                snapshot2.PropertyChangesStackTree["Surname"].ShouldBe("\"test-user-surname-updated\" -> \"test-user-surname-start\"");
            }
        }

        private int CreateUserAndGetId()
        {
            int userId;

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var user = new UserTestEntity
                {
                    Name = "test-user-name-start", 
                    Surname = "test-user-surname-start", 
                    Age = 18
                };

                userId = _userRepository.InsertAndGetId(user);

                uow.Complete();
            }

            return userId;
        }
    }
}
