using System;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.DynamicEntityProperties;
using Abp.Runtime.Caching;
using Abp.Zero.SampleApp.EntityHistory;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityProperties
{
    public class DynamicEntityPropertyManager_Tests : DynamicEntityPropertiesTestBase
    {
        private (ICache dynamicEntityPropertyManagerCache, DynamicEntityPropertyManager dynamicEntityPropertyManager)
            InitializeDynamicEntityPropertyManagerWithCacheSubstitute()
        {
            var cacheManager = Substitute.For<ICacheManager>();
            var cacheSubstitute = Substitute.For<ICache>();

            //------------- substitute will call factory method when cache requested
            //example usage: DynamicEntityPropertyCache.Get(id, () => DynamicEntityPropertyStore.Get(id)); 
            cacheSubstitute
                .Get(Arg.Any<string>(), Arg.Any<Func<string, object>>())
                .Returns((callInfo) => callInfo.ArgAt<Func<string, object>>(1).Invoke(callInfo.ArgAt<string>(0)));

            cacheSubstitute //await DynamicEntityPropertyCache.GetAsync(id, () => DynamicEntityPropertyStore.GetAsync(id));
                .GetAsync(Arg.Any<string>(), Arg.Any<Func<string, Task<object>>>())
                .Returns((callInfo) => callInfo.ArgAt<Func<string, Task<object>>>(1).Invoke(callInfo.ArgAt<string>(0)));
            //-------------

            cacheManager.GetCache(Arg.Any<string>()).Returns(cacheSubstitute);

            var dynamicEntityPropertyManager = new DynamicEntityPropertyManager(
                Resolve<IDynamicPropertyPermissionChecker>(),
                cacheManager,
                Resolve<IUnitOfWorkManager>(),
                Resolve<IDynamicEntityPropertyDefinitionManager>()
            )
            {
                DynamicEntityPropertyStore = Resolve<IDynamicEntityPropertyStore>()
            };

            return (cacheSubstitute, dynamicEntityPropertyManager);
        }

        private void CheckEquality(DynamicEntityProperty edp1, DynamicEntityProperty edp2)
        {
            edp1.ShouldNotBeNull();
            edp2.ShouldNotBeNull();
            edp1.DynamicPropertyId.ShouldBe(edp2.DynamicPropertyId);
            edp1.EntityFullName.ShouldBe(edp2.EntityFullName);
            edp1.TenantId.ShouldBe(edp2.TenantId);
        }

        [Fact]
        public void Should_Get_From_Cache()
        {
            var dynamicEntityPropertyStoreSubstitute = RegisterFake<IDynamicEntityPropertyStore>();
            var (dynamicEntityPropertyManagerCache, dynamicEntityPropertyManager) =
                InitializeDynamicEntityPropertyManagerWithCacheSubstitute();

            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();
            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = TestEntityFullName
            };

            var cacheKey = dynamicEntityProperty.Id + "@" + (dynamicEntityProperty.TenantId ?? 0);

            dynamicEntityPropertyManagerCache
                .Get(cacheKey, Arg.Any<Func<string, object>>())
                .Returns(dynamicEntityProperty);

            var entity = dynamicEntityPropertyManager.Get(dynamicEntityProperty.Id);
            CheckEquality(entity, dynamicEntityProperty);

            dynamicEntityPropertyManagerCache.Received().Get(cacheKey, Arg.Any<Func<string, object>>());
            dynamicEntityPropertyStoreSubstitute.DidNotReceive().Get(dynamicEntityProperty.Id);
        }

        [Fact]
        public void Should_Not_Get_From_Cache_For_Different_Tenant()
        {
            var dynamicEntityPropertyStoreSubstitute = RegisterFake<IDynamicEntityPropertyStore>();
            var (dynamicEntityPropertyManagerCache, dynamicEntityPropertyManager) =
                InitializeDynamicEntityPropertyManagerWithCacheSubstitute();

            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();
            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = TestEntityFullName,
                TenantId = 2
            };

            var cacheKey = dynamicEntityProperty.Id + "@" + (dynamicEntityProperty.TenantId ?? 0);

            dynamicEntityPropertyManagerCache
                .Get(cacheKey, Arg.Any<Func<string, object>>())
                .Returns(dynamicEntityProperty);

            Assert.Throws<NullReferenceException>(
                () => dynamicEntityPropertyManager.Get(dynamicEntityProperty.Id)
            );
        }

        [Fact]
        public void Should_Get_From_Db()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                Id = -1,
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            var dynamicEntityPropertyStoreSubstitute = RegisterFake<IDynamicEntityPropertyStore>();
            dynamicEntityPropertyStoreSubstitute.Get(dynamicEntityProperty.Id).Returns(dynamicEntityProperty);

            var dynamicEntityPropertyManager = Resolve<IDynamicEntityPropertyManager>();

            var entity = dynamicEntityPropertyManager.Get(dynamicEntityProperty.Id);
            CheckEquality(entity, dynamicEntityProperty);

            dynamicEntityPropertyStoreSubstitute.Received().Get(dynamicEntityProperty.Id);
        }

        [Fact]
        public void Should_Add_Property()
        {
            var (dynamicEntityPropertyManagerCache, dynamicEntityPropertyManager) =
                InitializeDynamicEntityPropertyManagerWithCacheSubstitute();

            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();
            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            RunAndCheckIfPermissionControlled(() => { dynamicEntityPropertyManager.Add(dynamicEntityProperty); });

            var cacheKey = dynamicEntityProperty.Id + "@" + (dynamicEntityProperty.TenantId ?? 0);
            dynamicEntityPropertyManagerCache.Received().Set(cacheKey, dynamicEntityProperty);

            WithUnitOfWork(() =>
            {
                var val = dynamicEntityPropertyManager.Get(dynamicEntityProperty.Id);

                val.ShouldNotBeNull();

                val.DynamicPropertyId.ShouldBe(dynamicEntityProperty.DynamicPropertyId);
                val.EntityFullName.ShouldBe(dynamicEntityProperty.EntityFullName);
            });
        }

        [Fact]
        public void Should_Not_Add_Property_If_Entity_Not_Registered()
        {
            var (dynamicEntityPropertyManagerCache, dynamicEntityPropertyManager) =
                InitializeDynamicEntityPropertyManagerWithCacheSubstitute();

            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();
            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = typeof(Post).FullName,
                TenantId = AbpSession.TenantId
            };

            try
            {
                dynamicEntityPropertyManager.Add(dynamicEntityProperty);
                throw new Exception("Should check if entity registered");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain(typeof(Post).FullName);
            }
        }

        [Fact]
        public void Should_Update_Property()
        {
            var (dynamicEntityPropertyManagerCache, dynamicEntityPropertyManager) =
                InitializeDynamicEntityPropertyManagerWithCacheSubstitute();

            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();
            var dynamicProperty2 = CreateAndGetDynamicPropertyWithTestPermission();
            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() => { dynamicEntityPropertyManager.Add(dynamicEntityProperty); });

            var cacheKey = dynamicEntityProperty.Id + "@" + (dynamicEntityProperty.TenantId ?? 0);

            dynamicEntityPropertyManagerCache.Received().Set(cacheKey, dynamicEntityProperty);
            dynamicEntityPropertyManagerCache.ClearReceivedCalls();

            WithUnitOfWork(() =>
            {
                dynamicEntityProperty = dynamicEntityPropertyManager.Get(dynamicEntityProperty.Id);

                dynamicEntityProperty.ShouldNotBeNull();
                dynamicEntityProperty.DynamicPropertyId.ShouldBe(dynamicEntityProperty.DynamicPropertyId);
                dynamicEntityProperty.EntityFullName.ShouldBe(dynamicEntityProperty.EntityFullName);
            });

            dynamicEntityProperty.DynamicPropertyId = dynamicProperty2.Id;

            dynamicEntityPropertyManagerCache.ClearReceivedCalls();
            RunAndCheckIfPermissionControlled(() => { dynamicEntityPropertyManager.Update(dynamicEntityProperty); });
            dynamicEntityPropertyManagerCache.Received().Set(cacheKey, dynamicEntityProperty);

            WithUnitOfWork(() =>
            {
                var val = dynamicEntityPropertyManager.Get(dynamicEntityProperty.Id);

                val.ShouldNotBeNull();
                val.DynamicPropertyId.ShouldBe(dynamicProperty2.Id);
            });
        }

        [Fact]
        public void Should_Not_Update_Property_If_Entity_Not_Registered()
        {
            var (dynamicEntityPropertyManagerCache, dynamicEntityPropertyManager) =
                InitializeDynamicEntityPropertyManagerWithCacheSubstitute();


            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();
            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() => { dynamicEntityPropertyManager.Add(dynamicEntityProperty); });

            var cacheKey = dynamicEntityProperty.Id + "@" + (dynamicEntityProperty.TenantId ?? 0);

            dynamicEntityPropertyManagerCache.Received().Set(cacheKey, dynamicEntityProperty);
            dynamicEntityPropertyManagerCache.ClearReceivedCalls();

            WithUnitOfWork(() =>
            {
                dynamicEntityProperty = dynamicEntityPropertyManager.Get(dynamicEntityProperty.Id);

                dynamicEntityProperty.ShouldNotBeNull();
                dynamicEntityProperty.DynamicPropertyId.ShouldBe(dynamicEntityProperty.DynamicPropertyId);
                dynamicEntityProperty.EntityFullName.ShouldBe(dynamicEntityProperty.EntityFullName);
            });

            dynamicEntityProperty.EntityFullName = typeof(Post).FullName;
            try
            {
                dynamicEntityPropertyManager.Update(dynamicEntityProperty);
                throw new Exception("Should check if entity registered");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain(typeof(Post).FullName);
            }
        }

        [Fact]
        public void Should_Delete_Property()
        {
            var (dynamicEntityPropertyManagerCache, dynamicEntityPropertyManager) =
                InitializeDynamicEntityPropertyManagerWithCacheSubstitute();
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() => { dynamicEntityPropertyManager.Add(dynamicEntityProperty); });

            var cacheKey = dynamicEntityProperty.Id + "@" + (dynamicEntityProperty.TenantId ?? 0);
            dynamicEntityPropertyManagerCache.Received().Set(cacheKey, dynamicEntityProperty);

            WithUnitOfWork(() =>
            {
                dynamicEntityProperty = dynamicEntityPropertyManager.Get(dynamicEntityProperty.Id);

                dynamicEntityProperty.ShouldNotBeNull();
                dynamicEntityProperty.DynamicPropertyId.ShouldBe(dynamicEntityProperty.DynamicPropertyId);
                dynamicEntityProperty.EntityFullName.ShouldBe(dynamicEntityProperty.EntityFullName);
            });

            dynamicEntityPropertyManagerCache.ClearReceivedCalls();
            RunAndCheckIfPermissionControlled(() => { dynamicEntityPropertyManager.Delete(dynamicEntityProperty.Id); });
            dynamicEntityPropertyManagerCache.Received().Remove(cacheKey);

            WithUnitOfWork(() =>
            {
                try
                {
                    var val = dynamicEntityPropertyManager.Get(dynamicEntityProperty.Id);
                    val.ShouldBeNull();
                }
                catch (EntityNotFoundException)
                {
                }
            });
        }


        [Fact]
        public async Task Should_Add_Property_Async()
        {
            var (dynamicEntityPropertyManagerCache, dynamicEntityPropertyManager) =
                InitializeDynamicEntityPropertyManagerWithCacheSubstitute();
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                await dynamicEntityPropertyManager.AddAsync(dynamicEntityProperty);
            });

            var cacheKey = dynamicEntityProperty.Id + "@" + (dynamicEntityProperty.TenantId ?? 0);
            await dynamicEntityPropertyManagerCache.Received().SetAsync(cacheKey, dynamicEntityProperty);

            WithUnitOfWork(() =>
            {
                var val = dynamicEntityPropertyManager.Get(dynamicEntityProperty.Id);

                val.ShouldNotBeNull();

                val.DynamicPropertyId.ShouldBe(dynamicEntityProperty.DynamicPropertyId);
                val.EntityFullName.ShouldBe(dynamicEntityProperty.EntityFullName);
            });
        }

        [Fact]
        public async Task Should_Not_Add_Property_If_Entity_Not_Registered_Async()
        {
            var (dynamicEntityPropertyManagerCache, dynamicEntityPropertyManager) =
                InitializeDynamicEntityPropertyManagerWithCacheSubstitute();

            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();
            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = typeof(Post).FullName,
                TenantId = AbpSession.TenantId
            };

            try
            {
                await dynamicEntityPropertyManager.AddAsync(dynamicEntityProperty);
                throw new Exception("Should check if entity registered");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain(typeof(Post).FullName);
            }
        }

        [Fact]
        public async Task Should_Update_Property_Async()
        {
            var (dynamicEntityPropertyManagerCache, dynamicEntityPropertyManager) =
                InitializeDynamicEntityPropertyManagerWithCacheSubstitute();
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();
            var dynamicProperty2 = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await dynamicEntityPropertyManager.AddAsync(dynamicEntityProperty);
            });

            var cacheKey = dynamicEntityProperty.Id + "@" + (dynamicEntityProperty.TenantId ?? 0);

            await dynamicEntityPropertyManagerCache.Received().SetAsync(cacheKey, dynamicEntityProperty);

            await WithUnitOfWorkAsync(async () =>
            {
                dynamicEntityProperty = await dynamicEntityPropertyManager.GetAsync(dynamicEntityProperty.Id);

                dynamicEntityProperty.ShouldNotBeNull();
                dynamicEntityProperty.DynamicPropertyId.ShouldBe(dynamicEntityProperty.DynamicPropertyId);
                dynamicEntityProperty.EntityFullName.ShouldBe(dynamicEntityProperty.EntityFullName);
            });

            dynamicEntityProperty.DynamicPropertyId = dynamicProperty2.Id;

            dynamicEntityPropertyManagerCache.ClearReceivedCalls();
            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                await dynamicEntityPropertyManager.UpdateAsync(dynamicEntityProperty);
            });
            await dynamicEntityPropertyManagerCache.Received().SetAsync(cacheKey, dynamicEntityProperty);

            await WithUnitOfWorkAsync(async () =>
            {
                var val = await dynamicEntityPropertyManager.GetAsync(dynamicEntityProperty.Id);

                val.ShouldNotBeNull();
                val.DynamicPropertyId.ShouldBe(dynamicProperty2.Id);
            });
        }

        [Fact]
        public void Should_Not_Update_Property_If_Entity_Not_Registered_Async()
        {
            var (dynamicEntityPropertyManagerCache, dynamicEntityPropertyManager) =
                InitializeDynamicEntityPropertyManagerWithCacheSubstitute();


            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();
            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() => { dynamicEntityPropertyManager.Add(dynamicEntityProperty); });

            var cacheKey = dynamicEntityProperty.Id + "@" + (dynamicEntityProperty.TenantId ?? 0);

            dynamicEntityPropertyManagerCache.Received().Set(cacheKey, dynamicEntityProperty);
            dynamicEntityPropertyManagerCache.ClearReceivedCalls();

            WithUnitOfWork(() =>
            {
                dynamicEntityProperty = dynamicEntityPropertyManager.Get(dynamicEntityProperty.Id);

                dynamicEntityProperty.ShouldNotBeNull();
                dynamicEntityProperty.DynamicPropertyId.ShouldBe(dynamicEntityProperty.DynamicPropertyId);
                dynamicEntityProperty.EntityFullName.ShouldBe(dynamicEntityProperty.EntityFullName);
            });

            dynamicEntityProperty.EntityFullName = typeof(Post).FullName;
            try
            {
                dynamicEntityPropertyManager.Update(dynamicEntityProperty);
                throw new Exception("Should check if entity registered");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain(typeof(Post).FullName);
            }
        }

        [Fact]
        public async Task Should_Delete_Property_Async()
        {
            var (dynamicEntityPropertyManagerCache, dynamicEntityPropertyManager) =
                InitializeDynamicEntityPropertyManagerWithCacheSubstitute();
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicEntityProperty = new DynamicEntityProperty()
            {
                DynamicPropertyId = dynamicProperty.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await dynamicEntityPropertyManager.AddAsync(dynamicEntityProperty);
            });

            var cacheKey = dynamicEntityProperty.Id + "@" + (dynamicEntityProperty.TenantId ?? 0);

            await dynamicEntityPropertyManagerCache.Received().SetAsync(cacheKey, dynamicEntityProperty);

            await WithUnitOfWorkAsync(async () =>
            {
                var val = await dynamicEntityPropertyManager.GetAsync(dynamicEntityProperty.Id);

                val.ShouldNotBeNull();
                val.DynamicPropertyId.ShouldBe(dynamicEntityProperty.DynamicPropertyId);
                val.EntityFullName.ShouldBe(dynamicEntityProperty.EntityFullName);
            });

            dynamicEntityPropertyManagerCache.ClearReceivedCalls();
            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                await dynamicEntityPropertyManager.DeleteAsync(dynamicEntityProperty.Id);
            });
            await dynamicEntityPropertyManagerCache.Received().RemoveAsync(cacheKey);

            await WithUnitOfWorkAsync(async () =>
            {
                try
                {
                    var val = await dynamicEntityPropertyManager.GetAsync(dynamicEntityProperty.Id);
                    val.ShouldBeNull();
                }
                catch (EntityNotFoundException)
                {
                }
            });
        }
    }
}
