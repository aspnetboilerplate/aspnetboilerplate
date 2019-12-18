using Abp.TestBase;
using Shouldly;
using Xunit;

namespace Abp.AutoMapper.Tests
{
    public class StaticAutoMapper_Tests : AbpIntegratedTestBase<AbpAutoMapperTestModule>
    {
        [Fact]
        public void StaticAutoMapper_Test()
        {
            AbpEmulateAutoMapper.Mapper.ShouldNotBeNull();

            var a = new ClassA
            {
                Id = 1,
                Name = "test1"
            };

            var b = a.MapTo<ClassB>();

            b.Id.ShouldBe(1);
            b.Name.ShouldBe("test1");


            var c = new ClassB
            {
                Id = 2,
                Name = "test2"
            };

            a.MapTo(c);

            c.Id.ShouldBe(1);
            c.Name.ShouldBe("test1");
        }

        private class ClassA
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        [AutoMapFrom(typeof(ClassA))]
        private class ClassB
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}