using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Abp.EntityFrameworkCore.Tests.Ef;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class DbQuery_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> _blogRepository;

        public DbQuery_Tests()
        {
            _blogRepository = Resolve<IRepository<Blog>>();
        }

        [Fact]
        public async Task DbQuery_Test()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var blogViews = (_blogRepository.GetDbContext() as BloggingDbContext)?.BlogView.ToList();

                blogViews.ShouldNotBeNull();
                blogViews.ShouldContain(x => x.Name == "test-blog-1" && x.Url == "http://testblog1.myblogs.com");

                await uow.CompleteAsync();
            }
        }
    }
}