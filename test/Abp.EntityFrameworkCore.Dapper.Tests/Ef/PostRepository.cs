using System;

using Abp.EntityFrameworkCore.Dapper.Tests.Domain;
using Abp.EntityFrameworkCore.Repositories;

namespace Abp.EntityFrameworkCore.Dapper.Tests.Ef
{
    public class PostRepository : EfCoreRepositoryBase<BloggingDbContext, Post, Guid>, IPostRepository
    {
        public PostRepository(IDbContextProvider<BloggingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public override int Count()
        {
            throw new Exception("can not get count of posts");
        }
    }
}
