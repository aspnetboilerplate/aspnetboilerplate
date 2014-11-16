using System;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This <see cref="IOutputDto"/> can be used to send Id of an entity as response from an <see cref="IApplicationService"/> method.
    /// </summary>
    [Serializable]
    public class EntityResultOutput : EntityResultOutput<int>, IEntityDto
    {

    }
}