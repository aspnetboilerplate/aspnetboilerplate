using System;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.DynamicEntityParameters;
using Abp.Runtime.Caching;
using Abp.Zero.SampleApp.EntityHistory;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityParameters
{
    public class EntityDynamicParameterManager_Tests : DynamicEntityParametersTestBase
    {
        private (ICache entityDynamicParameterManagerCache, EntityDynamicParameterManager entityDynamicParameterManager) InitializeEntityDynamicParameterManagerWithCacheSubstitute()
        {
            var cacheManager = Substitute.For<ICacheManager>();
            var cacheSubstitute = Substitute.For<ICache>();

            //------------- substitute will call factory method when cache requested
            //example usage: EntityDynamicParameterCache.Get(id, () => EntityDynamicParameterStore.Get(id)); 
            cacheSubstitute
                .Get(Arg.Any<string>(), Arg.Any<Func<string, object>>())
                .Returns((callInfo) => callInfo.ArgAt<Func<string, object>>(1).Invoke(callInfo.ArgAt<string>(0)));

            cacheSubstitute//await EntityDynamicParameterCache.GetAsync(id, () => EntityDynamicParameterStore.GetAsync(id));
                .GetAsync(Arg.Any<string>(), Arg.Any<Func<string, Task<object>>>())
                .Returns((callInfo) => callInfo.ArgAt<Func<string, Task<object>>>(1).Invoke(callInfo.ArgAt<string>(0)));
            //-------------

            cacheManager.GetCache(Arg.Any<string>()).Returns(cacheSubstitute);

            var entityDynamicParameterManager = new EntityDynamicParameterManager(
                    Resolve<IDynamicParameterPermissionChecker>(),
                    cacheManager,
                    Resolve<IUnitOfWorkManager>(),
                    Resolve<IDynamicEntityParameterDefinitionManager>()
                )
            {
                EntityDynamicParameterStore = Resolve<IEntityDynamicParameterStore>()
            };

            return (cacheSubstitute, entityDynamicParameterManager);
        }

        private void CheckEquality(EntityDynamicParameter edp1, EntityDynamicParameter edp2)
        {
            edp1.ShouldNotBeNull();
            edp2.ShouldNotBeNull();
            edp1.DynamicParameterId.ShouldBe(edp2.DynamicParameterId);
            edp1.EntityFullName.ShouldBe(edp2.EntityFullName);
            edp1.TenantId.ShouldBe(edp2.TenantId);
        }

        [Fact]
        public void Should_Get_From_Cache()
        {
            var entityDynamicParameterStoreSubstitute = RegisterFake<IEntityDynamicParameterStore>();
            var (entityDynamicParameterManagerCache, entityDynamicParameterManager) = InitializeEntityDynamicParameterManagerWithCacheSubstitute();

            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();
            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            entityDynamicParameterManagerCache
                .Get(entityDynamicParameter.Id.ToString(), Arg.Any<Func<string, object>>())
                .Returns(entityDynamicParameter);

            var entity = entityDynamicParameterManager.Get(entityDynamicParameter.Id);
            CheckEquality(entity, entityDynamicParameter);

            entityDynamicParameterManagerCache.Received().Get(entityDynamicParameter.Id.ToString(), Arg.Any<Func<string, object>>());
            entityDynamicParameterStoreSubstitute.DidNotReceive().Get(entityDynamicParameter.Id);
        }

        [Fact]
        public void Should_Get_From_Db()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                Id = -1,
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            var entityDynamicParameterStoreSubstitute = RegisterFake<IEntityDynamicParameterStore>();
            entityDynamicParameterStoreSubstitute.Get(entityDynamicParameter.Id).Returns(entityDynamicParameter);

            var entityDynamicParameterManager = Resolve<IEntityDynamicParameterManager>();

            var entity = entityDynamicParameterManager.Get(entityDynamicParameter.Id);
            CheckEquality(entity, entityDynamicParameter);

            entityDynamicParameterStoreSubstitute.Received().Get(entityDynamicParameter.Id);
        }

        [Fact]
        public void Should_Add_Parameter()
        {
            var (entityDynamicParameterManagerCache, entityDynamicParameterManager) = InitializeEntityDynamicParameterManagerWithCacheSubstitute();

            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();
            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            RunAndCheckIfPermissionControlled(() =>
            {
                entityDynamicParameterManager.Add(entityDynamicParameter);
            });

            entityDynamicParameterManagerCache.Received().Set(entityDynamicParameter.Id.ToString(), entityDynamicParameter);

            WithUnitOfWork(() =>
            {
                var val = entityDynamicParameterManager.Get(entityDynamicParameter.Id);

                val.ShouldNotBeNull();

                val.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                val.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });
        }

        [Fact]
        public void Should_Not_Add_Parameter_If_Entity_Not_Registered()
        {
            var (entityDynamicParameterManagerCache, entityDynamicParameterManager) = InitializeEntityDynamicParameterManagerWithCacheSubstitute();

            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();
            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = typeof(Post).FullName,
                TenantId = AbpSession.TenantId
            };

            try
            {
                entityDynamicParameterManager.Add(entityDynamicParameter);
                throw new Exception("Should check if entity registered");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain(typeof(Post).FullName);
            }
        }

        [Fact]
        public void Should_Update_Parameter()
        {
            var (entityDynamicParameterManagerCache, entityDynamicParameterManager) = InitializeEntityDynamicParameterManagerWithCacheSubstitute();
            
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();
            var dynamicParameter2 = CreateAndGetDynamicParameterWithTestPermission();
            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                entityDynamicParameterManager.Add(entityDynamicParameter);
            });

            entityDynamicParameterManagerCache.Received().Set(entityDynamicParameter.Id.ToString(), entityDynamicParameter);
            entityDynamicParameterManagerCache.ClearReceivedCalls();

            WithUnitOfWork(() =>
            {
                entityDynamicParameter = entityDynamicParameterManager.Get(entityDynamicParameter.Id);

                entityDynamicParameter.ShouldNotBeNull();
                entityDynamicParameter.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                entityDynamicParameter.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });

            entityDynamicParameter.DynamicParameterId = dynamicParameter2.Id;

            entityDynamicParameterManagerCache.ClearReceivedCalls();
            RunAndCheckIfPermissionControlled(() =>
            {
                entityDynamicParameterManager.Update(entityDynamicParameter);
            });
            entityDynamicParameterManagerCache.Received().Set(entityDynamicParameter.Id.ToString(), entityDynamicParameter);

            WithUnitOfWork(() =>
            {
                var val = entityDynamicParameterManager.Get(entityDynamicParameter.Id);

                val.ShouldNotBeNull();
                val.DynamicParameterId.ShouldBe(dynamicParameter2.Id);
            });
        }

        [Fact]
        public void Should_Not_Update_Parameter_If_Entity_Not_Registered()
        {
            var (entityDynamicParameterManagerCache, entityDynamicParameterManager) = InitializeEntityDynamicParameterManagerWithCacheSubstitute();


            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();
            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                entityDynamicParameterManager.Add(entityDynamicParameter);
            });

            entityDynamicParameterManagerCache.Received().Set(entityDynamicParameter.Id.ToString(), entityDynamicParameter);
            entityDynamicParameterManagerCache.ClearReceivedCalls();

            WithUnitOfWork(() =>
            {
                entityDynamicParameter = entityDynamicParameterManager.Get(entityDynamicParameter.Id);

                entityDynamicParameter.ShouldNotBeNull();
                entityDynamicParameter.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                entityDynamicParameter.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });

            entityDynamicParameter.EntityFullName = typeof(Post).FullName;
            try
            {
                entityDynamicParameterManager.Update(entityDynamicParameter);
                throw new Exception("Should check if entity registered");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain(typeof(Post).FullName);
            }
        }

        [Fact]
        public void Should_Delete_Parameter()
        {
            var (entityDynamicParameterManagerCache, entityDynamicParameterManager) = InitializeEntityDynamicParameterManagerWithCacheSubstitute();
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                entityDynamicParameterManager.Add(entityDynamicParameter);
            });

            entityDynamicParameterManagerCache.Received().Set(entityDynamicParameter.Id.ToString(), entityDynamicParameter);

            WithUnitOfWork(() =>
            {
                entityDynamicParameter = entityDynamicParameterManager.Get(entityDynamicParameter.Id);

                entityDynamicParameter.ShouldNotBeNull();
                entityDynamicParameter.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                entityDynamicParameter.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });

            entityDynamicParameterManagerCache.ClearReceivedCalls();
            RunAndCheckIfPermissionControlled(() =>
            {
                entityDynamicParameterManager.Delete(entityDynamicParameter.Id);
            });
            entityDynamicParameterManagerCache.Received().Remove(entityDynamicParameter.Id.ToString());

            WithUnitOfWork(() =>
            {
                try
                {
                    var val = entityDynamicParameterManager.Get(entityDynamicParameter.Id);
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
            var (entityDynamicParameterManagerCache, entityDynamicParameterManager) = InitializeEntityDynamicParameterManagerWithCacheSubstitute();
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            await RunAndCheckIfPermissionControlledAsync(async () =>
             {
                 await entityDynamicParameterManager.AddAsync(entityDynamicParameter);
             });

            await entityDynamicParameterManagerCache.Received().SetAsync(entityDynamicParameter.Id.ToString(), entityDynamicParameter);

            WithUnitOfWork(() =>
            {
                var val = entityDynamicParameterManager.Get(entityDynamicParameter.Id);

                val.ShouldNotBeNull();

                val.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                val.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });
        }

        [Fact]
        public async Task Should_Not_Add_Parameter_If_Entity_Not_Registered_Async()
        {
            var (entityDynamicParameterManagerCache, entityDynamicParameterManager) = InitializeEntityDynamicParameterManagerWithCacheSubstitute();

            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();
            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = typeof(Post).FullName,
                TenantId = AbpSession.TenantId
            };

            try
            {
                await entityDynamicParameterManager.AddAsync(entityDynamicParameter);
                throw new Exception("Should check if entity registered");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain(typeof(Post).FullName);
            }
        }

        [Fact]
        public async Task Should_Update_Parameter_Async()
        {
            var (entityDynamicParameterManagerCache, entityDynamicParameterManager) = InitializeEntityDynamicParameterManagerWithCacheSubstitute();
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();
            var dynamicParameter2 = CreateAndGetDynamicParameterWithTestPermission();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await entityDynamicParameterManager.AddAsync(entityDynamicParameter);
            });

            await entityDynamicParameterManagerCache.Received().SetAsync(entityDynamicParameter.Id.ToString(), entityDynamicParameter);

            await WithUnitOfWorkAsync(async () =>
            {
                entityDynamicParameter = await entityDynamicParameterManager.GetAsync(entityDynamicParameter.Id);

                entityDynamicParameter.ShouldNotBeNull();
                entityDynamicParameter.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                entityDynamicParameter.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });

            entityDynamicParameter.DynamicParameterId = dynamicParameter2.Id;

            entityDynamicParameterManagerCache.ClearReceivedCalls();
            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                await entityDynamicParameterManager.UpdateAsync(entityDynamicParameter);
            });
            await entityDynamicParameterManagerCache.Received().SetAsync(entityDynamicParameter.Id.ToString(), entityDynamicParameter);

            await WithUnitOfWorkAsync(async () =>
            {
                var val = await entityDynamicParameterManager.GetAsync(entityDynamicParameter.Id);

                val.ShouldNotBeNull();
                val.DynamicParameterId.ShouldBe(dynamicParameter2.Id);
            });
        }

        [Fact]
        public void Should_Not_Update_Parameter_If_Entity_Not_Registered_Async()
        {
            var (entityDynamicParameterManagerCache, entityDynamicParameterManager) = InitializeEntityDynamicParameterManagerWithCacheSubstitute();


            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();
            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            WithUnitOfWork(() =>
            {
                entityDynamicParameterManager.Add(entityDynamicParameter);
            });

            entityDynamicParameterManagerCache.Received().Set(entityDynamicParameter.Id.ToString(), entityDynamicParameter);
            entityDynamicParameterManagerCache.ClearReceivedCalls();

            WithUnitOfWork(() =>
            {
                entityDynamicParameter = entityDynamicParameterManager.Get(entityDynamicParameter.Id);

                entityDynamicParameter.ShouldNotBeNull();
                entityDynamicParameter.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                entityDynamicParameter.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });

            entityDynamicParameter.EntityFullName = typeof(Post).FullName;
            try
            {
                entityDynamicParameterManager.Update(entityDynamicParameter);
                throw new Exception("Should check if entity registered");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain(typeof(Post).FullName);
            }
        }

        [Fact]
        public async Task Should_Delete_Parameter_Async()
        {
            var (entityDynamicParameterManagerCache, entityDynamicParameterManager) = InitializeEntityDynamicParameterManagerWithCacheSubstitute();
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var entityDynamicParameter = new EntityDynamicParameter()
            {
                DynamicParameterId = dynamicParameter.Id,
                EntityFullName = TestEntityFullName,
                TenantId = AbpSession.TenantId
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await entityDynamicParameterManager.AddAsync(entityDynamicParameter);
            });
            await entityDynamicParameterManagerCache.Received().SetAsync(entityDynamicParameter.Id.ToString(), entityDynamicParameter);

            await WithUnitOfWorkAsync(async () =>
            {
                var val = await entityDynamicParameterManager.GetAsync(entityDynamicParameter.Id);

                val.ShouldNotBeNull();
                val.DynamicParameterId.ShouldBe(entityDynamicParameter.DynamicParameterId);
                val.EntityFullName.ShouldBe(entityDynamicParameter.EntityFullName);
            });

            entityDynamicParameterManagerCache.ClearReceivedCalls();
            await RunAndCheckIfPermissionControlledAsync(async () =>
            {
                await entityDynamicParameterManager.DeleteAsync(entityDynamicParameter.Id);
            });
            await entityDynamicParameterManagerCache.Received().RemoveAsync(entityDynamicParameter.Id.ToString());

            await WithUnitOfWorkAsync(async () =>
            {
                try
                {
                    var val = entityDynamicParameterManager.Get(entityDynamicParameter.Id);
                    val.ShouldBeNull();
                }
                catch (EntityNotFoundException)
                {
                }
            });
        }
    }
}
