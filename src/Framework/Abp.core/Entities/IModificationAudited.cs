using System;
using Abp.Entities.Core;

namespace Abp.Entities
{
    public interface IModificationAudited
    {
        DateTime? LastModificationDate { get; set; }

        User LastModifier { get; set; }
    }
}