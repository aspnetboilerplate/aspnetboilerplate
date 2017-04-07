using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abp.EntityFramework.GraphDiff.Mapping;
using Abp.EntityFramework.GraphDIff.Tests.Entities;
using RefactorThis.GraphDiff;
using Shouldly;
using Xunit;

namespace Abp.EntityFramework.GraphDIff.Tests.Mapping
{
    public class EntityMappingManager_Tests : AbpEntityFrameworkGraphDiffTestBase
    {

        private readonly IEntityMappingManager _entityMappingManager;

        public EntityMappingManager_Tests()
        {
            _entityMappingManager = LocalIocManager.Resolve<IEntityMappingManager>();
        }

        [Fact]
        public void Mapping_Manager_Should_Be_Registered_Automatically_With_The_Assembly()
        {
            _entityMappingManager.ShouldNotBeNull();
        }

        [Fact]
        public void Should_Get_Mapping_For_Each_Entity()
        {
            var mainEntityMapping = _entityMappingManager.GetEntityMappingOrNull<MyMainEntity>();
            var dependentEntityMapping = _entityMappingManager.GetEntityMappingOrNull<MyDependentEntity>();

            Expression<Func<IUpdateConfiguration<MyMainEntity>, object>> expectedMainExrepssion =
                config => config.AssociatedCollection(entity => entity.MyDependentEntities);
            Expression<Func<IUpdateConfiguration<MyDependentEntity>, object>> expectedDependentExpression =
                config => config.AssociatedEntity(entity => entity.MyMainEntity);

            //Mappings shouldn't be null as they are configured
            mainEntityMapping.ShouldNotBeNull();
            dependentEntityMapping.ShouldNotBeNull();

            //Assert that string representation of mappings are equal
            mainEntityMapping.ToString().ShouldBe(expectedMainExrepssion.ToString());
            dependentEntityMapping.ToString().ShouldBe(expectedDependentExpression.ToString());
        }

        [Fact]
        public void Should_Get_Null_If_Mapping_For_Entity_Does_Not_Exist()
        {
            _entityMappingManager.GetEntityMappingOrNull<MyUnmappedEntity>().ShouldBeNull();
        }
    }
}
