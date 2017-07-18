using System;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;

namespace Abp.EntityFrameworkCore.Tests.Ef
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
    }
}