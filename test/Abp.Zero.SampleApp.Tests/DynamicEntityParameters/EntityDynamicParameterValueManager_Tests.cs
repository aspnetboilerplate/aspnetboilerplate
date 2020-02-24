using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.DynamicEntityParameters;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityParameters
{
    public class EntityDynamicParameterValueManager_Tests : DynamicEntityParametersTestBase
    {
        private readonly IEntityDynamicParameterValueManager _entityDynamicParameterValueManager;

        public EntityDynamicParameterValueManager_Tests()
        {
            _entityDynamicParameterValueManager = Resolve<IEntityDynamicParameterValueManager>();
        }

        [Fact]
        public void Should_Add_Parameter_Value()
        {
            var entityDynamicParameter = CreateAndGetEntityDynamicParameter();
            var val = new EntityDynamicParameterValue()
            {
                EntityDynamicParameterId = entityDynamicParameter.Id,
                EntityRowId = "123",
                Value = "TestValue"
            };

            RunAndCheckIfPermissionControlled(() =>
            {
                _entityDynamicParameterValueManager.Add(val);
            });

            WithUnitOfWork(() =>
            {
                var val2 = _entityDynamicParameterValueManager.Get(val.Id);

                val.ShouldNotBeNull();
                val2.ShouldNotBeNull();

                val.EntityDynamicParameterId.ShouldBe(val2.EntityDynamicParameterId);
                val.EntityRowId.ShouldBe(val2.EntityRowId);
                val.Value.ShouldBe(val2.Value);
            });
        }

        [Fact]
        public void Should_Update_Parameter_Value()
        {
            var entityDynamicParameter = CreateAndGetEntityDynamicParameter();
            var parameterValue = new EntityDynamicParameterValue()
            {
                EntityDynamicParameterId = entityDynamicParameter.Id,
                EntityRowId = "123",
                Value = "TestValue"
            };

            WithUnitOfWork(() => { _entityDynamicParameterValueManager.Add(parameterValue); });

            parameterValue.Value = "TestValue2";
            RunAndCheckIfPermissionControlled(() =>
            {
                _entityDynamicParameterValueManager.Update(parameterValue);
            });

            WithUnitOfWork(() =>
            {
                _entityDynamicParameterValueManager.Add(parameterValue);
            });

            RunAndCheckIfPermissionControlled(() =>
            {
                parameterValue.Value = "TestValue2";
                _entityDynamicParameterValueManager.Update(parameterValue);
            });

            WithUnitOfWork(() =>
            {
                var parameterValueLatest = _entityDynamicParameterValueManager.Get(parameterValue.Id);
                parameterValueLatest.Value.ShouldBe("TestValue2");
                parameterValueLatest.EntityRowId.ShouldBe(parameterValue.EntityRowId);
                parameterValueLatest.EntityDynamicParameterId.ShouldBe(parameterValue.EntityDynamicParameterId);
            });
        }

        [Fact]
        public void Should_Delete_Parameter_Value()
        {
            var entityDynamicParameter = CreateAndGetEntityDynamicParameter();
            var parameterValue = new EntityDynamicParameterValue()
            {
                EntityDynamicParameterId = entityDynamicParameter.Id,
                EntityRowId = "123",
                Value = "TestValue"
            };

            WithUnitOfWork(() =>
            {
                _entityDynamicParameterValueManager.Add(parameterValue);
            });

            RunAndCheckIfPermissionControlled(() =>
            {
                _entityDynamicParameterValueManager.Delete(parameterValue.Id);
            });

            WithUnitOfWork(() =>
            {
                try
                {
                    var dynamicParameterValue = _entityDynamicParameterValueManager.Get(parameterValue.Id);
                    dynamicParameterValue.ShouldBeNull();
                }
                catch (EntityNotFoundException)
                {
                }
            });
        }
        
        [Fact]
        public async Task Should_Add_Parameter_Value_Async()
        {
            var entityDynamicParameter = CreateAndGetEntityDynamicParameter();
            var val = new EntityDynamicParameterValue()
            {
                EntityDynamicParameterId = entityDynamicParameter.Id,
                EntityRowId = "123",
                Value = "TestValue"
            };

            await RunAndCheckIfPermissionControlledAsync(() => _entityDynamicParameterValueManager.AddAsync(val));

            await WithUnitOfWorkAsync(async () =>
             {
                 var val2 = await _entityDynamicParameterValueManager.GetAsync(val.Id);
                 val.ShouldNotBeNull();
                 val2.ShouldNotBeNull();

                 val.EntityDynamicParameterId.ShouldBe(val2.EntityDynamicParameterId);
                 val.EntityRowId.ShouldBe(val2.EntityRowId);
                 val.Value.ShouldBe(val2.Value);
             });
        }

        [Fact]
        public async Task Should_Update_Parameter_Value_Async()
        {
            var entityDynamicParameter = CreateAndGetEntityDynamicParameter();
            var parameterValue = new EntityDynamicParameterValue()
            {
                EntityDynamicParameterId = entityDynamicParameter.Id,
                EntityRowId = "123",
                Value = "TestValue"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _entityDynamicParameterValueManager.AddAsync(parameterValue);
            });

            await RunAndCheckIfPermissionControlledAsync(async () =>
             {
                 parameterValue.Value = "TestValue2";
                 await _entityDynamicParameterValueManager.UpdateAsync(parameterValue);
             });

            await WithUnitOfWorkAsync(async () =>
            {
                var parameterValueLatest = await _entityDynamicParameterValueManager.GetAsync(parameterValue.Id);
                parameterValueLatest.Value.ShouldBe("TestValue2");
                parameterValueLatest.EntityRowId.ShouldBe(parameterValue.EntityRowId);
                parameterValueLatest.EntityDynamicParameterId.ShouldBe(parameterValue.EntityDynamicParameterId);
            });
        }

        [Fact]
        public async Task Should_Delete_Parameter_Value_Async()
        {
            var entityDynamicParameter = CreateAndGetEntityDynamicParameter();
            var parameterValue = new EntityDynamicParameterValue()
            {
                EntityDynamicParameterId = entityDynamicParameter.Id,
                EntityRowId = "123",
                Value = "TestValue"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _entityDynamicParameterValueManager.AddAsync(parameterValue);
            });

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                await _entityDynamicParameterValueManager.DeleteAsync(parameterValue.Id);
            });

            await WithUnitOfWorkAsync(async () =>
                {
                    try
                    {
                        var dynamicParameterValue = await _entityDynamicParameterValueManager.GetAsync(parameterValue.Id);
                        dynamicParameterValue.ShouldBeNull();
                    }
                    catch (EntityNotFoundException)
                    {
                    }
                });
        }
    }
}
