using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace Abp.Application.Services
{
    public interface IAsyncCrudAppService<TEntityDto>
        : IAsyncCrudAppService<TEntityDto, int>
        where TEntityDto : IEntityDto<int>
    {

    }

    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, PagedAndSortedResultRequestInput>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {

    }

    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TSelectRequestInput>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TSelectRequestInput, TEntityDto, TEntityDto>
        where TSelectRequestInput : IPagedAndSortedResultRequest
        where TEntityDto : IEntityDto<TPrimaryKey>
    {

    }

    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TSelectRequestInput, in TCreateInput>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TCreateInput>
        where TSelectRequestInput : IPagedAndSortedResultRequest
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TCreateInput : IEntityDto<TPrimaryKey>
    {

    }

    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TSelectRequestInput, in TCreateInput, in TUpdateInput>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TUpdateInput, EntityDto<TPrimaryKey>>
        where TSelectRequestInput : IPagedAndSortedResultRequest
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {

    }

    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TSelectRequestInput, in TCreateInput, in TUpdateInput, in TDeleteInput>
        : IApplicationService
        where TSelectRequestInput : IPagedAndSortedResultRequest
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TDeleteInput : IEntityDto<TPrimaryKey>
    {
        Task<TEntityDto> Get(IEntityDto<TPrimaryKey> input);

        Task<PagedResultOutput<TEntityDto>> GetAll(TSelectRequestInput input);

        Task<TEntityDto> Create(TCreateInput input);

        Task<TEntityDto> Update(TUpdateInput input);

        Task Delete(TDeleteInput input);
    }
}
