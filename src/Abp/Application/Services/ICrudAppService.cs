using Abp.Application.Services.Dto;

namespace Abp.Application.Services
{
    public interface ICrudAppService<TEntityDto>
        : ICrudAppService<TEntityDto, int>
    {

    }

    public interface ICrudAppService<TEntityDto, TPrimaryKey>
        : ICrudAppService<TEntityDto, TPrimaryKey, PagedResultRequest>
    {

    }

    public interface ICrudAppService<TEntityDto, TPrimaryKey, in TSelectRequestInput>
        : ICrudAppService<TEntityDto, TPrimaryKey, TSelectRequestInput, TEntityDto, TEntityDto>
        where TSelectRequestInput : IPagedResultRequest
    {

    }

    public interface ICrudAppService<TEntityDto, TPrimaryKey, in TSelectRequestInput, in TCreateInput, in TUpdateInput>
        : IApplicationService
        where TSelectRequestInput : IPagedResultRequest
    {
        TEntityDto Get(IdInput<TPrimaryKey> input);

        PagedResultOutput<TEntityDto> GetAll(TSelectRequestInput input);

        TPrimaryKey Create(TCreateInput input);

        void Update(TUpdateInput input);

        void Delete(IdInput<TPrimaryKey> input);
    }
}
