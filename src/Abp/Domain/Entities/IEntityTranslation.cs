namespace Abp.Domain.Entities
{
    public interface IEntityTranslation<TEntity>
    {
        TEntity Core { get; set; }

        int CoreId { get; set; }

        string Language { get; set; }
    }
}