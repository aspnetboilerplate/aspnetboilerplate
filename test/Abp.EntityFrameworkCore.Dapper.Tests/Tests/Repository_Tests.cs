using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Dapper.Tests.Domain;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Dapper.Tests.Tests
{
    public class Repository_Tests : AbpEfCoreDapperTestApplicationBase
    {
        private readonly IDapperRepository<Blog> _blogDapperRepository;
        private readonly IRepository<Blog> _blogRepository;
        private readonly IDapperRepository<Post, Guid> _postDapperRepository;
        private readonly IRepository<Post, Guid> _postRepository;
        private readonly IRepository<Comment, long> _commentRepository;
        private readonly IDapperRepository<Comment, long> _commentDapperRepository;
        private readonly IUnitOfWorkManager _uowManager;

        public Repository_Tests()
        {
            _uowManager = Resolve<IUnitOfWorkManager>();
            _blogRepository = Resolve<IRepository<Blog>>();
            _postRepository = Resolve<IRepository<Post, Guid>>();
            _blogDapperRepository = Resolve<IDapperRepository<Blog>>();
            _postDapperRepository = Resolve<IDapperRepository<Post, Guid>>();
            _commentRepository = Resolve<IRepository<Comment, long>>();
            _commentDapperRepository = Resolve<IDapperRepository<Comment, long>>();
        }

        [Fact]
        public void Should_Get_Initial_Blogs()
        {
            //Act
            List<Blog> blogs = _blogRepository.GetAllList();
            IEnumerable<Blog> blogsFromDapper = _blogDapperRepository.GetAll();

            //Assert
            blogs.Count.ShouldBeGreaterThan(0);
            blogsFromDapper.Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Should_Automatically_Save_Changes_On_Uow()
        {
            int blog1Id;
            int blog2Id;

            //Act

            using (IUnitOfWorkCompleteHandle uow = _uowManager.Begin())
            {
                Blog blog1 = await _blogRepository.SingleAsync(b => b.Name == "test-blog-1");
                blog1Id = blog1.Id;

                blog1.Name = "test-blog-1-updated";

                await _blogDapperRepository.InsertAsync(new Blog("test-blog-2", "www"));

                Blog blog2 = await _blogRepository.SingleAsync(x => x.Name == "test-blog-2");
                blog2Id = blog2.Id;

                blog2.Name = "test-blog-2-updated";

                await _blogDapperRepository.UpdateAsync(blog2);

                await uow.CompleteAsync();
            }

            //Assert

            await UsingDbContextAsync(async context =>
            {
                Blog blog1 = await context.Blogs.SingleAsync(b => b.Id == blog1Id);
                blog1.Name.ShouldBe("test-blog-1-updated");

                Blog blog2 = await context.Blogs.SingleAsync(b => b.Id == blog2Id);
                blog2.Name.ShouldBe("test-blog-2-updated");
            });
        }

        [Fact]
        public async Task Should_automatically_save_changes_on_uow_completed_with_dapper()
        {
            int blog1Id;

            //Act
            using (IUnitOfWorkCompleteHandle uow = _uowManager.Begin())
            {
                Blog blog1 = await _blogDapperRepository.SingleAsync(b => b.Name == "test-blog-1");
                blog1Id = blog1.Id;

                blog1.Name = "test-blog-1-updated";
                await _blogDapperRepository.UpdateAsync(blog1);

                await uow.CompleteAsync();
            }

            //Assert

            await UsingDbContextAsync(async context =>
            {
                Blog blog1 = await context.Blogs.SingleAsync(b => b.Id == blog1Id);
                blog1.Name.ShouldBe("test-blog-1-updated");
            });
        }

        [Fact]
        public async Task Should_Not_Include_Navigation_Properties_If_Not_Requested()
        {
            //EF Core does not support lazy loading yet, so navigation properties will not be loaded if not included

            using (IUnitOfWorkCompleteHandle uow = _uowManager.Begin())
            {
                Post post = await _postRepository.GetAll().FirstAsync();

                post.Blog.ShouldBeNull();

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Include_Navigation_Properties_If_Requested()
        {
            using (IUnitOfWorkCompleteHandle uow = _uowManager.Begin())
            {
                Post post = await _postRepository.GetAllIncluding(p => p.Blog).FirstAsync();

                post.Blog.ShouldNotBeNull();
                post.Blog.Name.ShouldBe("test-blog-1");

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Insert_New_Entity()
        {
            using (IUnitOfWorkCompleteHandle uow = _uowManager.Begin())
            {
                var blog = new Blog("blog2", "http://myblog2.com");
                blog.IsTransient().ShouldBeTrue();
                await _blogRepository.InsertAsync(blog);
                await uow.CompleteAsync();
                blog.IsTransient().ShouldBeFalse();
            }
        }

        [Fact]
        public async Task Should_Insert_New_Entity_with_dapper()
        {
            using (IUnitOfWorkCompleteHandle uow = _uowManager.Begin())
            {
                var blog = new Blog("blog2", "http://myblog2.com");
                blog.IsTransient().ShouldBeTrue();
                await _blogDapperRepository.InsertAsync(blog);
                await uow.CompleteAsync();
                blog.IsTransient().ShouldBeFalse();
            }
        }

        [Fact]
        public async Task Should_Insert_New_Entity_With_Guid_Id()
        {
            using (IUnitOfWorkCompleteHandle uow = _uowManager.Begin())
            {
                Blog blog1 = await _blogRepository.GetAsync(1);
                var post = new Post(blog1, "a test title", "a test body");
                post.IsTransient().ShouldBeTrue();
                await _postRepository.InsertAsync(post);
                await uow.CompleteAsync();
                post.IsTransient().ShouldBeFalse();
            }
        }

        [Fact]
        public async Task Should_Insert_New_Entity_With_Guid_Id_with_dapper()
        {
            using (IUnitOfWorkCompleteHandle uow = _uowManager.Begin())
            {
                Blog blog1 = await _blogRepository.GetAsync(1);
                var post = new Post(blog1.Id, "a test title", "a test body");
                post.IsTransient().ShouldBeTrue();
                await _postDapperRepository.InsertAsync(post);
                await uow.CompleteAsync();
                post.IsTransient().ShouldBeFalse();
            }
        }

        [Fact]
        public void Dapper_and_EfCore_should_work_under_same_unitofwork()
        {
            using (IUnitOfWorkCompleteHandle uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                int blogId = _blogDapperRepository.InsertAndGetId(new Blog("Oguzhan_Same_Uow", "www"));

                Blog blog = _blogRepository.Get(blogId);

                blog.ShouldNotBeNull();

                uow.Complete();
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(CommandType.Text)]
        public async Task execute_method_for_void_sqls_should_work(CommandType? commandType)
        {
            int blogId = await _blogDapperRepository.InsertAndGetIdAsync(
                new Blog("Oguzhan_Blog", "wwww.aspnetboilerplate.com")
            );

            await _blogDapperRepository.ExecuteAsync(
                "Update Blogs Set Name = @name where Id =@id",
                new
                {
                    id = blogId,
                    name = "Oguzhan_New_Blog"
                },
                commandType
            );

            (await _blogDapperRepository.GetAsync(blogId)).Name.ShouldBe("Oguzhan_New_Blog");
            (await _blogRepository.GetAsync(blogId)).Name.ShouldBe("Oguzhan_New_Blog");
        }

        [Fact]
        public void querying_with_TEntity_TPrimaryKey_should_work_on_dapper_repositories()
        {
            _commentRepository.Insert(new Comment("hey!"));

            List<Comment> comments = _commentDapperRepository.Query("select * from Comments").ToList();
            List<Comment> comments2 = _commentDapperRepository.Query<Comment>("select * from Comments").ToList();

            comments2.Count.ShouldBe(1);
            comments.Count.ShouldBe(1);
        }
    }
}
