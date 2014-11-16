using AutoMapper;
using Shouldly;
using Xunit;

namespace Abp.AutoMapper.Tests
{
    public class MapTo_Tests
    {
        [Fact]
        public void Test1()
        {
            //ABP will automatically find and create these mappings!
            Mapper.CreateMap<MyClass1, MyClass2>();
            Mapper.CreateMap<MyClass1, MyClass3>();
            
            var obj1 = new MyClass1 { TestProp = "Test value" };
            
            var obj2 = obj1.MapTo<MyClass2>();
            obj2.ShouldNotBe(null);
            obj2.TestProp.ShouldBe("Test value");

            var obj3 = obj1.MapTo<MyClass3>();
            obj3.ShouldNotBe(null);
            obj3.TestProp.ShouldBe("Test value");
        }

        private class MyClass1 : IMapTo<MyClass2>, IMapTo<MyClass3>
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
