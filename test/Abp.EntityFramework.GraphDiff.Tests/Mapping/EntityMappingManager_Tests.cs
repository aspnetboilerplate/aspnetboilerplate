using System.Collections.Generic;
using System.Data.Entity;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.GraphDiff.Extensions;
using Abp.GraphDiff.Mapping;
using Abp.Tests;
using NSubstitute;
using RefactorThis.GraphDiff;
using Shouldly;
using Xunit;

namespace Abp.EntityFramework.GraphDIff.Tests.Mapping
{
    public class EntityMappingManager_Tests
    {
        private readonly IEntityMappingManager _entityMappingManager;
        private readonly IRepository<MyMainEntity> _mainEntityRepository;
        private readonly IRepository<MyDependentEntity> _dependentEntityRepository;

        public EntityMappingManager_Tests()
        {
            _entityMappingManager = Substitute.For<IEntityMappingManager>();
            _mainEntityRepository = Substitute.For<IRepository<MyMainEntity>>();
            _dependentEntityRepository = Substitute.For<IRepository<MyDependentEntity>>();
        }

        public class MyMappingProvider1 : EntityMappingProvider
        {
            public override IEnumerable<EntityMapping> GetEntityMappings()
            {
                var mappings = new List<EntityMapping>
                {
                    MappingExpressionBuilder.For<MyMainEntity>(config => config.AssociatedCollection(entity => entity.MyDependentEntities)),
                    MappingExpressionBuilder.For<MyDependentEntity>(config => config.AssociatedEntity(entity => entity.MyMainEntity))
                };
                return mappings;
            }
        }

        public class MyMainDbContext : AbpDbContext
        {
            public virtual DbSet<MyMainEntity> MainEntities { get; set; }

            public virtual DbSet<MyDependentEntity> DependentEntities { get; set; }
        }

        public class MyMainEntity : Entity
        {
            public virtual ICollection<MyDependentEntity> MyDependentEntities { get; set; }
        }

        public class MyDependentEntity : Entity
        {
            public virtual MyMainEntity MyMainEntity { get; set; }
        }

        [Fact]
        public void Should_Change_Dependent_Entities_Using_GraphDiff()
        {
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            {
                //Insert few entities
                var mainEntity1 = _mainEntityRepository.Insert(new MyMainEntity()); //Id: 1
                var mainEntity2 = _mainEntityRepository.Insert(new MyMainEntity()); //Id: 2
                unitOfWorkManager.Current.SaveChanges();

                var dependentEntity1 = new MyDependentEntity { MyMainEntity = mainEntity1 }; //Id: 1
                unitOfWorkManager.Current.SaveChanges();  //dependentEntity1 gets Id 1 and is linked to mainEntity1 using EF change tracker

                //Let's assume that we got it via API (i.e. was deserialized from JSOn format) or was just created manually (not fetched via repository)
                var disattachedEntity1 = new MyDependentEntity
                {
                    Id = 1,
                    MyMainEntity = new MyMainEntity {Id = 2}
                };

                //As a result of graph attachment, we should get old entity with UPDATED nav property (EF would create a new entity as it's disattached);
                var attachedDependentEntity1 = _dependentEntityRepository.AttachGraph(disattachedEntity1);
                unitOfWorkManager.Current.SaveChanges();

                attachedDependentEntity1.MyMainEntity.ShouldBe(mainEntity2); //New entity should be attached with it's navigation property
                dependentEntity1.Id.ShouldBe(attachedDependentEntity1.Id); //As entity was detached (but not deleted), it should be updated (not re-created)
            }
        }
    }
}
