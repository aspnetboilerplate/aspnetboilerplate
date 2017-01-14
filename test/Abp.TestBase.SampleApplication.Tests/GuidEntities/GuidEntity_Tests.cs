using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.TestBase.SampleApplication.GuidEntities;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.GuidEntities
{
    public class GuidEntity_Tests : SampleApplicationTestBase
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<TestEntityWithGuidPk, Guid> _testEntityWithGuidPkRepository;
        private readonly IRepository<TestEntityWithGuidPkAndDbGeneratedValue, Guid> _testEntityWithGuidPkAndDbGeneratedValueRepository;

        public GuidEntity_Tests()
        {
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _testEntityWithGuidPkRepository = Resolve<IRepository<TestEntityWithGuidPk, Guid>>();
            _testEntityWithGuidPkAndDbGeneratedValueRepository = Resolve<IRepository<TestEntityWithGuidPkAndDbGeneratedValue, Guid>>();
        }

        [Fact]
        public void Should_Set_Id_On_Insert_For_Non_DbGenerated()
        {
            //Arrange
            var entity = new TestEntityWithGuidPk();
            Guid assignedId;

            //Act
            using (var uow = _unitOfWorkManager.Begin())
            {
                _testEntityWithGuidPkRepository.Insert(entity);

                //Assert: It should be set
                assignedId = entity.Id;
                assignedId.ShouldNotBe(Guid.Empty);

                uow.Complete();
            }

            //Assert: It should still be the same
            entity.Id.ShouldBe(assignedId);
        }

        [Fact]
        public void Should_Not_Set_Id_On_Insert_For_DbGenerated()
        {
            //Arrange
            var entity = new TestEntityWithGuidPkAndDbGeneratedValue();

            //Act
            using (var uow = _unitOfWorkManager.Begin())
            {
                _testEntityWithGuidPkAndDbGeneratedValueRepository.Insert(entity);

                //Assert: It should not be set yet, since UOW is not completed
                entity.Id.ShouldBe(Guid.Empty);

                uow.Complete();
            }

            //Assert: It should be assigned by database
            entity.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public void Should_Set_Id_On_InsertAndGetId_For_DbGenerated()
        {
            //Arrange
            var entity = new TestEntityWithGuidPkAndDbGeneratedValue();

            //Act
            using (var uow = _unitOfWorkManager.Begin())
            {
                _testEntityWithGuidPkAndDbGeneratedValueRepository.InsertAndGetId(entity);

                //Assert: It should be set yet, since InsertAndGetId saved to database
                entity.Id.ShouldNotBe(Guid.Empty);

                uow.Complete();
            }
        }
    }
}
