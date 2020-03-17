using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.DynamicEntityParameters;
using Abp.Threading;
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
                EntityId = "123",
                Value = "TestValue",
                TenantId = AbpSession.TenantId
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
                val.EntityId.ShouldBe(val2.EntityId);
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
                EntityId = "123",
                Value = "TestValue",
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() => { _entityDynamicParameterValueManager.Add(parameterValue); });

            parameterValue.Value = "TestValue2";
            RunAndCheckIfPermissionControlled(() =>
            {
                _entityDynamicParameterValueManager.Update(parameterValue);
            });

            WithUnitOfWork(() =>
            {
                var parameterValueLatest = _entityDynamicParameterValueManager.Get(parameterValue.Id);
                parameterValueLatest.Value.ShouldBe("TestValue2");
                parameterValueLatest.EntityId.ShouldBe(parameterValue.EntityId);
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
                EntityId = "123",
                Value = "TestValue",
                TenantId = AbpSession.TenantId
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
                EntityId = "123",
                Value = "TestValue",
                TenantId = AbpSession.TenantId
            };

            await RunAndCheckIfPermissionControlledAsync(() => _entityDynamicParameterValueManager.AddAsync(val));

            await WithUnitOfWorkAsync(async () =>
             {
                 var val2 = await _entityDynamicParameterValueManager.GetAsync(val.Id);
                 val.ShouldNotBeNull();
                 val2.ShouldNotBeNull();

                 val.EntityDynamicParameterId.ShouldBe(val2.EntityDynamicParameterId);
                 val.EntityId.ShouldBe(val2.EntityId);
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
                EntityId = "123",
                Value = "TestValue",
                TenantId = AbpSession.TenantId
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
                parameterValueLatest.EntityId.ShouldBe(parameterValue.EntityId);
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
                EntityId = "123",
                Value = "TestValue",
                TenantId = AbpSession.TenantId
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


        private (EntityDynamicParameter entityDynamicParameter, List<EntityDynamicParameterValue> values) AddTestItems(int loop = 3, string rowId = "123")
        {
            var entityDynamicParameter = CreateAndGetEntityDynamicParameter();

            var user = AsyncHelper.RunSync(() => UserManager.FindByIdAsync(AbpSession.UserId.Value));
            AsyncHelper.RunSync(() => GrantPermissionAsync(user, TestPermission));

            var items = new List<EntityDynamicParameterValue>();
            for (int i = 0; i < loop; i++)
            {
                WithUnitOfWork(() =>
                {
                    var item = new EntityDynamicParameterValue()
                    {
                        EntityDynamicParameterId = entityDynamicParameter.Id,
                        EntityId = rowId,
                        Value = "TestValue",
                        TenantId = AbpSession.TenantId
                    };
                    _entityDynamicParameterValueManager.Add(item);

                    items.Add(item);
                });
            }
            return (entityDynamicParameter, items);
        }

        private void CheckEquality(EntityDynamicParameterValue value1, EntityDynamicParameterValue value2)
        {
            value1.ShouldNotBeNull();
            value2.ShouldNotBeNull();

            value1.Id.ShouldBe(value2.Id);
            value1.EntityDynamicParameterId.ShouldBe(value2.EntityDynamicParameterId);
            value1.EntityId.ShouldBe(value2.EntityId);
            value1.Value.ShouldBe(value2.Value);
            value1.TenantId.ShouldBe(value2.TenantId);
        }

        private void CheckIfSequencesEqual(List<EntityDynamicParameterValue> values1, List<EntityDynamicParameterValue> values2)
        {
            if (!values1.Select(v => v.Id).SequenceEqual(values2.Select(v2 => v2.Id)))
            {
                throw new Exception("Sequences Not Equal");
            }

            foreach (var value1 in values1)
            {
                CheckEquality(value1, values2.Single(value2 => value2.Id == value1.Id));
            }
        }

        [Fact]
        public async Task Should_Get_All_Values_Async_With_entityDynamicParameterId_entityId()
        {
            var testItems = AddTestItems();

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                var list = await _entityDynamicParameterValueManager.GetValuesAsync(entityDynamicParameterId: testItems.entityDynamicParameter.Id, entityId: "123");
                CheckIfSequencesEqual(list, testItems.values);
            });
        }

        [Fact]
        public void Should_Get_All_Values_With_entityDynamicParameterId_entityId()
        {
            var testItems = AddTestItems();

            RunAndCheckIfPermissionControlled(() =>
            {
                var list = _entityDynamicParameterValueManager.GetValues(entityDynamicParameterId: testItems.entityDynamicParameter.Id, entityId: "123");
                CheckIfSequencesEqual(list, testItems.values);
            });
        }

        [Fact]
        public async Task Should_Get_All_Values_Async_With_entityFullName_entityId()
        {
            var testItems = AddTestItems();

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                var list = await _entityDynamicParameterValueManager.GetValuesAsync(entityFullName: testItems.entityDynamicParameter.EntityFullName, entityId: "123");
                CheckIfSequencesEqual(list, testItems.values);
            });
        }

        [Fact]
        public void Should_Get_All_Values_With_entityFullName_entityId()
        {
            var testItems = AddTestItems();

            RunAndCheckIfPermissionControlled(() =>
            {
                var list = _entityDynamicParameterValueManager.GetValues(entityFullName: testItems.entityDynamicParameter.EntityFullName, entityId: "123");
                CheckIfSequencesEqual(list, testItems.values);
            });
        }

        [Fact]
        public async Task Should_Get_All_Values_Async_With_entityFullName_entityId_dynamicParameterId()
        {
            var testItems = AddTestItems();

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                var list = await _entityDynamicParameterValueManager.GetValuesAsync(
                    entityFullName: testItems.entityDynamicParameter.EntityFullName,
                    entityId: "123",
                    dynamicParameterId: testItems.entityDynamicParameter.DynamicParameterId);
                CheckIfSequencesEqual(list, testItems.values);
            });
        }

        [Fact]
        public void Should_Get_All_Values_With_entityFullName_entityId_dynamicParameterId()
        {
            var testItems = AddTestItems();

            RunAndCheckIfPermissionControlled(() =>
            {
                var list = _entityDynamicParameterValueManager.GetValues(
                    entityFullName: testItems.entityDynamicParameter.EntityFullName,
                    entityId: "123",
                    dynamicParameterId: testItems.entityDynamicParameter.DynamicParameterId);
                CheckIfSequencesEqual(list, testItems.values);
            });
        }

        [Fact]
        public async Task Should_Get_All_Values_Async_With_entityFullName_entityId_parameterName()
        {
            var testItems = AddTestItems();

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                var list = await _entityDynamicParameterValueManager.GetValuesAsync(
                    entityFullName: testItems.entityDynamicParameter.EntityFullName,
                    entityId: "123",
                    parameterName: testItems.values.First().EntityDynamicParameter.DynamicParameter.ParameterName);
                CheckIfSequencesEqual(list, testItems.values);
            });
        }

        [Fact]
        public void Should_Get_All_Values_With_entityFullName_entityId_parameterName()
        {
            var testItems = AddTestItems();

            RunAndCheckIfPermissionControlled(() =>
            {
                var list = _entityDynamicParameterValueManager.GetValues(
                    entityFullName: testItems.entityDynamicParameter.EntityFullName,
                    entityId: "123",
                    parameterName: testItems.values.First().EntityDynamicParameter.DynamicParameter.ParameterName);
                CheckIfSequencesEqual(list, testItems.values);
            });
        }

        [Fact]
        public void Should_Clean_Values()
        {
            var testItems = AddTestItems();

            RunAndCheckIfPermissionControlled(() =>
            {
                _entityDynamicParameterValueManager.CleanValues(testItems.entityDynamicParameter.Id, "123");
            });

            WithUnitOfWork(() =>
            {
                var items = _entityDynamicParameterValueManager.GetValues(testItems.entityDynamicParameter.Id, "123");
                items.ShouldBeEmpty();
            });
        }

        [Fact]
        public async Task Should_Clean_Values_Async()
        {
            var testItems = AddTestItems();

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                await _entityDynamicParameterValueManager.CleanValuesAsync(testItems.entityDynamicParameter.Id, "123");
            });

            WithUnitOfWork(() =>
            {
                var items = _entityDynamicParameterValueManager.GetValues(testItems.entityDynamicParameter.Id, "123");
                items.ShouldBeEmpty();
            });
        }
    }
}
