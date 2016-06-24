using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Tests.Domain;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests
{
    public class Repository_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> _blogRepository;
        private readonly IUnitOfWorkManager _uowManager;

        public Repository_Tests()
        {
            _uowManager = Resolve<IUnitOfWorkManager>();
            _blogRepository = Resolve<IRepository<Blog>>();
        }

        [Fact]
        public void Should_Get_Initial_Blogs()
        {
            //Act

            var blogs = _blogRepository.GetAllList();

            //Assert

            blogs.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Should_Automatically_Save_Changes_On_Uow()
        {
            int blog1Id;

            //Act

            using (var uow = _uowManager.Begin())
            {
                var blog1 = await _blogRepository.SingleAsync(b => b.Name == "test-blog-1");
                blog1Id = blog1.Id;

                blog1.Name = "test-blog-1-updated";

                await uow.CompleteAsync();
            }

            //Assert

            await UsingDbContextAsync(async context =>
            {
                var blog1 = await context.Blogs.SingleAsync(b => b.Id == blog1Id);
                blog1.Name.ShouldBe("test-blog-1-updated");
            });
        }
    }
}