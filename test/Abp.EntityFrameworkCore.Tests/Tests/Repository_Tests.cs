using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Tests.Domain;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class Repository_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<Post, Guid> _postRepository;
        private readonly IUnitOfWorkManager _uowManager;

        public Repository_Tests()
        {
            _uowManager = Resolve<IUnitOfWorkManager>();
            _blogRepository = Resolve<IRepository<Blog>>();
            _postRepository = Resolve<IRepository<Post, Guid>>();
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

        [Fact]
        public async Task Should_Not_Include_Navigation_Properties_If_Not_Requested()
        {
            //EF Core does not support lazy loading yet, so navigation properties will not be loaded if not included

            using (var uow = _uowManager.Begin())
            {
                var post = await _postRepository.GetAll().FirstAsync();

                post.Blog.ShouldBeNull();

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Include_Navigation_Properties_If_Requested()
        {
            using (var uow = _uowManager.Begin())
            {
                var post = await _postRepository.GetAllIncluding(p => p.Blog).FirstAsync();

                post.Blog.ShouldNotBeNull();
                post.Blog.Name.ShouldBe("test-blog-1");

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Insert_New_Entity()
        {
            using (var uow = _uowManager.Begin())
            {
                var blog = new Blog("blog2", "http://myblog2.com");
                blog.IsTransient().ShouldBeTrue();
                await _blogRepository.InsertAsync(blog);
                await uow.CompleteAsync();
                blog.IsTransient().ShouldBeFalse();
            }
        }

        [Fact]
        public async Task Should_Insert_New_Entity_With_Guid_Id()
        {
            using (var uow = _uowManager.Begin())
            {
                var blog1 = await _blogRepository.GetAsync(1);
                var post = new Post(blog1, "a test title", "a test body");
                post.IsTransient().ShouldBeTrue();
                await _postRepository.InsertAsync(post);
                await uow.CompleteAsync();
                post.IsTransient().ShouldBeFalse();
            }
        }
    }
}