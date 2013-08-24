using System;

namespace Abp.Entities
{
    public interface IModificationAudited
    {
        DateTime? LastModificationDate { get; set; }

        User LastModifier { get; set; }
    }
}