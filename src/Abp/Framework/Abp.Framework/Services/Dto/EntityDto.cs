namespace Abp.Services.Dto
{
    public abstract class EntityDto<TPrimaryKey>
    {
        /// <summary>
        /// Id of the entity.
        /// </summary>
        public TPrimaryKey Id { get; set; }
    }

    public abstract class EntityDto : EntityDto<int>
    {
        
    }
}