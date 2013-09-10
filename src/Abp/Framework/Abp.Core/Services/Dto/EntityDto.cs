namespace Abp.Services.Dto
{
    /// <summary>
    /// A shortcut of <see cref="EntityDto{TPrimaryKey}"/> for most used primary key type (Int32).
    /// </summary>
    public class EntityDto : EntityDto<int>
    {

    }

    /// <summary>
    /// This base class can be used to simplify defining an entity based DTO.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key</typeparam>
    public class EntityDto<TPrimaryKey> : IDto
    {
        /// <summary>
        /// Id of the entity.
        /// </summary>
        public TPrimaryKey Id { get; set; }
    }
}