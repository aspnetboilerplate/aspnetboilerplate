using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Abp.AutoMapper.Tests
{
    public class AutoMapper_Inheritance_Tests
    {
        private readonly IMapper _mapper;

        public AutoMapper_Inheritance_Tests()
        {
            var config = new MapperConfiguration(configuration =>
            {
                configuration.CreateAutoAttributeMaps(typeof(MyTargetClassToMap));
                configuration.CreateAutoAttributeMaps(typeof(EntityDto));
                configuration.CreateAutoAttributeMaps(typeof(DerivedEntityDto));
                configuration.CreateAutoAttributeMaps(typeof(MyAutoMapKeyClass1));
                configuration.CreateAutoAttributeMaps(typeof(MyAutoMapKeyClass3));
                configuration.CreateAutoAttributeMaps(typeof(MyAutoMapKeyClass5));
                configuration.CreateAutoAttributeMaps(typeof(MyAutoMapKeyClass7));
                configuration.AddCollectionMappers();
                configuration.CreateMap<MyAutoMapKeyClass1, MyAutoMapKeyClass2>().EqualityComparison((x, y) => x.Id == y.Id);
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Should_Map_Derived_To_Target()
        {
            var derived = new MyDerivedClass { Value = "fortytwo" };
            var target = _mapper.Map<MyTargetClassToMap>(derived);
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

        [Fact]
        public void Should_Map_EntityProxy_To_EntityDto_And_To_DrivedEntityDto()
        {
            var proxy = new EntityProxy() { Value = "42" };
            var target = _mapper.Map<EntityDto>(proxy);
            var target2 = _mapper.Map<DerivedEntityDto>(proxy);
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

        private class MyEntityDto
        {
            [AutoMapKey]
            public int Id { get; set; }
        }

        private class MyDerivedEntityDto : Abp.Application.Services.Dto.EntityDto
        {
            [AutoMapKey]
            public new int Id { get; set; }
        }

        private class MyDualKeyEntityDto
        {
            [AutoMapKey]
            public int Id { get; set; }

            [AutoMapKey]
            public int SecondId { get; set; }
        }

        private class MyDerivedDualKeyEntityDto : Abp.Application.Services.Dto.EntityDto
        {
            [AutoMapKey]
            public new int Id { get; set; }

            [AutoMapKey]
            public int SecondId { get; set; }
        }

        private class MyDualKeyEntity
        {
           public int Id { get; set; }

            public int SecondId { get; set; }
        }

        private class MyAutoMapKeyClass1 : MyEntityDto
        {
            public string TestProp { get; set; }
        }

        private class MyAutoMapKeyClass2 : Abp.Domain.Entities.Entity
        {

            public string TestProp { get; set; }

            public int Value { get; set; }
        }

        [AutoMapTo(typeof(MyAutoMapKeyClass4))]
        private class MyAutoMapKeyClass3 : MyDerivedEntityDto
        {
            public string TestProp { get; set; }
        }

        private class MyAutoMapKeyClass4 : Abp.Domain.Entities.Entity
        {

            public string TestProp { get; set; }

            public int Value { get; set; }
        }

        [AutoMapTo(typeof(MyAutoMapKeyClass6))]
        private class MyAutoMapKeyClass5 : MyDualKeyEntityDto
        {
            public string TestProp { get; set; }
        }

        private class MyAutoMapKeyClass6 : MyDualKeyEntity
        {

            public string TestProp { get; set; }

            public int Value { get; set; }
        }

        [AutoMapTo(typeof(MyAutoMapKeyClass8))]
        private class MyAutoMapKeyClass7 : MyDerivedDualKeyEntityDto
        {
            public string TestProp { get; set; }
        }

        private class MyAutoMapKeyClass8 : MyDualKeyEntity
        {

            public string TestProp { get; set; }

            public int Value { get; set; }
        }

        [Fact]
        public void AutoMapKey_MapTo_DerivedCollection_Tests()
        {
            var list1 = new List<MyAutoMapKeyClass1>
                        {
                            new MyAutoMapKeyClass1 { Id = 1, TestProp = "New test value 1"},
                            new MyAutoMapKeyClass1 { Id = 2, TestProp = "New test value 2"}
                        };

            var list2 = new List<MyAutoMapKeyClass2>
                        {
                            new MyAutoMapKeyClass2 { Id = 1, TestProp = "Test value 1", Value = 5},
                            new MyAutoMapKeyClass2 { Id = 2, TestProp = "Test value 2", Value = 10}
                        };

            var list3 = new List<MyAutoMapKeyClass3>
                        {
                            new MyAutoMapKeyClass3 { Id = 1, TestProp = "New test value 1"},
                            new MyAutoMapKeyClass3 { Id = 2, TestProp = "New test value 2"}
                        };

            var list4 = new List<MyAutoMapKeyClass4>
                        {
                            new MyAutoMapKeyClass4 { Id = 1, TestProp = "Test value 1", Value = 5},
                            new MyAutoMapKeyClass4 { Id = 2, TestProp = "Test value 2", Value = 10}
                        };

            var list5 = new List<MyAutoMapKeyClass5>
                        {
                            new MyAutoMapKeyClass5 { Id = 1, SecondId = 2, TestProp = "New test value 1"},
                            new MyAutoMapKeyClass5 { Id = 2, SecondId = 3, TestProp = "New test value 2"}
                        };

            var list6 = new List<MyAutoMapKeyClass6>
                        {
                            new MyAutoMapKeyClass6 { Id = 1, SecondId = 2,  TestProp = "Test value 1", Value = 5},
                            new MyAutoMapKeyClass6 { Id = 2, SecondId = 3,  TestProp = "Test value 2", Value = 10}
                        };

            var list7 = new List<MyAutoMapKeyClass7>
                        {
                            new MyAutoMapKeyClass7 { Id = 1, SecondId = 2,  TestProp = "New test value 1"},
                            new MyAutoMapKeyClass7 { Id = 2, SecondId = 3,  TestProp = "New test value 2"}
                        };

            var list8 = new List<MyAutoMapKeyClass8>
                        {
                            new MyAutoMapKeyClass8 { Id = 1, SecondId = 2,  TestProp = "Test value 1", Value = 5},
                            new MyAutoMapKeyClass8 { Id = 2, SecondId = 3,  TestProp = "Test value 2", Value = 10}
                        };

            _mapper.Map(list1, list2);
            list2.Count.ShouldBe(2);
            list2[0].TestProp.ShouldBe("New test value 1");
            list2[0].Value.ShouldBe(5);
            list2[1].TestProp.ShouldBe("New test value 2");
            list2[1].Value.ShouldBe(10);

            _mapper.Map(list3, list4);
            list4.Count.ShouldBe(2);
            list4[0].TestProp.ShouldBe("New test value 1");
            list4[0].Value.ShouldBe(5);
            list4[1].TestProp.ShouldBe("New test value 2");
            list4[1].Value.ShouldBe(10);

            _mapper.Map(list5, list6);
            list6.Count.ShouldBe(2);
            list6[0].TestProp.ShouldBe("New test value 1");
            list6[0].Value.ShouldBe(5);
            list6[1].TestProp.ShouldBe("New test value 2");
            list6[1].Value.ShouldBe(10);

            _mapper.Map(list7, list8);
            list8.Count.ShouldBe(2);
            list8[0].TestProp.ShouldBe("New test value 1");
            list8[0].Value.ShouldBe(5);
            list8[1].TestProp.ShouldBe("New test value 2");
            list8[1].Value.ShouldBe(10);
        }
    }
}
