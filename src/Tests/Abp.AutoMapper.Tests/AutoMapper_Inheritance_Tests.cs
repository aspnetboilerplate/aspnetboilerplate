using Shouldly;
using Xunit;

namespace Abp.AutoMapper.Tests
{
    public class AutoMapper_Inheritance_Tests
    {
        public AutoMapper_Inheritance_Tests()
        {
            AutoMapperHelper.CreateMap(typeof(MyTargetClassToMap));
            AutoMapperHelper.CreateMap(typeof(MyTargetDerivedClassToMap));
        }

        [Fact]
        public void Should_Map_Derived_To_Target()
        {
            var derived = new MyDerivedClass { Value = "fortytwo" };
            var target = derived.MapTo<MyTargetClassToMap>();
            target.Value.ShouldBe("fortytwo");
        }

        [Fact]
        public void Should_Map_SubDerived_To_Base_And_To_Drived()
        {
            var subDerived = new MySubDerivedClass { Value = "fortytwo" };
            var target = subDerived.MapTo<MyTargetClassToMap>();
            var target2 = subDerived.MapTo<MyTargetDerivedClassToMap>();
            target.Value.ShouldBe("fortytwo");
            target2.Value.ShouldBe("fortytwo");
        }

        public class MyBaseClass
        {
            public string Value { get; set; }
        }

        public class MyDerivedClass : MyBaseClass
        {

        }

        public class MySubDerivedClass : MyDerivedClass
        {
            
        }

        [AutoMapFrom(typeof(MyBaseClass))]
        public class MyTargetClassToMap
        {
            public string Value { get; set; }
        }

        [AutoMapFrom(typeof(MyDerivedClass))]
        public class MyTargetDerivedClassToMap : MyTargetClassToMap
        {
            
        }
    }
}
