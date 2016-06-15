using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.TestBase;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests
{
    public class Repository_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> _blogRepository;

        public Repository_Tests()
        {
            _blogRepository = Resolve<IRepository<Blog>>();
        }

        [Fact]
        public void Should_Get_Initial_Blogs()
        {
            var blogs = _blogRepository.GetAllList();
            blogs.Count.ShouldBeGreaterThan(0);
        }
    }
}