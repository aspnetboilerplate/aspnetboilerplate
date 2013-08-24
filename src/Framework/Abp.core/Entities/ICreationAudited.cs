using System;
using Abp.Entities.Core;

namespace Abp.Entities
{
    public interface ICreationAudited
    {
        DateTime CreationDate { get; set; }

        User Creator { get; set; }
    }
}