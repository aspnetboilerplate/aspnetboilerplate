using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class DbQuery_Tests : EntityFrameworkCoreModuleTestBase
    {
        [Fact]
        public async Task DbQuery_Test()
        {
            var blogViewRepository = Resolve<IRepository<BlogView>>();

            var blogViews = blogViewRepository.GetAllList();

            blogViews.ShouldNotBeNull();
            blogViews.ShouldContain(x => x.Name == "test-blog-1" && x.Url == "http://testblog1.myblogs.com");
        }
    }
}