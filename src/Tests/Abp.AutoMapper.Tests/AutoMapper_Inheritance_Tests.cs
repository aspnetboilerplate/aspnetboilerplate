using Shouldly;
using Xunit;

namespace Abp.AutoMapper.Tests
{
    public class AutoMapper_Inheritance_Tests
    {
        public AutoMapper_Inheritance_Tests()
        {
            AutoMapperHelper.CreateMap(typeof(MyTargetClassToMap));
        }

        [Fact]
        public void Should_Map_Derived_To_Target()
        {
            var derived = new MyDerivedClass { Value = "fortytwo" };
            var target = derived.MapTo<MyTargetClassToMap>();
            target.Value.ShouldBe("fortytwo");
        }

        public class MyBaseClass
        {
            public string Value { get; set; }
        }

        public class MyDerivedClass : MyBaseClass
        {

        }

        [AutoMapFrom(typeof(MyBaseClass))]
        public class MyTargetClassToMap
        {
            public string Value { get; set; }
        }
    }
}
