using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.DynamicEntityParameters;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityParameters
{
    public class EntityDynamicParameterValueManager_Tests : DynamicEntityParametersTestBase
    {
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
                EntityDynamicParameterValueManager.Add(val);
            });

            WithUnitOfWork(() =>
            {
                var val2 = EntityDynamicParameterValueManager.Get(val.Id);

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

            WithUnitOfWork(() => { EntityDynamicParameterValueManager.Add(parameterValue); });

            parameterValue.Value = "TestValue2";
            RunAndCheckIfPermissionControlled(() =>
            {
                EntityDynamicParameterValueManager.Update(parameterValue);
            });

            WithUnitOfWork(() =>
            {
                EntityDynamicParameterValueManager.Add(parameterValue);
            });

            RunAndCheckIfPermissionControlled(() =>
            {
                parameterValue.Value = "TestValue2";
                EntityDynamicParameterValueManager.Update(parameterValue);
            });

            WithUnitOfWork(() =>
            {
                var parameterValueLatest = EntityDynamicParameterValueManager.Get(parameterValue.Id);
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
                EntityDynamicParameterValueManager.Add(parameterValue);
            });

            RunAndCheckIfPermissionControlled(() =>
            {
                EntityDynamicParameterValueManager.Delete(parameterValue.Id);
            });

            WithUnitOfWork(() =>
            {
                try
                {
                    var dynamicParameterValue = EntityDynamicParameterValueManager.Get(parameterValue.Id);
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

            await RunAndCheckIfPermissionControlledAsync(() => EntityDynamicParameterValueManager.AddAsync(val));

            await WithUnitOfWorkAsync(async () =>
             {
                 var val2 = await EntityDynamicParameterValueManager.GetAsync(val.Id);
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
                await EntityDynamicParameterValueManager.AddAsync(parameterValue);
            });

            await RunAndCheckIfPermissionControlledAsync(async () =>
             {
                 parameterValue.Value = "TestValue2";
                 await EntityDynamicParameterValueManager.UpdateAsync(parameterValue);
             });

            await WithUnitOfWorkAsync(async () =>
            {
                var parameterValueLatest = await EntityDynamicParameterValueManager.GetAsync(parameterValue.Id);
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
                await EntityDynamicParameterValueManager.AddAsync(parameterValue);
            });

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                await EntityDynamicParameterValueManager.DeleteAsync(parameterValue.Id);
            });

            await WithUnitOfWorkAsync(async () =>
                {
                    try
                    {
                        var dynamicParameterValue = await EntityDynamicParameterValueManager.GetAsync(parameterValue.Id);
                        dynamicParameterValue.ShouldBeNull();
                    }
                    catch (EntityNotFoundException)
                    {
                    }
                });
        }
    }
}
