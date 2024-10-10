using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests;

public class Repository_Extension_Tests : EntityFrameworkCoreModuleTestBase
{
    private readonly IRepository<Blog> _blogRepository;
    private readonly IUnitOfWorkManager _uowManager;

    public Repository_Extension_Tests()
    {
        _uowManager = Resolve<IUnitOfWorkManager>();
        _blogRepository = Resolve<IRepository<Blog>>();
    }

    [Fact]
    public async Task Should_Insert_Range_New_Entities()
    {
        using var uow = _uowManager.Begin();
        var blog1 = new Blog("blog1", "http://myblog1.com");
        var blog2 = new Blog("blog2", "http://myblog2.com");
        var blog3 = new Blog("blog3", "http://myblog3.com");
        var blog4 = new Blog("blog4", "http://myblog4.com");

        blog1.IsTransient().ShouldBeTrue();
        blog2.IsTransient().ShouldBeTrue();
        blog3.IsTransient().ShouldBeTrue();
        blog4.IsTransient().ShouldBeTrue();

        _blogRepository.InsertRange(blog1, blog2);
        _blogRepository.InsertRange(new List<Blog> { blog3, blog4 });

        await uow.CompleteAsync();

        blog1.IsTransient().ShouldBeFalse();
        blog2.IsTransient().ShouldBeFalse();
        blog3.IsTransient().ShouldBeFalse();
        blog4.IsTransient().ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Insert_Range_Async_New_Entities()
    {
        using var uow = _uowManager.Begin();
        var blog1 = new Blog("blog1", "http://myblog1.com");
        var blog2 = new Blog("blog2", "http://myblog2.com");
        var blog3 = new Blog("blog3", "http://myblog3.com");
        var blog4 = new Blog("blog4", "http://myblog4.com");

        blog1.IsTransient().ShouldBeTrue();
        blog2.IsTransient().ShouldBeTrue();
        blog3.IsTransient().ShouldBeTrue();
        blog4.IsTransient().ShouldBeTrue();

        await _blogRepository.InsertRangeAsync(blog1, blog2);
        await _blogRepository.InsertRangeAsync(new List<Blog> { blog3, blog4 });

        await uow.CompleteAsync();

        blog1.IsTransient().ShouldBeFalse();
        blog2.IsTransient().ShouldBeFalse();
        blog3.IsTransient().ShouldBeFalse();
        blog4.IsTransient().ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Remove_Range_Entities()
    {
        var count = await _blogRepository.CountAsync();

        var blogs = new List<Blog>
            {
                new("blog1", "http://myblog1.com"),
                new("blog2", "http://myblog2.com"),
                new("blog3", "http://myblog3.com"),
                new("blog4", "http://myblog4.com")
            };

        using (var uow = _uowManager.Begin())
        {
            await _blogRepository.InsertRangeAsync(blogs);

            await uow.CompleteAsync();

            var afterCount = await _blogRepository.CountAsync();
            afterCount.ShouldBe(count + 4);
        }

        using (var uow = _uowManager.Begin())
        {
            _blogRepository.RemoveRange(blogs[0], blogs[1]);
            _blogRepository.RemoveRange(new List<Blog> { blogs[2], blogs[3] });

            await uow.CompleteAsync();

            var afterCount = await _blogRepository.CountAsync();
            afterCount.ShouldBe(count);
        }

    }
}