using System.Collections.Generic;

namespace Abp.Domain.Entities
{
    public interface IMultiLingualEntity<TTranslation>
    {
        ICollection<TTranslation> Translations { get; set; }
    }
}