using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Specifications;

namespace Abp.Domain.Specifications
{
    /// <summary>
    /// Base specification for guid entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EntitySpecification<TEntity>
        where TEntity : class, IEntity<Guid>
    {

        /// <summary>
        /// the true spec
        /// </summary>
        /// <returns></returns>
        public static Specification<TEntity> Any()
        {
            return new AnySpecification<TEntity>();
        }

        /// <summary>
        /// find entity by key id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Specification<TEntity> IsKey(Guid id)
        {
            return new DirectSpecification<TEntity>(entity => entity.Id == id);
        }

        /// <summary>
        /// find entity list by key ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static Specification<TEntity> InKeys(List<Guid> ids)
        {
            if (ids != null && ids.Count > 0)
                return new DirectSpecification<TEntity>(entity => ids.Contains(entity.Id));
            else
                return !Any();
        }

    }

    /// <summary>
    /// Base spec for guid entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EntitySpecification<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {

        /// <summary>
        /// the true spec
        /// </summary>
        /// <returns></returns>
        public static Specification<TEntity> Any()
        {
            return new AnySpecification<TEntity>();
        }

        /// <summary>
        /// find entity by key id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Specification<TEntity> IsKey(TPrimaryKey id)
        {
            return new DirectSpecification<TEntity>(entity => entity.Id.Equals(id));
        }

        /// <summary>
        /// find entity list by key ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static Specification<TEntity> InKeys(List<TPrimaryKey> ids)
        {
            if (ids != null && ids.Count > 0)
                return new DirectSpecification<TEntity>(entity => ids.Contains(entity.Id));
            else
                return !Any();
        }

    }
}
