using Abp.Runtime.Validation;

namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// This interface is used to define DTOs those are used as input parameters.
    /// </summary>
    public interface IInputDto : IDto, IValidate
    {

    }
}