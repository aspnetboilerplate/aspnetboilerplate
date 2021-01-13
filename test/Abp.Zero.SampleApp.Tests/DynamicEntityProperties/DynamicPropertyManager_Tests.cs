using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.DynamicEntityProperties;
using Abp.Runtime.Caching;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityProperties
{
    public class DynamicPropertyManager_Tests : DynamicEntityPropertiesTestBase
    {
        private void CheckEquality(DynamicProperty p1, DynamicProperty p2)
        {
            p1.ShouldNotBeNull();
            p2.ShouldNotBeNull();
            p1.Id.ShouldBe(p2.Id);
            p1.PropertyName.ShouldBe(p2.PropertyName);
            p1.InputType.ShouldBe(p2.InputType);
        }

        private (ICacheManager cacheManager, IDynamicPropertyStore dynamicPropertyStoreSubstitute, ICache
            cacheSubstitute) InitializeFakes()
        {
            var cacheManager = RegisterFake<ICacheManager>();
            var dynamicPropertyStoreSubstitute = RegisterFake<IDynamicPropertyStore>();
            var cacheSubstitute = Substitute.For<ICache>();

            cacheManager.GetCache(Arg.Any<string>()).Returns(cacheSubstitute);

            return (cacheManager, dynamicPropertyStoreSubstitute, cacheSubstitute);
        }

        [Fact]
        public void Should_Get_From_Cache()
        {
            var (cacheManager, dynamicPropertyStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var testDynamicProperty = new DynamicProperty
            {
                Id = -1,
                PropertyName = "Test123",
                InputType = "TestType"
            };

            cacheSubstitute
                .Get(testDynamicProperty.Id.ToString(), Arg.Any<Func<string, object>>())
                .Returns(testDynamicProperty);

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();

            var entity = dynamicPropertyManager.Get(testDynamicProperty.Id);
            CheckEquality(entity, testDynamicProperty);

            cacheManager.Received().GetCache(Arg.Any<string>());
            cacheSubstitute.Received().Get(testDynamicProperty.Id.ToString(), Arg.Any<Func<string, object>>());
            dynamicPropertyStoreSubstitute.DidNotReceive().Get(testDynamicProperty.Id);
        }

        [Fact]
        public void Should_Get_From_Db_If_Cache_Not_Exists()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyStoreSubstitute = RegisterFake<IDynamicPropertyStore>();
            dynamicPropertyStoreSubstitute.Get(dynamicProperty.Id).Returns(dynamicProperty);

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();
            var entity = dynamicPropertyManager.Get(dynamicProperty.Id);
            CheckEquality(entity, dynamicProperty);

            dynamicPropertyStoreSubstitute.Received().Get(dynamicProperty.Id);
        }

        [Fact]
        public void Should_Add_And_Change_Cache()
        {
            var (cacheManager, dynamicPropertyStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();

            var testDynamicProperty = new DynamicProperty
            {
                PropertyName = "Test123",
                InputType = GetRandomAllowedInputType()
            };

            var result = dynamicPropertyManager.Add(testDynamicProperty);

            cacheSubstitute.Received().Set(
                testDynamicProperty.Id.ToString(),
                testDynamicProperty,
                Arg.Any<TimeSpan?>(),
                Arg.Any<DateTimeOffset?>()
            );

            dynamicPropertyStoreSubstitute.Received().Add(testDynamicProperty);
            result.ShouldBe(testDynamicProperty);
        }

        [Fact]
        public void Should_Not_Add_If_Input_Type_Is_Not_Exists()
        {
            var testDynamicProperty = new DynamicProperty
            {
                PropertyName = "Test123",
                InputType = "asd123"
            };

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();
            try
            {
                dynamicPropertyManager.Add(testDynamicProperty);
                throw new Exception("Should check if input type exists");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain("asd123");
            }
        }

        [Fact]
        public void Should_Not_Add_If_Property_Name_Is_Null_Or_Empty()
        {
            var testDynamicProperty = new DynamicProperty
            {
                PropertyName = string.Empty,
                InputType = Resolve<IDynamicEntityPropertyDefinitionManager>().GetAllAllowedInputTypeNames().First()
            };

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();

            var exception = Should.Throw<ArgumentNullException>(() => dynamicPropertyManager.Add(testDynamicProperty));
            exception.Message.ShouldContain(nameof(testDynamicProperty.PropertyName));

            var testDynamicProperty2 = new DynamicProperty
            {
                PropertyName = null,
                InputType = Resolve<IDynamicEntityPropertyDefinitionManager>().GetAllAllowedInputTypeNames().First()
            };

            var exception2 =
                Should.Throw<ArgumentNullException>(() => dynamicPropertyManager.Add(testDynamicProperty2));
            exception2.Message.ShouldContain(nameof(testDynamicProperty.PropertyName));
        }

        [Fact]
        public void Should_Update_And_Change_Cache()
        {
            var testDynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var (cacheManager, dynamicPropertyStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();

            testDynamicProperty.PropertyName = "Test";

            var result = dynamicPropertyManager.Update(testDynamicProperty);

            cacheSubstitute.Received().Set(
                testDynamicProperty.Id.ToString(),
                testDynamicProperty,
                Arg.Any<TimeSpan?>(),
                Arg.Any<DateTimeOffset?>()
            );

            dynamicPropertyStoreSubstitute.Received().Update(testDynamicProperty);
            result.ShouldBe(testDynamicProperty);
        }

        [Fact]
        public void Should_Not_Update_If_Input_Type_Is_Not_Exists()
        {
            var testDynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();
            testDynamicProperty.InputType = "asd123";

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();
            try
            {
                dynamicPropertyManager.Update(testDynamicProperty);
                throw new Exception("Should check if input type exists");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain("asd123");
            }
        }

        [Fact]
        public void Should_Not_Update_If_Property_Name_Is_Null_Or_Empty()
        {
            var testDynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();

            testDynamicProperty.PropertyName = string.Empty;

            var exception =
                Should.Throw<ArgumentNullException>(() => dynamicPropertyManager.Update(testDynamicProperty));
            exception.Message.ShouldContain(nameof(testDynamicProperty.PropertyName));

            testDynamicProperty.PropertyName = null;

            var exception2 =
                Should.Throw<ArgumentNullException>(() => dynamicPropertyManager.Update(testDynamicProperty));
            exception2.Message.ShouldContain(nameof(testDynamicProperty.PropertyName));
        }

        [Fact]
        public void Should_Delete_And_Change_Cache()
        {
            var testDynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var (cacheManager, dynamicPropertyStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();

            dynamicPropertyManager.Delete(testDynamicProperty.Id);
            cacheSubstitute.Received().Remove(testDynamicProperty.Id.ToString());
            dynamicPropertyStoreSubstitute.Received().Delete(testDynamicProperty.Id);
        }

        [Fact]
        public async Task Should_Get_From_Cache_Async()
        {
            var (cacheManager, dynamicPropertyStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var testDynamicProperty = new DynamicProperty
            {
                Id = -1,
                PropertyName = "Test123",
                InputType = "TestType"
            };

            cacheSubstitute
                .GetAsync(testDynamicProperty.Id.ToString(), Arg.Any<Func<string, Task<object>>>())
                .Returns(testDynamicProperty);

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();

            var entity = await dynamicPropertyManager.GetAsync(testDynamicProperty.Id);
            CheckEquality(entity, testDynamicProperty);

            cacheManager.Received().GetCache(Arg.Any<string>());
            await cacheSubstitute.Received()
                .GetAsync(testDynamicProperty.Id.ToString(), Arg.Any<Func<string, Task<object>>>());
            await dynamicPropertyStoreSubstitute.DidNotReceive().GetAsync(testDynamicProperty.Id);
        }

        [Fact]
        public async Task Should_Get_From_Db_If_Cache_Not_Exists_Async()
        {
            var dynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyStoreSubstitute = RegisterFake<IDynamicPropertyStore>();
            dynamicPropertyStoreSubstitute.GetAsync(dynamicProperty.Id).Returns(dynamicProperty);

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();
            var entity = await dynamicPropertyManager.GetAsync(dynamicProperty.Id);
            CheckEquality(entity, dynamicProperty);

            await dynamicPropertyStoreSubstitute.Received().GetAsync(dynamicProperty.Id);
        }

        [Fact]
        public async Task Should_Add_And_Change_Cache_Async()
        {
            var (cacheManager, dynamicPropertyStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();

            var testDynamicProperty = new DynamicProperty
            {
                PropertyName = "Test123",
                InputType = GetRandomAllowedInputType()
            };

            var result = await dynamicPropertyManager.AddAsync(testDynamicProperty);
            await cacheSubstitute.Received().SetAsync(
                testDynamicProperty.Id.ToString(),
                testDynamicProperty,
                Arg.Any<TimeSpan?>(),
                Arg.Any<DateTimeOffset?>()
            );

            await dynamicPropertyStoreSubstitute.Received().AddAsync(testDynamicProperty);

            result.ShouldBe(testDynamicProperty);
        }

        [Fact]
        public async Task Should_Not_Add_If_Input_Type_Is_Not_Exists_Async()
        {
            var testDynamicProperty = new DynamicProperty
            {
                PropertyName = "Test123",
                InputType = "asd123"
            };

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();
            try
            {
                await dynamicPropertyManager.AddAsync(testDynamicProperty);
                throw new Exception("Should check if input type exists");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain("asd123");
            }
        }

        [Fact]
        public async Task Should_Not_Add_If_Property_Name_Is_Null_Or_Empty_Async()
        {
            var testDynamicProperty = new DynamicProperty
            {
                PropertyName = string.Empty,
                InputType = Resolve<IDynamicEntityPropertyDefinitionManager>().GetAllAllowedInputTypeNames().First()
            };

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();

            var exception =
                await Should.ThrowAsync<ArgumentNullException>(() =>
                    dynamicPropertyManager.AddAsync(testDynamicProperty));
            exception.Message.ShouldContain(nameof(testDynamicProperty.PropertyName));

            var testDynamicProperty2 = new DynamicProperty
            {
                PropertyName = null,
                InputType = Resolve<IDynamicEntityPropertyDefinitionManager>().GetAllAllowedInputTypeNames().First()
            };

            var exception2 =
                await Should.ThrowAsync<ArgumentNullException>(() =>
                    dynamicPropertyManager.AddAsync(testDynamicProperty2));
            exception2.Message.ShouldContain(nameof(testDynamicProperty.PropertyName));
        }

        [Fact]
        public async Task Should_Update_And_Change_Cache_Async()
        {
            var testDynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var (cacheManager, dynamicPropertyStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();

            testDynamicProperty.PropertyName = "Test";

            var result = await dynamicPropertyManager.UpdateAsync(testDynamicProperty);
            await cacheSubstitute.Received().SetAsync(
                testDynamicProperty.Id.ToString(),
                testDynamicProperty,
                Arg.Any<TimeSpan?>(),
                Arg.Any<DateTimeOffset?>()
            );

            await dynamicPropertyStoreSubstitute.Received().UpdateAsync(testDynamicProperty);

            result.ShouldBe(testDynamicProperty);
        }

        [Fact]
        public async Task Should_Not_Update_If_Input_Type_Is_Not_Exists_Async()
        {
            var testDynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();
            testDynamicProperty.InputType = "asd123";
            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();
            try
            {
                await dynamicPropertyManager.UpdateAsync(testDynamicProperty);
                throw new Exception("Should check if input type exists");
            }
            catch (Exception e)
            {
                e.Message.ShouldContain("asd123");
            }
        }

        [Fact]
        public async Task Should_Not_Update_If_Property_Name_Is_Null_Or_Empty_Async()
        {
            var testDynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();

            testDynamicProperty.PropertyName = string.Empty;

            var exception =
                await Should.ThrowAsync<ArgumentNullException>(() =>
                    dynamicPropertyManager.UpdateAsync(testDynamicProperty));
            exception.Message.ShouldContain(nameof(testDynamicProperty.PropertyName));

            testDynamicProperty.PropertyName = null;

            var exception2 =
                await Should.ThrowAsync<ArgumentNullException>(() =>
                    dynamicPropertyManager.UpdateAsync(testDynamicProperty));
            exception2.Message.ShouldContain(nameof(testDynamicProperty.PropertyName));
        }

        [Fact]
        public async Task Should_Delete_And_Change_Cache_Async()
        {
            var testDynamicProperty = CreateAndGetDynamicPropertyWithTestPermission();

            var (cacheManager, dynamicPropertyStoreSubstitute, cacheSubstitute) = InitializeFakes();

            var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();

            await dynamicPropertyManager.DeleteAsync(testDynamicProperty.Id);
            await cacheSubstitute.Received().RemoveAsync(testDynamicProperty.Id.ToString());
            await dynamicPropertyStoreSubstitute.Received().DeleteAsync(testDynamicProperty.Id);
        }
    }
}