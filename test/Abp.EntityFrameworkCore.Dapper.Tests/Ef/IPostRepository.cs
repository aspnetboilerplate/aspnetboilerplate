using System;

using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Dapper.Tests.Domain;

namespace Abp.EntityFrameworkCore.Dapper.Tests.Ef;

public interface IPostRepository : IRepository<Post, Guid>
{
}