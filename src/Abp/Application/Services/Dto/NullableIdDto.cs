using System;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This DTO can be directly used (or inherited)
    /// to pass an nullable Id value to an application service method.
    /// </summary>
    /// <typeparam name="TId">Type of the Id</typeparam>
    [Serializable]
    public class NullableIdDto<TId>
        where TId : struct
    {
        public TId? Id { get; set; }

        public NullableIdDto()
        {

        }

        public NullableIdDto(TId? id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// A shortcut of <see cref="NullableIdDto{TId}"/> for <see cref="int"/>.
    /// </summary>
    [Serializable]
    public class NullableIdDto : NullableIdDto<int>
    {
        public NullableIdDto()
        {

        }

        public NullableIdDto(int? id)
            : base(id)
        {

        }
    }
}