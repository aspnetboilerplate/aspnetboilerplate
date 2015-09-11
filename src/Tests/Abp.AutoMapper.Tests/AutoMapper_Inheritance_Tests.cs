using Shouldly;
using Xunit;

namespace Abp.AutoMapper.Tests
{
    public class AutoMapper_Inheritance_Tests
    {
        public AutoMapper_Inheritance_Tests()
        {
            AutoMapperHelper.CreateMap(typeof(MyTargetClassToMap));
            AutoMapperHelper.CreateMap(typeof(EntityDto));
            AutoMapperHelper.CreateMap(typeof(DerivedEntityDto));
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

        //[Fact] //TODO: That's a problem but related to AutoMapper rather than ABP.
        public void Should_Map_EntityProxy_To_EntityDto_And_To_DrivedEntityDto()
        {
            var proxy = new EntityProxy() { Value = "42"};
            var target = proxy.MapTo<EntityDto>();
            var target2 = proxy.MapTo<DerivedEntityDto>();
            target.Value.ShouldBe("42");
            target2.Value.ShouldBe("42");
        }

        public class Entity
        {
            public string Value { get; set; }
        }
        public class DerivedEntity : Entity { }
        public class EntityProxy : DerivedEntity { }

        [AutoMapFrom(typeof(Entity))]
        public class EntityDto
        {
            public string Value { get; set; }
        }

        [AutoMapFrom(typeof(DerivedEntity))]
        public class DerivedEntityDto : EntityDto { }
    }
}
