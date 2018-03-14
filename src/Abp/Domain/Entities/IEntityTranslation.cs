namespace Abp.Domain.Entities
{
    public interface IEntityTranslation<TEntity, TPrimaryKey> : IEntity<TPrimaryKey>
    {
        TEntity Core { get; set; }

        int CoreId { get; set; }

        string Language { get; set; }
    }
}