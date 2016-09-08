namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// Defines common properties for entity based DTOs.
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IEntityDto<TPrimaryKey>
    {
        /// <summary>
        /// Id of the entity.
        /// </summary>
        TPrimaryKey Id { get; set; }
    }
}