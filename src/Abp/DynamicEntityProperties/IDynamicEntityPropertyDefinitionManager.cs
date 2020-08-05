using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.UI.Inputs;

namespace Abp.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyDefinitionManager
    {
        /// <summary>
        /// Adds the specified inputType to allowed list. Throws exception if it is already added
        /// </summary>
        void AddAllowedInputType<TInputType>() where TInputType : IInputType;

        /// <summary>
        /// Gets a Input Type by name.
        /// </summary>
        IInputType GetOrNullAllowedInputType(string name);

        /// <summary>
        /// Gets all input type names
        /// </summary>
        List<string> GetAllAllowedInputTypeNames();

        /// <summary>
        /// Gets all input types
        /// </summary>
        List<IInputType> GetAllAllowedInputTypes();

        /// <summary>
        /// Returns if allowed input types contains the given name
        /// </summary>
        bool ContainsInputType(string name);

        /// <summary>
        /// Adds the specified entity to entity list. Throws exception if it is already added
        /// </summary>
        void AddEntity<TEntity>()
            where TEntity : IEntity<int>;

        /// <summary>
        /// Adds the specified entity to entity list. Throws exception if it is already added
        /// </summary>
        void AddEntity<TEntity, TPrimaryKey>()
            where TEntity : IEntity<TPrimaryKey>;

        /// <summary>
        /// Returns all entities
        /// </summary>
        List<string> GetAllEntities();

        /// <summary>
        /// Returns if contains entity
        /// </summary>
        bool ContainsEntity(string entityFullName);

        /// <summary>
        /// Returns if contains entity
        /// </summary>
        bool ContainsEntity<TEntity, TPrimaryKey>()
            where TEntity : IEntity<TPrimaryKey>;

        /// <summary>
        /// Returns if contains entity
        /// </summary>
        bool ContainsEntity<TEntity>() 
            where TEntity : IEntity<int>;
    }
}
