using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Domain.Repositories
{
    public interface IFromDB<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {//20150919
        /// <summary>
        /// Gets an entity with given primary key or null if not found.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity or null</returns>
        Task<TEntity> FirstOrDefaultAsyncFromDB(TPrimaryKey id);
        IQueryable<TEntity> GetAllFromDB();
    }
}
