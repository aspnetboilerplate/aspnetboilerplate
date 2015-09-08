using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Abp.AutoMapper.Tests
{
    public class AutoMapping_Tests
    {
        static AutoMapping_Tests()
        {
            //ABP will automatically find and create these mappings normally. This is just for test purposes.
            AutoMapperHelper.CreateMap(typeof(MyClass1));
            AutoMapperHelper.CreateMap(typeof(MyClass2));
        }

        [Fact]
        public void Map_Null_Tests()
        {
            MyClass1 obj1 = null;
            var obj2 = obj1.MapTo<MyClass2>();
            obj2.ShouldBe(null);
        }

        [Fact]
        public void Map_Null_Existing_Object_Tests()
        {
            MyClass1 obj1 = null;

            var obj2 = new MyClass2 { TestProp = "before map" };
            obj1.MapTo(obj2);
            obj2.TestProp.ShouldBe("before map");
        }

        [Fact]
        public void MapTo_Tests()
        {
            var obj1 = new MyClass1 { TestProp = "Test value" };

            var obj2 = obj1.MapTo<MyClass2>();
            obj2.TestProp.ShouldBe("Test value");

            var obj3 = obj1.MapTo<MyClass3>();
            obj3.TestProp.ShouldBe("Test value");
        }

        [Fact]
        public void MapTo_Existing_Object_Tests()
        {
            var obj1 = new MyClass1 { TestProp = "Test value" };

            var obj2 = new MyClass2();
            obj1.MapTo(obj2);
            obj2.TestProp.ShouldBe("Test value");

            var obj3 = new MyClass3();
            obj2.MapTo(obj3);
            obj3.TestProp.ShouldBe("Test value");
        }

        [Fact]
        public void MapFrom_Tests()
        {
            var obj2 = new MyClass2 { TestProp = "Test value" };

            var obj1 = obj2.MapTo<MyClass1>();
            obj1.TestProp.ShouldBe("Test value");
        }

        [Fact]
        public void MapTo_Collection_Tests()
        {
            var list1 = new List<MyClass1>
                        {
                            new MyClass1 {TestProp = "Test value 1"},
                            new MyClass1 {TestProp = "Test value 2"}
                        };

            var list2 = list1.MapTo<List<MyClass2>>();
            list2.Count.ShouldBe(2);
            list2[0].TestProp.ShouldBe("Test value 1");
            list2[1].TestProp.ShouldBe("Test value 2");
        }

        [Fact]
        public void Map_Should_Set_Null_Existing_Object_Tests()
        {
            MyClass1 obj1 = new MyClass1 { TestProp = null };
            var obj2 = new MyClass2 { TestProp = "before map" };
            obj1.MapTo(obj2);
            obj2.TestProp.ShouldBe(null);
        }

        [AutoMap(typeof(MyClass2), typeof(MyClass3))]
        private class MyClass1
        {
            public string TestProp { get; set; }
        }

        [AutoMapTo(typeof(MyClass3))]
        private class MyClass2
        {
            public string TestProp { get; set; }
        }

        private class MyClass3
        {
            public string TestProp { get; set; }
        }
    }
}
