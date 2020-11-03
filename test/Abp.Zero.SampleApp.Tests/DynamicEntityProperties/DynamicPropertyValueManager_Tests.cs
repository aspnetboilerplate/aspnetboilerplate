using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.DynamicEntityProperties;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityProperties
{
    public class DynamicPropertyValueManager_Tests : DynamicEntityPropertiesTestBase
    {
        private readonly IDynamicPropertyValueManager _dynamicPropertyValueManager;

        public DynamicPropertyValueManager_Tests()
        {
            _dynamicPropertyValueManager = Resolve<IDynamicPropertyValueManager>();
        }

        private void CheckEquality(DynamicPropertyValue v1, DynamicPropertyValue v2)
        {
            v1.ShouldNotBeNull();
            v2.ShouldNotBeNull();

            v1.DynamicPropertyId.ShouldBe(v2.DynamicPropertyId);
            v1.Value.ShouldBe(v2.Value);
        }

        [Fact]
        public void Should_Get_Value()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyValue = new DynamicPropertyValue()
            {
                DynamicPropertyId = dynamicProperty.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                var d = DynamicPropertyStore.Get(dynamicProperty.Id);
                _dynamicPropertyValueManager.Add(dynamicPropertyValue);
            });

            RunAndCheckIfPermissionControlled(() =>
            {
                var entity = _dynamicPropertyValueManager.Get(dynamicPropertyValue.Id);
                CheckEquality(entity, dynamicPropertyValue);
            });
        }

        [Fact]
        public void Should_Add_Value()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyValue = new DynamicPropertyValue()
            {
                DynamicPropertyId = dynamicProperty.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            RunAndCheckIfPermissionControlled(() => { _dynamicPropertyValueManager.Add(dynamicPropertyValue); });

            WithUnitOfWork(() =>
            {
                var entity = _dynamicPropertyValueManager.Get(dynamicPropertyValue.Id);
                CheckEquality(entity, dynamicPropertyValue);
            });
        }

        [Fact]
        public void Should_Update_Value()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyValue = new DynamicPropertyValue()
            {
                DynamicPropertyId = dynamicProperty.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() => { _dynamicPropertyValueManager.Add(dynamicPropertyValue); });

            WithUnitOfWork(() =>
            {
                dynamicPropertyValue = _dynamicPropertyValueManager.Get(dynamicPropertyValue.Id);
                dynamicPropertyValue.ShouldNotBeNull();
            });

            dynamicPropertyValue.Value = "Test2";

            RunAndCheckIfPermissionControlled(() => { _dynamicPropertyValueManager.Update(dynamicPropertyValue); });

            WithUnitOfWork(() =>
            {
                var entity = _dynamicPropertyValueManager.Get(dynamicPropertyValue.Id);
                entity.Value.ShouldBe("Test2");
                entity.DynamicPropertyId.ShouldBe(dynamicProperty.Id);
            });
        }

        [Fact]
        public void Should_Delete_Value()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyValue = new DynamicPropertyValue()
            {
                DynamicPropertyId = dynamicProperty.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() => { _dynamicPropertyValueManager.Add(dynamicPropertyValue); });

            RunAndCheckIfPermissionControlled(() => { _dynamicPropertyValueManager.Delete(dynamicPropertyValue.Id); });

            WithUnitOfWork(() =>
            {
                try
                {
                    var entity = _dynamicPropertyValueManager.Get(dynamicPropertyValue.Id);
                    entity.ShouldBeNull();
                }
                catch (EntityNotFoundException)
                {
                }
            });
        }

        [Fact]
        public void Should_Clean_Value()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyValue = new DynamicPropertyValue()
            {
                DynamicPropertyId = dynamicProperty.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            var dynamicPropertyValue2 = new DynamicPropertyValue()
            {
                DynamicPropertyId = dynamicProperty.Id,
                Value = "Test2",
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                _dynamicPropertyValueManager.Add(dynamicPropertyValue);
                _dynamicPropertyValueManager.Add(dynamicPropertyValue2);
            });

            RunAndCheckIfPermissionControlled(() => { _dynamicPropertyValueManager.CleanValues(dynamicProperty.Id); });

            WithUnitOfWork(() =>
            {
                var entity = _dynamicPropertyValueManager.GetAllValuesOfDynamicProperty(dynamicProperty.Id);
                entity.ShouldBeEmpty();
            });
        }

        [Fact]
        public async Task Should_Get_Value_Async()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyValue = new DynamicPropertyValue()
            {
                DynamicPropertyId = dynamicProperty.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () => { await _dynamicPropertyValueManager.AddAsync(dynamicPropertyValue); });

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                var entity = await _dynamicPropertyValueManager.GetAsync(dynamicPropertyValue.Id);
                CheckEquality(entity, dynamicPropertyValue);
            });
        }

        [Fact]
        public async Task Should_Add_Value_Async()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyValue = new DynamicPropertyValue()
            {
                DynamicPropertyId = dynamicProperty.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            await RunAndCheckIfPermissionControlledAsync(async () => { await _dynamicPropertyValueManager.AddAsync(dynamicPropertyValue); });

            await WithUnitOfWorkAsync(async () =>
            {
                var entity = await _dynamicPropertyValueManager.GetAsync(dynamicPropertyValue.Id);
                CheckEquality(entity, dynamicPropertyValue);
            });
        }

        [Fact]
        public async Task Should_Update_Value_Async()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyValue = new DynamicPropertyValue()
            {
                DynamicPropertyId = dynamicProperty.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () => { await _dynamicPropertyValueManager.AddAsync(dynamicPropertyValue); });

            await WithUnitOfWorkAsync(async () =>
            {
                dynamicPropertyValue = await _dynamicPropertyValueManager.GetAsync(dynamicPropertyValue.Id);
                dynamicPropertyValue.ShouldNotBeNull();
            });

            dynamicPropertyValue.Value = "Test2";

            await RunAndCheckIfPermissionControlledAsync(async () => { await _dynamicPropertyValueManager.UpdateAsync(dynamicPropertyValue); });

            await WithUnitOfWorkAsync(async () =>
            {
                var entity = await _dynamicPropertyValueManager.GetAsync(dynamicPropertyValue.Id);
                entity.Value.ShouldBe("Test2");
                entity.DynamicPropertyId.ShouldBe(dynamicProperty.Id);
            });
        }

        [Fact]
        public async Task Should_Delete_Value_Async()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyValue = new DynamicPropertyValue()
            {
                DynamicPropertyId = dynamicProperty.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () => { await _dynamicPropertyValueManager.AddAsync(dynamicPropertyValue); });

            await RunAndCheckIfPermissionControlledAsync(async () => { await _dynamicPropertyValueManager.DeleteAsync(dynamicPropertyValue.Id); });

            await WithUnitOfWorkAsync(async () =>
            {
                try
                {
                    var entity = await _dynamicPropertyValueManager.GetAsync(dynamicPropertyValue.Id);
                    entity.ShouldBeNull();
                }
                catch (EntityNotFoundException)
                {
                }
            });
        }

        [Fact]
        public async Task Should_Clean_Value_Async()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyValue = new DynamicPropertyValue()
            {
                DynamicPropertyId = dynamicProperty.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            var dynamicPropertyValue2 = new DynamicPropertyValue()
            {
                DynamicPropertyId = dynamicProperty.Id,
                Value = "Test2",
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _dynamicPropertyValueManager.AddAsync(dynamicPropertyValue);
                await _dynamicPropertyValueManager.AddAsync(dynamicPropertyValue2);
            });

            await RunAndCheckIfPermissionControlledAsync(async () => { await _dynamicPropertyValueManager.CleanValuesAsync(dynamicProperty.Id); });

            await WithUnitOfWorkAsync(async () =>
            {
                var entity = await _dynamicPropertyValueManager.GetAllValuesOfDynamicPropertyAsync(dynamicProperty.Id);
                entity.ShouldBeEmpty();
            });
        }
    }
}