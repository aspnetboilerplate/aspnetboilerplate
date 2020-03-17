using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.DynamicEntityParameters;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityParameters
{
    public class DynamicParameterValueManager_Tests : DynamicEntityParametersTestBase
    {
        private readonly IDynamicParameterValueManager _dynamicParameterValueManager;

        public DynamicParameterValueManager_Tests()
        {
            _dynamicParameterValueManager = Resolve<IDynamicParameterValueManager>();
        }

        private void CheckEquality(DynamicParameterValue v1, DynamicParameterValue v2)
        {
            v1.ShouldNotBeNull();
            v2.ShouldNotBeNull();

            v1.DynamicParameterId.ShouldBe(v2.DynamicParameterId);
            v1.Value.ShouldBe(v2.Value);
        }

        [Fact]
        public void Should_Get_Value()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterValue = new DynamicParameterValue()
            {
                DynamicParameterId = dynamicParameter.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                var d = DynamicParameterStore.Get(dynamicParameter.Id);
                _dynamicParameterValueManager.Add(dynamicParameterValue);
            });

            RunAndCheckIfPermissionControlled(() =>
            {
                var entity = _dynamicParameterValueManager.Get(dynamicParameterValue.Id);
                CheckEquality(entity, dynamicParameterValue);
            });
        }

        [Fact]
        public void Should_Add_Value()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterValue = new DynamicParameterValue()
            {
                DynamicParameterId = dynamicParameter.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            RunAndCheckIfPermissionControlled(() =>
            {
                _dynamicParameterValueManager.Add(dynamicParameterValue);
            });

            WithUnitOfWork(() =>
            {
                var entity = _dynamicParameterValueManager.Get(dynamicParameterValue.Id);
                CheckEquality(entity, dynamicParameterValue);
            });
        }

        [Fact]
        public void Should_Update_Value()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterValue = new DynamicParameterValue()
            {
                DynamicParameterId = dynamicParameter.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                _dynamicParameterValueManager.Add(dynamicParameterValue);
            });

            WithUnitOfWork(() =>
            {
                dynamicParameterValue = _dynamicParameterValueManager.Get(dynamicParameterValue.Id);
                dynamicParameterValue.ShouldNotBeNull();
            });

            dynamicParameterValue.Value = "Test2";

            RunAndCheckIfPermissionControlled(() =>
            {
                _dynamicParameterValueManager.Update(dynamicParameterValue);
            });

            WithUnitOfWork(() =>
            {
                var entity = _dynamicParameterValueManager.Get(dynamicParameterValue.Id);
                entity.Value.ShouldBe("Test2");
                entity.DynamicParameterId.ShouldBe(dynamicParameter.Id);
            });
        }

        [Fact]
        public void Should_Delete_Value()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterValue = new DynamicParameterValue()
            {
                DynamicParameterId = dynamicParameter.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                _dynamicParameterValueManager.Add(dynamicParameterValue);
            });

            RunAndCheckIfPermissionControlled(() =>
            {
                _dynamicParameterValueManager.Delete(dynamicParameterValue.Id);
            });

            WithUnitOfWork(() =>
            {
                try
                {
                    var entity = _dynamicParameterValueManager.Get(dynamicParameterValue.Id);
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
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterValue = new DynamicParameterValue()
            {
                DynamicParameterId = dynamicParameter.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            var dynamicParameterValue2 = new DynamicParameterValue()
            {
                DynamicParameterId = dynamicParameter.Id,
                Value = "Test2",
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                _dynamicParameterValueManager.Add(dynamicParameterValue);
                _dynamicParameterValueManager.Add(dynamicParameterValue2);
            });

            RunAndCheckIfPermissionControlled(() =>
            {
                _dynamicParameterValueManager.CleanValues(dynamicParameter.Id);
            });

            WithUnitOfWork(() =>
            {
                var entity = _dynamicParameterValueManager.GetAllValuesOfDynamicParameter(dynamicParameter.Id);
                entity.ShouldBeEmpty();
            });
        }

        [Fact]
        public async Task Should_Get_Value_Async()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterValue = new DynamicParameterValue()
            {
                DynamicParameterId = dynamicParameter.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () =>
             {
                 await _dynamicParameterValueManager.AddAsync(dynamicParameterValue);
             });

            await RunAndCheckIfPermissionControlledAsync(async () =>
              {
                  var entity = await _dynamicParameterValueManager.GetAsync(dynamicParameterValue.Id);
                  CheckEquality(entity, dynamicParameterValue);
              });
        }

        [Fact]
        public async Task Should_Add_Value_Async()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterValue = new DynamicParameterValue()
            {
                DynamicParameterId = dynamicParameter.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            await RunAndCheckIfPermissionControlledAsync(async () =>
             {
                 await _dynamicParameterValueManager.AddAsync(dynamicParameterValue);
             });

            await WithUnitOfWorkAsync(async () =>
             {
                 var entity = await _dynamicParameterValueManager.GetAsync(dynamicParameterValue.Id);
                 CheckEquality(entity, dynamicParameterValue);
             });
        }

        [Fact]
        public async Task Should_Update_Value_Async()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterValue = new DynamicParameterValue()
            {
                DynamicParameterId = dynamicParameter.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () =>
             {
                 await _dynamicParameterValueManager.AddAsync(dynamicParameterValue);
             });

            await WithUnitOfWorkAsync(async () =>
             {
                 dynamicParameterValue = await _dynamicParameterValueManager.GetAsync(dynamicParameterValue.Id);
                 dynamicParameterValue.ShouldNotBeNull();
             });

            dynamicParameterValue.Value = "Test2";

            await RunAndCheckIfPermissionControlledAsync(async () =>
             {
                 await _dynamicParameterValueManager.UpdateAsync(dynamicParameterValue);
             });

            await WithUnitOfWorkAsync(async () =>
             {
                 var entity = await _dynamicParameterValueManager.GetAsync(dynamicParameterValue.Id);
                 entity.Value.ShouldBe("Test2");
                 entity.DynamicParameterId.ShouldBe(dynamicParameter.Id);
             });
        }

        [Fact]
        public async Task Should_Delete_Value_Async()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterValue = new DynamicParameterValue()
            {
                DynamicParameterId = dynamicParameter.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _dynamicParameterValueManager.AddAsync(dynamicParameterValue);
            });

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                await _dynamicParameterValueManager.DeleteAsync(dynamicParameterValue.Id);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                try
                {
                    var entity = await _dynamicParameterValueManager.GetAsync(dynamicParameterValue.Id);
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
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterValue = new DynamicParameterValue()
            {
                DynamicParameterId = dynamicParameter.Id,
                Value = "Test",
                TenantId = AbpSession.TenantId
            };

            var dynamicParameterValue2 = new DynamicParameterValue()
            {
                DynamicParameterId = dynamicParameter.Id,
                Value = "Test2",
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _dynamicParameterValueManager.AddAsync(dynamicParameterValue);
                await _dynamicParameterValueManager.AddAsync(dynamicParameterValue2);
            });

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                await _dynamicParameterValueManager.CleanValuesAsync(dynamicParameter.Id);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var entity = await _dynamicParameterValueManager.GetAllValuesOfDynamicParameterAsync(dynamicParameter.Id);
                entity.ShouldBeEmpty();
            });
        }
    }
}
