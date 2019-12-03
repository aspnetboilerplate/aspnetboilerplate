using System;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Shouldly;
using Xunit;

namespace Abp.AutoMapper.Tests
{
    public class AutoMapping_Tests
    {
        private readonly IMapper _mapper;

        public AutoMapping_Tests()
        {
            var config = new MapperConfiguration(configuration =>
            {
                configuration.CreateAutoAttributeMaps(typeof(MyClass1));
                configuration.CreateAutoAttributeMaps(typeof(MyClass2));
                configuration.CreateAutoAttributeMaps(typeof(MyAutoMapKeyClass1));
                configuration.CreateAutoAttributeMaps(typeof(MyAutoMapKeyClass2));
                configuration.AddCollectionMappers();
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Map_Null_Tests()
        {
            MyClass1 obj1 = null;
            var obj2 = _mapper.Map<MyClass2>(obj1);
            obj2.ShouldBe(null);
        }

        [Fact]
        public void Map_Null_Existing_Object_Tests()
        {
            MyClass1 obj1 = null;

            var obj2 = new MyClass2 { TestProp = "before map" };
            _mapper.Map(obj1, obj2);
            obj2.TestProp.ShouldBe("before map");
        }

        [Fact]
        public void MapTo_Tests()
        {
            var obj1 = new MyClass1 { TestProp = "Test value" };

            var obj2 = _mapper.Map<MyClass2>(obj1);
            obj2.TestProp.ShouldBe("Test value");

            var obj3 = _mapper.Map<MyClass3>(obj1);
            obj3.TestProp.ShouldBe("Test value");
        }

        [Fact]
        public void Should_Map_Two_Way_When_AutoMap_Attribute_Is_Used()
        {
            MyClass3 obj2 = new MyClass3
            {
                TestProp = "test",
                AnotherValue = 1
            };

            var obj1 = _mapper.Map<MyClass1>(obj2);

            obj1.TestProp.ShouldBe("test");
        }

        [Fact]
        public void MapTo_Existing_Object_Tests()
        {
            var obj1 = new MyClass1 { TestProp = "Test value" };

            var obj2 = new MyClass2();
            _mapper.Map(obj1, obj2);
            obj2.TestProp.ShouldBe("Test value");

            var obj3 = new MyClass3();
            _mapper.Map(obj2, obj3);
            obj3.TestProp.ShouldBe("Test value");

            Assert.ThrowsAny<Exception>(() => //Did not define reverse mapping!
            {
                _mapper.Map(obj3, obj2);
            });
        }

        [Fact]
        public void MapFrom_Tests()
        {
            var obj2 = new MyClass2 { TestProp = "Test value" };

            var obj1 = _mapper.Map<MyClass1>(obj2);
            obj1.TestProp.ShouldBe("Test value");
        }

        [Fact]
        public void IgnoreMap_Tests()
        {
            var obj2 = new MyClass2 { TestProp = "Test value", AnotherValue = 42 };
            var obj3 = _mapper.Map<MyClass3>(obj2);
            obj3.TestProp.ShouldBe("Test value");
            obj3.AnotherValue.ShouldBe(0); //Ignored because of IgnoreMap attribute!
        }

        [Fact]
        public void MapTo_Collection_Tests()
        {
            var list1 = new List<MyClass1>
                        {
                            new MyClass1 {TestProp = "Test value 1"},
                            new MyClass1 {TestProp = "Test value 2"}
                        };

            var list2 = _mapper.Map<List<MyClass2>>(list1);
            list2.Count.ShouldBe(2);
            list2[0].TestProp.ShouldBe("Test value 1");
            list2[1].TestProp.ShouldBe("Test value 2");
        }

        [Fact]
        public void AutoMapKey_MapTo_Collection_Tests()
        {
            var list1 = new List<MyAutoMapKeyClass1>
                        {
                            new MyAutoMapKeyClass1 { Id = 1, TestProp = "New test value 1"},
                            new MyAutoMapKeyClass1 { Id = 2, TestProp = "New test value 2"}
                        };
            var list2 = new List<MyAutoMapKeyClass2>
                        {
                            new MyAutoMapKeyClass2 { Id = 1, SecondId = 10, ThirdId = 100, TestProp = "Test value 1", Value = 5},
                            new MyAutoMapKeyClass2 { Id = 2,  SecondId = 20, ThirdId = 200,TestProp = "Test value 2", Value = 10}
                        };
            var list3 = new List<MyAutoMapKeyClass3>
                        {
                            new MyAutoMapKeyClass3 { SecondId = 10, ThirdId = 100, TestProp = "Test value 1", SecondValue = 50},
                            new MyAutoMapKeyClass3 { SecondId = 20, ThirdId = 200, TestProp = "Test value 2", SecondValue = 100}
                        };

            _mapper.Map(list1, list2);
            list2.Count.ShouldBe(2);
            list2[0].TestProp.ShouldBe("New test value 1");
            list2[0].Value.ShouldBe(5);
            list2[1].TestProp.ShouldBe("New test value 2");
            list2[1].Value.ShouldBe(10);

            _mapper.Map(list2, list3);
            list3.Count.ShouldBe(2);
            list3[0].TestProp.ShouldBe("New test value 1");
            list3[0].SecondValue.ShouldBe(50);
            list3[1].TestProp.ShouldBe("New test value 2");
            list3[1].SecondValue.ShouldBe(100);
        }

        [Fact]
        public void Map_Should_Set_Null_Existing_Object_Tests()
        {
            MyClass1 obj1 = new MyClass1 { TestProp = null };
            var obj2 = new MyClass2 { TestProp = "before map" };
            _mapper.Map(obj1, obj2);
            obj2.TestProp.ShouldBe(null);
        }

        [Fact]
        public void Should_Map_Nullable_Value_To_Null_If_It_Is_Null_On_Source()
        {
            var obj1 = new MyClass1();
            var obj2 = _mapper.Map<MyClass2>(obj1);
            obj2.NullableValue.ShouldBeNull();
        }

        [Fact]
        public void Should_Map_Nullable_Value_To__Not_Null_If_It_Is__Not_Null_On_Source()
        {
            var obj1 = new MyClass1 { NullableValue = 42 };
            var obj2 = _mapper.Map<MyClass2>(obj1);
            obj2.NullableValue.ShouldBe(42);
        }

        [AutoMap(typeof(MyClass2), typeof(MyClass3))]
        private class MyClass1
        {
            public string TestProp { get; set; }

            public long? NullableValue { get; set; }
        }

        [AutoMapTo(typeof(MyClass3))]
        private class MyClass2
        {
            public string TestProp { get; set; }

            public long? NullableValue { get; set; }

            public int AnotherValue { get; set; }
        }

        private class MyClass3
        {
            public string TestProp { get; set; }

            [IgnoreMap]
            public int AnotherValue { get; set; }
        }

        [AutoMapTo(typeof(MyAutoMapKeyClass2))]
        private class MyAutoMapKeyClass1
        {
            [AutoMapKey]
            public int Id { get; set; }

            public string TestProp { get; set; }
        }

        [AutoMapTo(typeof(MyAutoMapKeyClass3))]
        private class MyAutoMapKeyClass2
        {
            public int Id { get; set; }

            [AutoMapKey]
            public int SecondId { get; set; }

            [AutoMapKey]
            public int ThirdId { get; set; }

            public string TestProp { get; set; }

            public int Value { get; set; }
        }

        private class MyAutoMapKeyClass3
        {
            public int SecondId { get; set; }

            public int ThirdId { get; set; }

            public string TestProp { get; set; }

            public int SecondValue { get; set; }
        }
    }
}
