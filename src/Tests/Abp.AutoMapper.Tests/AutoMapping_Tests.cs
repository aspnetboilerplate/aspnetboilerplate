using AutoMapper;
using Shouldly;
using Xunit;

namespace Abp.AutoMapper.Tests
{
    public class AutoMapping_Tests
    {
        static AutoMapping_Tests()
        {
            //ABP will automatically find and create these mappings!
            Mapper.CreateMap<MyClass1, MyClass2>().ReverseMap();
            Mapper.CreateMap<MyClass1, MyClass3>().ReverseMap();
        }

        [Fact]
        public void MapTo_Tests()
        {
            var obj1 = new MyClass1 { TestProp = "Test value" };

            var obj2 = obj1.Map<MyClass2>();
            obj2.ShouldNotBe(null);
            obj2.TestProp.ShouldBe("Test value");

            var obj3 = obj1.Map<MyClass3>();
            obj3.ShouldNotBe(null);
            obj3.TestProp.ShouldBe("Test value");
        }

        [Fact]
        public void MapFrom_Tests()
        {
            var obj2 = new MyClass2 { TestProp = "Test value" };

            var obj1 = obj2.Map<MyClass2, MyClass1>();

            obj1.ShouldNotBe(null);
            obj1.TestProp.ShouldBe("Test value");
        }

        private class MyClass1 :
            IAutoMap<MyClass2>,
            IAutoMap<MyClass3>
        {
            public string TestProp { get; set; }
        }

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
