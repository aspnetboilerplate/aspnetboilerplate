using System;
using System.Linq;
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
                InputType = GetRandomAllowedInputType()
            };

            dynamicParameterManager.Add(testDynamicParameter);

            cacheSubstitute.Received()
                .Set(testDynamicParameter.Id.ToString(), testDynamicParameter, Arg.Any<TimeSpan?>(), Arg.Any<TimeSpan?>());
            dynamicParameterStoreSubstitute.Received().Add(testDynamicParameter);
        }

        [Fact]
        public void Should_Not_Add_If_Input_Type_Is_Not_Exists()
        {
            var testDynamicParameter = new DynamicParameter
            {
                ParameterName = "Test123",
                InputType = "asd123"
            };

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();
            try
            {
                dynamicParameterManager.Add(testDynamicParameter);
                throw new Exception("Should check if input type exists");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain("asd123");
            }
        }

        [Fact]
        public void Should_Not_Add_If_Parameter_Name_Is_Null_Or_Empty()
        {
            var testDynamicParameter = new DynamicParameter
            {
                ParameterName = string.Empty,
                InputType = Resolve<IDynamicEntityParameterDefinitionManager>().GetAllAllowedInputTypeNames().First()
            };

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();

            var exception = Should.Throw<ArgumentNullException>(() => dynamicParameterManager.Add(testDynamicParameter));
            exception.Message.ShouldContain(nameof(testDynamicParameter.ParameterName));

            var testDynamicParameter2 = new DynamicParameter
            {
                ParameterName = null,
                InputType = Resolve<IDynamicEntityParameterDefinitionManager>().GetAllAllowedInputTypeNames().First()
            };

            var exception2 = Should.Throw<ArgumentNullException>(() => dynamicParameterManager.Add(testDynamicParameter2));
            exception2.Message.ShouldContain(nameof(testDynamicParameter.ParameterName));
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
        public void Should_Not_Update_If_Input_Type_Is_Not_Exists()
        {
            var testDynamicParameter = CreateAndGetDynamicParameterWithTestPermission();
            testDynamicParameter.InputType = "asd123";

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();
            try
            {
                dynamicParameterManager.Update(testDynamicParameter);
                throw new Exception("Should check if input type exists");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain("asd123");
            }
        }

        [Fact]
        public void Should_Not_Update_If_Parameter_Name_Is_Null_Or_Empty()
        {
            var testDynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();

            testDynamicParameter.ParameterName = string.Empty;

            var exception = Should.Throw<ArgumentNullException>(() => dynamicParameterManager.Update(testDynamicParameter));
            exception.Message.ShouldContain(nameof(testDynamicParameter.ParameterName));

            testDynamicParameter.ParameterName = null;

            var exception2 = Should.Throw<ArgumentNullException>(() => dynamicParameterManager.Update(testDynamicParameter));
            exception2.Message.ShouldContain(nameof(testDynamicParameter.ParameterName));
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
                InputType = GetRandomAllowedInputType()
            };

            await dynamicParameterManager.AddAsync(testDynamicParameter);
            await cacheSubstitute.Received()
                .SetAsync(testDynamicParameter.Id.ToString(), testDynamicParameter, Arg.Any<TimeSpan?>(), Arg.Any<TimeSpan?>());
            await dynamicParameterStoreSubstitute.Received().AddAsync(testDynamicParameter);
        }

        [Fact]
        public async Task Should_Not_Add_If_Input_Type_Is_Not_Exists_Async()
        {
            var testDynamicParameter = new DynamicParameter
            {
                ParameterName = "Test123",
                InputType = "asd123"
            };

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();
            try
            {
                await dynamicParameterManager.AddAsync(testDynamicParameter);
                throw new Exception("Should check if input type exists");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain("asd123");
            }
        }

        [Fact]
        public async Task Should_Not_Add_If_Parameter_Name_Is_Null_Or_Empty_Async()
        {
            var testDynamicParameter = new DynamicParameter
            {
                ParameterName = string.Empty,
                InputType = Resolve<IDynamicEntityParameterDefinitionManager>().GetAllAllowedInputTypeNames().First()
            };

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();

            var exception = await Should.ThrowAsync<ArgumentNullException>(() => dynamicParameterManager.AddAsync(testDynamicParameter));
            exception.Message.ShouldContain(nameof(testDynamicParameter.ParameterName));

            var testDynamicParameter2 = new DynamicParameter
            {
                ParameterName = null,
                InputType = Resolve<IDynamicEntityParameterDefinitionManager>().GetAllAllowedInputTypeNames().First()
            };

            var exception2 = await Should.ThrowAsync<ArgumentNullException>(() => dynamicParameterManager.AddAsync(testDynamicParameter2));
            exception2.Message.ShouldContain(nameof(testDynamicParameter.ParameterName));
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
        public async Task Should_Not_Update_If_Input_Type_Is_Not_Exists_Async()
        {
            var testDynamicParameter = CreateAndGetDynamicParameterWithTestPermission();
            testDynamicParameter.InputType = "asd123";
            var dynamicParameterManager = Resolve<IDynamicParameterManager>();
            try
            {
                await dynamicParameterManager.UpdateAsync(testDynamicParameter);
                throw new Exception("Should check if input type exists");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain("asd123");
            }
        }

        [Fact]
        public async Task Should_Not_Update_If_Parameter_Name_Is_Null_Or_Empty_Async()
        {
            var testDynamicParameter = CreateAndGetDynamicParameterWithTestPermission();

            var dynamicParameterManager = Resolve<IDynamicParameterManager>();

            testDynamicParameter.ParameterName = string.Empty;

            var exception = await Should.ThrowAsync<ArgumentNullException>(() => dynamicParameterManager.UpdateAsync(testDynamicParameter));
            exception.Message.ShouldContain(nameof(testDynamicParameter.ParameterName));

            testDynamicParameter.ParameterName = null;

            var exception2 = await Should.ThrowAsync<ArgumentNullException>(() => dynamicParameterManager.UpdateAsync(testDynamicParameter));
            exception2.Message.ShouldContain(nameof(testDynamicParameter.ParameterName));
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
