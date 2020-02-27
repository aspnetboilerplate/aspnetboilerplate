using System;
using System.Threading.Tasks;
using Abp.DynamicEntityParameters;
using Abp.Runtime.Caching;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityParameters
{
    public class DynamicParameterManager_Tests : DynamicEntityParametersTestBase
    {
        private void CheckEquality(DynamicParameter p1, DynamicParameter p2)
        {
            p1.ShouldNotBeNull();
            p2.ShouldNotBeNull();
            p1.Id.ShouldBe(p2.Id);
            p1.ParameterName.ShouldBe(p2.ParameterName);
            p1.InputType.ShouldBe(p2.InputType);
        }

        private (ICacheManager cacheManager, IDynamicParameterStore dynamicParameterStoreSubstitute, ICache cacheSubstitute) InitializeFakes()
        {
            var cacheManager = RegisterFake<ICacheManager>();
            var dynamicParameterStoreSubstitute = RegisterFake<IDynamicParameterStore>();
            var cacheSubstitute = Substitute.For<ICache>();

            cacheManager.GetCache(Arg.Any<string>()).Returns(cacheSubstitute);

            return (cacheManager, dynamicParameterStoreSubstitute, cacheSubstitute);
        }

        [Fact]
        public void Should_Get_From_Cache()
        {
            var (cacheManager, dynamicParameterStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var testDynamicParameter = new DynamicParameter
            {
                Id = -1,
                ParameterName = "Test123",
                InputType = "TestType"
            };

            cacheSubstitute
                .Get(testDynamicParameter.Id.ToString(), Arg.Any<Func<string, object>>())
                .Returns(testDynamicParameter);

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();

            var entity = dynamicParameterManager.Get(testDynamicParameter.Id);
            CheckEquality(entity, testDynamicParameter);

            cacheManager.Received().GetCache(Arg.Any<string>());
            cacheSubstitute.Received().Get(testDynamicParameter.Id.ToString(), Arg.Any<Func<string, object>>());
            dynamicParameterStoreSubstitute.DidNotReceive().Get(testDynamicParameter.Id);
        }

        [Fact]
        public void Should_Get_From_Db_If_Cache_Not_Exists()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterStoreSubstitute = RegisterFake<IDynamicParameterStore>();
            dynamicParameterStoreSubstitute.Get(dynamicParameter.Id).Returns(dynamicParameter);

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();
            var entity = dynamicParameterManager.Get(dynamicParameter.Id);
            CheckEquality(entity, dynamicParameter);

            dynamicParameterStoreSubstitute.Received().Get(dynamicParameter.Id);
        }

        [Fact]
        public void Should_Add_And_Change_Cache()
        {
            var (cacheManager, dynamicParameterStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();

            var testDynamicParameter = new DynamicParameter
            {
                ParameterName = "Test123",
                InputType = "TestType"
            };

            dynamicParameterManager.Add(testDynamicParameter);

            cacheSubstitute.Received()
                .Set(testDynamicParameter.Id.ToString(), testDynamicParameter, Arg.Any<TimeSpan?>(), Arg.Any<TimeSpan?>());
            dynamicParameterStoreSubstitute.Received().Add(testDynamicParameter);
        }

        [Fact]
        public void Should_Update_And_Change_Cache()
        {
            var testDynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var (cacheManager, dynamicParameterStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();

            testDynamicParameter.ParameterName = "Test";

            dynamicParameterManager.Update(testDynamicParameter);

            cacheSubstitute.Received()
                .Set(testDynamicParameter.Id.ToString(), testDynamicParameter, Arg.Any<TimeSpan?>(), Arg.Any<TimeSpan?>());
            dynamicParameterStoreSubstitute.Received().Update(testDynamicParameter);
        }

        [Fact]
        public void Should_Delete_And_Change_Cache()
        {
            var testDynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var (cacheManager, dynamicParameterStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();

            dynamicParameterManager.Delete(testDynamicParameter.Id);
            cacheSubstitute.Received().Remove(testDynamicParameter.Id.ToString());
            dynamicParameterStoreSubstitute.Received().Delete(testDynamicParameter.Id);
        }

        [Fact]
        public async Task Should_Get_From_Cache_Async()
        {
            var (cacheManager, dynamicParameterStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var testDynamicParameter = new DynamicParameter
            {
                Id = -1,
                ParameterName = "Test123",
                InputType = "TestType"
            };

            cacheSubstitute
                .GetAsync(testDynamicParameter.Id.ToString(), Arg.Any<Func<string, Task<object>>>())
                .Returns(testDynamicParameter);

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();

            var entity = await dynamicParameterManager.GetAsync(testDynamicParameter.Id);
            CheckEquality(entity, testDynamicParameter);

            cacheManager.Received().GetCache(Arg.Any<string>());
            await cacheSubstitute.Received().GetAsync(testDynamicParameter.Id.ToString(), Arg.Any<Func<string, Task<object>>>());
            await dynamicParameterStoreSubstitute.DidNotReceive().GetAsync(testDynamicParameter.Id);
        }

        [Fact]
        public async Task Should_Get_From_Db_If_Cache_Not_Exists_Async()
        {
            var dynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterStoreSubstitute = RegisterFake<IDynamicParameterStore>();
            dynamicParameterStoreSubstitute.GetAsync(dynamicParameter.Id).Returns(dynamicParameter);

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();
            var entity = await dynamicParameterManager.GetAsync(dynamicParameter.Id);
            CheckEquality(entity, dynamicParameter);

            await dynamicParameterStoreSubstitute.Received().GetAsync(dynamicParameter.Id);
        }

        [Fact]
        public async Task Should_Add_And_Change_Cache_Async()
        {
            var (cacheManager, dynamicParameterStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();

            var testDynamicParameter = new DynamicParameter
            {
                ParameterName = "Test123",
                InputType = "TestType"
            };

            await dynamicParameterManager.AddAsync(testDynamicParameter);
            await cacheSubstitute.Received()
                .SetAsync(testDynamicParameter.Id.ToString(), testDynamicParameter, Arg.Any<TimeSpan?>(), Arg.Any<TimeSpan?>());
            await dynamicParameterStoreSubstitute.Received().AddAsync(testDynamicParameter);
        }

        [Fact]
        public async Task Should_Update_And_Change_Cache_Async()
        {
            var testDynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var (cacheManager, dynamicParameterStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();

            testDynamicParameter.ParameterName = "Test";

            await dynamicParameterManager.UpdateAsync(testDynamicParameter);
            await cacheSubstitute.Received()
                  .SetAsync(testDynamicParameter.Id.ToString(), testDynamicParameter, Arg.Any<TimeSpan?>(), Arg.Any<TimeSpan?>());
            await dynamicParameterStoreSubstitute.Received().UpdateAsync(testDynamicParameter);
        }

        [Fact]
        public async Task Should_Delete_And_Change_Cache_Async()
        {
            var testDynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var (cacheManager, dynamicParameterStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();

            await dynamicParameterManager.DeleteAsync(testDynamicParameter.Id);
            await cacheSubstitute.Received().RemoveAsync(testDynamicParameter.Id.ToString());
            await dynamicParameterStoreSubstitute.Received().DeleteAsync(testDynamicParameter.Id);
        }
    }
}
