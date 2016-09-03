using System;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This interface can be used to mark a DTO as both of <see cref="IInputDto"/> and <see cref="IOutputDto"/>.
    /// </summary>
    [Obsolete("Don't use this interface anymore, it's not needed.")]
    public interface IDoubleWayDto : IInputDto, IOutputDto
    {

    }
}