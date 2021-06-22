using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.DynamicEntityProperties;
using Abp.Threading;
using Microsoft.AspNet.Identity;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityProperties
{
    public class DynamicEntityPropertyValueManager_Tests : DynamicEntityPropertiesTestBase
    {
        private readonly IDynamicEntityPropertyValueManager _dynamicEntityPropertyValueManager;

        public DynamicEntityPropertyValueManager_Tests()
        {
            _dynamicEntityPropertyValueManager = Resolve<IDynamicEntityPropertyValueManager>();
        }

        [Fact]
        public void Should_Add_Property_Value()
        {
            var dynamicEntityProperty = CreateAndGetDynamicEntityProperty();
            var val = new DynamicEntityPropertyValue()
            {
                DynamicEntityPropertyId = dynamicEntityProperty.Id,
                EntityId = "123",
                Value = "TestValue",
                TenantId = AbpSession.TenantId
            };

            RunAndCheckIfPermissionControlled(() => { _dynamicEntityPropertyValueManager.Add(val); });

            WithUnitOfWork(() =>
            {
                var val2 = _dynamicEntityPropertyValueManager.Get(val.Id);

                val.ShouldNotBeNull();
                val2.ShouldNotBeNull();

                val.DynamicEntityPropertyId.ShouldBe(val2.DynamicEntityPropertyId);
                val.EntityId.ShouldBe(val2.EntityId);
                val.Value.ShouldBe(val2.Value);
            });
        }

        [Fact]
        public void Should_Update_Property_Value()
        {
            var dynamicEntityProperty = CreateAndGetDynamicEntityProperty();
            var propertyValue = new DynamicEntityPropertyValue()
            {
                DynamicEntityPropertyId = dynamicEntityProperty.Id,
                EntityId = "123",
                Value = "TestValue",
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() => { _dynamicEntityPropertyValueManager.Add(propertyValue); });

            propertyValue.Value = "TestValue2";
            RunAndCheckIfPermissionControlled(() => { _dynamicEntityPropertyValueManager.Update(propertyValue); });

            WithUnitOfWork(() =>
            {
                var propertyValueLatest = _dynamicEntityPropertyValueManager.Get(propertyValue.Id);
                propertyValueLatest.Value.ShouldBe("TestValue2");
                propertyValueLatest.EntityId.ShouldBe(propertyValue.EntityId);
                propertyValueLatest.DynamicEntityPropertyId.ShouldBe(propertyValue.DynamicEntityPropertyId);
            });
        }

        [Fact]
        public void Should_Delete_Property_Value()
        {
            var dynamicEntityProperty = CreateAndGetDynamicEntityProperty();
            var propertyValue = new DynamicEntityPropertyValue()
            {
                DynamicEntityPropertyId = dynamicEntityProperty.Id,
                EntityId = "123",
                Value = "TestValue",
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() => { _dynamicEntityPropertyValueManager.Add(propertyValue); });

            RunAndCheckIfPermissionControlled(() => { _dynamicEntityPropertyValueManager.Delete(propertyValue.Id); });

            WithUnitOfWork(() =>
            {
                try
                {
                    var dynamicEntityPropertyValue = _dynamicEntityPropertyValueManager.Get(propertyValue.Id);
                    dynamicEntityPropertyValue.ShouldBeNull();
                }
                catch (EntityNotFoundException)
                {
                }
            });
        }

        [Fact]
        public async Task Should_Add_Property_Value_Async()
        {
            var dynamicEntityProperty = CreateAndGetDynamicEntityProperty();
            var val = new DynamicEntityPropertyValue()
            {
                DynamicEntityPropertyId = dynamicEntityProperty.Id,
                EntityId = "123",
                Value = "TestValue",
                TenantId = AbpSession.TenantId
            };

            await RunAndCheckIfPermissionControlledAsync(() => _dynamicEntityPropertyValueManager.AddAsync(val));

            await WithUnitOfWorkAsync(async () =>
            {
                var val2 = await _dynamicEntityPropertyValueManager.GetAsync(val.Id);
                val.ShouldNotBeNull();
                val2.ShouldNotBeNull();

                val.DynamicEntityPropertyId.ShouldBe(val2.DynamicEntityPropertyId);
                val.EntityId.ShouldBe(val2.EntityId);
                val.Value.ShouldBe(val2.Value);
            });
        }

        [Fact]
        public async Task Should_Update_Property_Value_Async()
        {
            var dynamicEntityProperty = CreateAndGetDynamicEntityProperty();
            var propertyValue = new DynamicEntityPropertyValue()
            {
                DynamicEntityPropertyId = dynamicEntityProperty.Id,
                EntityId = "123",
                Value = "TestValue",
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () => { await _dynamicEntityPropertyValueManager.AddAsync(propertyValue); });

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                propertyValue.Value = "TestValue2";
                await _dynamicEntityPropertyValueManager.UpdateAsync(propertyValue);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var propertyValueLatest = await _dynamicEntityPropertyValueManager.GetAsync(propertyValue.Id);
                propertyValueLatest.Value.ShouldBe("TestValue2");
                propertyValueLatest.EntityId.ShouldBe(propertyValue.EntityId);
                propertyValueLatest.DynamicEntityPropertyId.ShouldBe(propertyValue.DynamicEntityPropertyId);
            });
        }

        [Fact]
        public async Task Should_Delete_Property_Value_Async()
        {
            var dynamicEntityProperty = CreateAndGetDynamicEntityProperty();
            var propertyValue = new DynamicEntityPropertyValue()
            {
                DynamicEntityPropertyId = dynamicEntityProperty.Id,
                EntityId = "123",
                Value = "TestValue",
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () => { await _dynamicEntityPropertyValueManager.AddAsync(propertyValue); });

            await RunAndCheckIfPermissionControlledAsync(async () => { await _dynamicEntityPropertyValueManager.DeleteAsync(propertyValue.Id); });

            await WithUnitOfWorkAsync(async () =>
            {
                try
                {
                    var dynamicEntityPropertyValue = await _dynamicEntityPropertyValueManager.GetAsync(propertyValue.Id);
                    dynamicEntityPropertyValue.ShouldBeNull();
                }
                catch (EntityNotFoundException)
                {
                }
            });
        }

        private (DynamicEntityProperty dynamicEntityProperty, List<DynamicEntityPropertyValue> values) AddTestItems(int loop = 3, string rowId = "123")
        {
            var dynamicEntityProperty = CreateAndGetDynamicEntityProperty();

            var user = UserManager.FindById(AbpSession.UserId.Value);
            GrantPermission(user, TestPermission);

            var items = new List<DynamicEntityPropertyValue>();
            for (int i = 0; i < loop; i++)
            {
                WithUnitOfWork(() =>
                {
                    var item = new DynamicEntityPropertyValue()
                    {
                        DynamicEntityPropertyId = dynamicEntityProperty.Id,
                        EntityId = rowId,
                        Value = "TestValue",
                        TenantId = AbpSession.TenantId
                    };
                    _dynamicEntityPropertyValueManager.Add(item);

                    items.Add(item);
                });
            }

            return (dynamicEntityProperty, items);
        }

        private void CheckEquality(DynamicEntityPropertyValue value1, DynamicEntityPropertyValue value2)
        {
            value1.ShouldNotBeNull();
            value2.ShouldNotBeNull();

            value1.Id.ShouldBe(value2.Id);
            value1.DynamicEntityPropertyId.ShouldBe(value2.DynamicEntityPropertyId);
            value1.EntityId.ShouldBe(value2.EntityId);
            value1.Value.ShouldBe(value2.Value);
            value1.TenantId.ShouldBe(value2.TenantId);
        }

        private void CheckIfSequencesEqual(List<DynamicEntityPropertyValue> values1, List<DynamicEntityPropertyValue> values2)
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
        public async Task Should_Get_All_Values_Async_With_dynamicEntityPropertyId_entityId()
        {
            var (dynamicEntityProperty, values) = AddTestItems();

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                var list = await _dynamicEntityPropertyValueManager.GetValuesAsync(dynamicEntityPropertyId: dynamicEntityProperty.Id, entityId: "123");
                CheckIfSequencesEqual(list, values);
            });
        }

        [Fact]
        public void Should_Get_All_Values_With_dynamicEntityPropertyId_entityId()
        {
            var (dynamicEntityProperty, values) = AddTestItems();

            RunAndCheckIfPermissionControlled(() =>
            {
                var list = _dynamicEntityPropertyValueManager.GetValues(dynamicEntityPropertyId: dynamicEntityProperty.Id, entityId: "123");
                CheckIfSequencesEqual(list, values);
            });
        }

        [Fact]
        public async Task Should_Get_All_Values_Async_With_entityFullName_entityId()
        {
            var (dynamicEntityProperty, values) = AddTestItems();

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                var list = await _dynamicEntityPropertyValueManager.GetValuesAsync(entityFullName: dynamicEntityProperty.EntityFullName, entityId: "123");
                CheckIfSequencesEqual(list, values);
            });
        }

        [Fact]
        public void Should_Get_All_Values_With_entityFullName_entityId()
        {
            var testItems = AddTestItems();

            RunAndCheckIfPermissionControlled(() =>
            {
                var list = _dynamicEntityPropertyValueManager.GetValues(entityFullName: testItems.dynamicEntityProperty.EntityFullName, entityId: "123");
                CheckIfSequencesEqual(list, testItems.values);
            });
        }

        [Fact]
        public async Task Should_Get_All_Values_Async_With_entityFullName_entityId_dynamicPropertyId()
        {
            var (dynamicEntityProperty, values) = AddTestItems();

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                var list = await _dynamicEntityPropertyValueManager.GetValuesAsync(
                    entityFullName: dynamicEntityProperty.EntityFullName,
                    entityId: "123",
                    dynamicPropertyId: dynamicEntityProperty.DynamicPropertyId);
                CheckIfSequencesEqual(list, values);
            });
        }

        [Fact]
        public void Should_Get_All_Values_With_entityFullName_entityId_dynamicPropertyId()
        {
            var (dynamicEntityProperty, values) = AddTestItems();

            RunAndCheckIfPermissionControlled(() =>
            {
                var list = _dynamicEntityPropertyValueManager.GetValues(
                    entityFullName: dynamicEntityProperty.EntityFullName,
                    entityId: "123",
                    dynamicPropertyId: dynamicEntityProperty.DynamicPropertyId);
                CheckIfSequencesEqual(list, values);
            });
        }

        [Fact]
        public async Task Should_Get_All_Values_Async_With_entityFullName_entityId_propertyName()
        {
            var (dynamicEntityProperty, values) = AddTestItems();

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                var list = await _dynamicEntityPropertyValueManager.GetValuesAsync(
                    entityFullName: dynamicEntityProperty.EntityFullName,
                    entityId: "123",
                    propertyName: values.First().DynamicEntityProperty.DynamicProperty.PropertyName);
                CheckIfSequencesEqual(list, values);
            });
        }

        [Fact]
        public void Should_Get_All_Values_With_entityFullName_entityId_propertyName()
        {
            var (dynamicEntityProperty, values) = AddTestItems();

            RunAndCheckIfPermissionControlled(() =>
            {
                var list = _dynamicEntityPropertyValueManager.GetValues(
                    entityFullName: dynamicEntityProperty.EntityFullName,
                    entityId: "123",
                    propertyName: values.First().DynamicEntityProperty.DynamicProperty.PropertyName);
                CheckIfSequencesEqual(list, values);
            });
        }

        [Fact]
        public void Should_Clean_Values()
        {
            var (dynamicEntityProperty, _) = AddTestItems();

            RunAndCheckIfPermissionControlled(() => { _dynamicEntityPropertyValueManager.CleanValues(dynamicEntityProperty.Id, "123"); });

            WithUnitOfWork(() =>
            {
                var items = _dynamicEntityPropertyValueManager.GetValues(dynamicEntityProperty.Id, "123");
                items.ShouldBeEmpty();
            });
        }

        [Fact]
        public async Task Should_Clean_Values_Async()
        {
            var (dynamicEntityProperty, _) = AddTestItems();

            await RunAndCheckIfPermissionControlledAsync(async () => { await _dynamicEntityPropertyValueManager.CleanValuesAsync(dynamicEntityProperty.Id, "123"); });

            WithUnitOfWork(() =>
            {
                var items = _dynamicEntityPropertyValueManager.GetValues(dynamicEntityProperty.Id, "123");
                items.ShouldBeEmpty();
            });
        }
    }
}
