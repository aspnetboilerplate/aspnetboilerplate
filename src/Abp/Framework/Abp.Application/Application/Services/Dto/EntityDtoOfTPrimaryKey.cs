namespace Abp.Application.Services.Dto
{
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
