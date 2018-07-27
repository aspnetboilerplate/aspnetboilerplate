using System;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Uow
{
    /// <summary>
    /// Extension methods for UnitOfWork.
    /// </summary>
    public static class UnitOfWorkExtensions
    {
        /// <summary>
        /// Gets a DbContext as a part of active unit of work.
        /// This method can be called when current unit of work is an <see cref="EfCoreUnitOfWork"/>.
        /// </summary>
        /// <typeparam name="TDbContext">Type of the DbContext</typeparam>
        /// <param name="unitOfWork">Current (active) unit of work</param>
        /// <param name="multiTenancySide">Multitenancy side</param>
        /// <param name="name">
        /// A custom name for the dbcontext to get a named dbcontext.
        /// If there is no dbcontext in this unit of work with given name, then a new one is created.
        /// </param>
        public static TDbContext GetDbContext<TDbContext>(this IActiveUnitOfWork unitOfWork, MultiTenancySides? multiTenancySide = null, string name = null)
            where TDbContext : DbContext
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            if (!(unitOfWork is EfCoreUnitOfWork))
            {
                throw new ArgumentException("unitOfWork is not type of " + typeof(EfCoreUnitOfWork).FullName, "unitOfWork");
            }

            return (unitOfWork as EfCoreUnitOfWork).GetOrCreateDbContext<TDbContext>(multiTenancySide, name);
        }
    }
}