using System;
using System.Data.Entity;

namespace Abp.EntityFramework
{
    internal class EntityTypeInfo
    {
        public EntityTypeInfo(Type entityType, Type declaringType)
        {
            EntityType = entityType;
            DeclaringType = declaringType;
        }

        /// <summary>
        ///     Type of the entity.
        /// </summary>
        public Type EntityType { get; private set; }

        /// <summary>
        ///     DbContext type that has <see cref="DbSet{TEntity}" /> property.
        /// </summary>
        public Type DeclaringType { get; private set; }
    }
}