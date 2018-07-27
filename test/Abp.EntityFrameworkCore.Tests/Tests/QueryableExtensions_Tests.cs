using System.Linq;
using Abp.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class QueryableExtensions_Tests : EntityFrameworkCoreModuleTestBase
    {
        [Fact]
        public void Ef_Core_IncludeIf_Extension_With_Query_Test()
        {
            UsingDbContext(context =>
            {
                var blog = context.Blogs.Where(b => b.Name == "test-blog-1")
                                  .IncludeIf(true, e => e.Include(b => b.Posts).ThenInclude(p => p.Comments))
                                  .FirstOrDefault();

                blog.ShouldNotBeNull();
                blog.Name.ShouldBe("test-blog-1");
                
                var post = blog.Posts.FirstOrDefault(p => p.Title == "test-post-1-title");

                post.ShouldNotBeNull();
                post.Comments.Count.ShouldBe(1);
            });
        }

        [Fact]
        public void Ef_Core_IncludeIf_Extension_With_Path_Test()
        {
            UsingDbContext(context =>
            {
                var blog = context.Blogs.Where(b => b.Name == "test-blog-1")
                    .IncludeIf(true, "Posts")
                    .IncludeIf(true, "Posts.Comments")
                    .FirstOrDefault();

                blog.ShouldNotBeNull();
                blog.Name.ShouldBe("test-blog-1");

                var post = blog.Posts.FirstOrDefault(p => p.Title == "test-post-1-title");

                post.ShouldNotBeNull();
                post.Comments.Count.ShouldBe(1);
            });
        }
    }
}
