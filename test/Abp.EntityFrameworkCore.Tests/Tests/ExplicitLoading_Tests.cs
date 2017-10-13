using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Tests.Domain;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class ExplicitLoading_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<Post, Guid> _postRepository;

        public ExplicitLoading_Tests()
        {
            _blogRepository = Resolve<IRepository<Blog>>();
            _postRepository = Resolve<IRepository<Post, Guid>>();
        }

        [Fact]
        public async Task Should_Explicitly_Load_Collections()
        {
            var uowManager = Resolve<IUnitOfWorkManager>();
            using (var uow = uowManager.Begin())
            {
                var blog = await _blogRepository.FirstOrDefaultAsync(b => b.Name == "test-blog-1");
                blog.ShouldNotBeNull();
                blog.Posts.ShouldBeNull(); //Because EF core does not have lazy loading yet!

                await _blogRepository.EnsureCollectionLoadedAsync(blog, b => b.Posts);
                blog.Posts.ShouldNotBeNull(); //Now loaded it!
                blog.Posts.Count.ShouldBeGreaterThan(0);

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Explicitly_Load_Properties()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var post = await _postRepository.FirstOrDefaultAsync(b => b.Title == "test-post-1-title");
                post.ShouldNotBeNull();
                post.Blog.ShouldBeNull(); //Because EF core does not have lazy loading yet!

                await _postRepository.EnsurePropertyLoadedAsync(post, p => p.Blog);
                post.Blog.ShouldNotBeNull(); //Now loaded it!
                post.Blog.Name.ShouldBe("test-blog-1");

                await uow.CompleteAsync();
            }
        }
    }
}
