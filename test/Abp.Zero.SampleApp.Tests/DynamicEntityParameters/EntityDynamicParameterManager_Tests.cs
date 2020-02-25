using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.DynamicEntityParameters;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityParameters
{
    public class EntityDynamicParameterManager_Tests : DynamicEntityParametersTestBase
    {
        private readonly IEntityDynamicParameterManager _entityDynamicParameterManager;

        public EntityDynamicParameterManager_Tests()
        {
            _entityDynamicParameterManager = Resolve<IEntityDynamicParameterManager>();
        }

        [Fact]
        public void Should_Add_Parameter()
        {
            var dynamicParameter = CreateAndGetDynamicParameter();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName
            };

            RunAndCheckIfPermissionControlled(() =>
            {
                _entityDynamicParameterManager.Add(entityDynamicParameter);
            });

            WithUnitOfWork(() =>
            {
                var val = _entityDynamicParameterManager.Get(entityDynamicParameter.Id);

                val.ShouldNotBeNull();

                val.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                val.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });
        }

        [Fact]
        public void Should_Update_Parameter()
        {
            var dynamicParameter = CreateAndGetDynamicParameter();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName
            };

            WithUnitOfWork(() =>
            {
                _entityDynamicParameterManager.Add(entityDynamicParameter);
            });

            WithUnitOfWork(() =>
            {
                entityDynamicParameter = _entityDynamicParameterManager.Get(entityDynamicParameter.Id);

                entityDynamicParameter.ShouldNotBeNull();
                entityDynamicParameter.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                entityDynamicParameter.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });

            entityDynamicParameter.EntityFullName = "Test2";

            RunAndCheckIfPermissionControlled(() =>
            {
                _entityDynamicParameterManager.Update(entityDynamicParameter);
            });

            WithUnitOfWork(() =>
            {
                var val = _entityDynamicParameterManager.Get(entityDynamicParameter.Id);

                val.ShouldNotBeNull();

                val.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                val.EntityFullName.ShouldBe("Test2");
            });
        }

        [Fact]
        public void Should_Delete_Parameter()
        {
            var dynamicParameter = CreateAndGetDynamicParameter();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName
            };

            WithUnitOfWork(() =>
            {
                _entityDynamicParameterManager.Add(entityDynamicParameter);
            });

            WithUnitOfWork(() =>
            {
                entityDynamicParameter = _entityDynamicParameterManager.Get(entityDynamicParameter.Id);

                entityDynamicParameter.ShouldNotBeNull();
                entityDynamicParameter.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                entityDynamicParameter.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });

            RunAndCheckIfPermissionControlled(() =>
            {
                _entityDynamicParameterManager.Delete(entityDynamicParameter.Id);
            });

            WithUnitOfWork(() =>
            {
                try
                {
                    var val = _entityDynamicParameterManager.Get(entityDynamicParameter.Id);
                    val.ShouldBeNull();
                }
                catch (EntityNotFoundException)
                {
                }
            });
        }


        [Fact]
        public async Task Should_Add_Parameter_Async()
        {
            var dynamicParameter = CreateAndGetDynamicParameter();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName
            };

            await RunAndCheckIfPermissionControlledAsync(async () =>
             {
                 await _entityDynamicParameterManager.AddAsync(entityDynamicParameter);
             });

            WithUnitOfWork(() =>
            {
                var val = _entityDynamicParameterManager.Get(entityDynamicParameter.Id);

                val.ShouldNotBeNull();

                val.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                val.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });
        }

        [Fact]
        public async Task Should_Update_Parameter_Async()
        {
            var dynamicParameter = CreateAndGetDynamicParameter();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _entityDynamicParameterManager.AddAsync(entityDynamicParameter);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                entityDynamicParameter = await _entityDynamicParameterManager.GetAsync(entityDynamicParameter.Id);

                entityDynamicParameter.ShouldNotBeNull();
                entityDynamicParameter.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                entityDynamicParameter.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });

            entityDynamicParameter.EntityFullName = "Test2";

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                await _entityDynamicParameterManager.UpdateAsync(entityDynamicParameter);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var val = await _entityDynamicParameterManager.GetAsync(entityDynamicParameter.Id);

                val.ShouldNotBeNull();

                val.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                val.EntityFullName.ShouldBe("Test2");
            });
        }

        [Fact]
        public async Task Should_Delete_Parameter_Async()
        {
            var dynamicParameter = CreateAndGetDynamicParameter();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _entityDynamicParameterManager.AddAsync(entityDynamicParameter);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var val = await _entityDynamicParameterManager.GetAsync(entityDynamicParameter.Id);

                val.ShouldNotBeNull();
                val.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                val.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                await _entityDynamicParameterManager.DeleteAsync(entityDynamicParameter.Id);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                try
                {
                    var val = _entityDynamicParameterManager.Get(entityDynamicParameter.Id);
                    val.ShouldBeNull();
                }
                catch (EntityNotFoundException)
                {
                }
            });
        }
    }
}
