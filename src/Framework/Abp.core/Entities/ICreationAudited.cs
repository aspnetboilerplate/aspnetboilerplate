using System;

namespace Abp.Entities
{
    public interface ICreationAudited
    {
        DateTime CreationDate { get; set; }

        User Creator { get; set; }
    }
}