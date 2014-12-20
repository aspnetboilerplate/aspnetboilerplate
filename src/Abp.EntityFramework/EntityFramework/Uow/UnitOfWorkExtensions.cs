using System;
using System.Data.Entity;
using Abp.Domain.Uow;

namespace Abp.EntityFramework.Uow
{
    internal static class UnitOfWorkExtensions
    {
        public static TDbContext GetDbContext<TDbContext>(this IActiveUnitOfWork unitOfWork) where TDbContext : DbContext
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            if (!(unitOfWork is EfUnitOfWork))
            {
                throw new ArgumentException("unitOfWork is not type of " + typeof(EfUnitOfWork).FullName, "unitOfWork");
            }

            return (unitOfWork as EfUnitOfWork).GetOrCreateDbContext<TDbContext>();
        }
    }
}