using System;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    ///     This <see cref="IInputDto" /> can be directly used (or inherited)
    ///     to pass an nullable Id value to an application service method.
    /// </summary>
    /// <typeparam name="TId">Type of the Id</typeparam>
    public class NullableIdInput<TId> : IInputDto
        where TId : struct
    {
        public NullableIdInput()
        {
        }

        public NullableIdInput(TId? id)
        {
            Id = id;
        }

        public TId? Id { get; set; }
    }

    /// <summary>
    ///     A shortcut of <see cref="NullableIdInput{TPrimaryKey}" /> for <see cref="Guid" />.
    /// </summary>
    public class NullableIdInput : NullableIdInput<Guid>
    {
        public NullableIdInput()
        {
        }

        public NullableIdInput(Guid? id)
            : base(id)
        {
        }
    }
}