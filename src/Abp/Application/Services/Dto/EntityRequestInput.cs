using System;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This <see cref="IInputDto"/> can be used to send Id of an entity to an <see cref="IApplicationService"/> method.
    /// </summary>
    [Serializable]
    public class EntityRequestInput : EntityRequestInput<int>, IEntityDto
    {
        /// <summary>
        /// Creates a new <see cref="EntityRequestInput"/> object.
        /// </summary>
        public EntityRequestInput()
        {

        }

        /// <summary>
        /// Creates a new <see cref="EntityRequestInput"/> object.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        public EntityRequestInput(int id)
            : base(id)
        {

        }
    }
}