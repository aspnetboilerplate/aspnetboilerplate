using System;
using System.Data.Entity;
using Abp.Domain.Uow;

namespace Abp.Domain.Repositories.EntityFramework
{
    public static class UnitOfWorkExtensions
    {
        public static TDbContext GetDbContext<TDbContext>(this IUnitOfWork unitOfWork) where TDbContext : DbContext
        {
            var uow = unitOfWork as EfUnitOfWork;
            if (uow == null)
            {
                throw new ArgumentException("unitOfWork is not type of " + typeof(EfUnitOfWork).FullName, "unitOfWork");
            }

            return uow.GetOrCreateDbContext<TDbContext>();
        }
    }
}