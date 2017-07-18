using System;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.Timing;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class EntityUtcDateTime_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> _blogRepository;

        public EntityUtcDateTime_Tests()
        {
            _blogRepository = Resolve<IRepository<Blog>>();
        }

        [Fact]
        public void DateTimes_Should_Be_UTC()
        {
            Clock.Kind.ShouldBe(DateTimeKind.Utc);

            //Act

            var blogs = _blogRepository.GetAllList();

            //Assert

            blogs.Count.ShouldBeGreaterThan(0);

            foreach (var blog in blogs)
            {
                blog.CreationTime.Kind.ShouldBe(DateTimeKind.Utc);
            }
        }
    }
}
