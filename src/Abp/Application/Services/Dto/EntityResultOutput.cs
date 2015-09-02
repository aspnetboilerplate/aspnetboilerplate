using System;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This <see cref="IOutputDto"/> can be used to send Id of an entity as response from an <see cref="IApplicationService"/> method.
    /// </summary>
    [Serializable]
    public class EntityResultOutput : EntityResultOutput<int>, IEntityDto
    {
        /// <summary>
        /// Creates a new <see cref="EntityResultOutput"/> object.
        /// </summary>
        public EntityResultOutput()
        {

        }

        /// <summary>
        /// Creates a new <see cref="EntityResultOutput"/> object.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        public EntityResultOutput(int id)
            : base(id)
        {

        }
    }
}