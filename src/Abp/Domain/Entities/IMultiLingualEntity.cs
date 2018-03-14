using System.Collections.Generic;

namespace Abp.Domain.Entities
{
    public interface IMultiLingualEntity<TTranslation> : IEntity where TTranslation : class, IEntity
    {
        ICollection<TTranslation> Translations { get; set; }
    }
}