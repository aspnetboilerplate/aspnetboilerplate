namespace Abp.Domain.Entities
{
    public interface IEntityTranslation: IEntity
    {
        string Language { get; set; }
    }

    public interface IEntityTranslation<TEntity> : IEntityTranslation
    {
        TEntity Core { get; set; }

        int CoreId { get; set; }
    }
}