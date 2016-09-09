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

    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TEntityDto, TEntityDto>
        where TGetAllInput : IPagedAndSortedResultRequest
        where TEntityDto : IEntityDto<TPrimaryKey>
    {

    }

    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput, in TCreateInput>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TCreateInput>
        where TGetAllInput : IPagedAndSortedResultRequest
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TCreateInput : IEntityDto<TPrimaryKey>
    {

    }

    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput, in TCreateInput, in TUpdateInput>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, EntityDto<TPrimaryKey>>
        where TGetAllInput : IPagedAndSortedResultRequest
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {

    }

    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput, in TCreateInput, in TUpdateInput, in TDeleteInput>
        : IApplicationService
        where TGetAllInput : IPagedAndSortedResultRequest
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TDeleteInput : IEntityDto<TPrimaryKey>
    {
        Task<TEntityDto> Get(IEntityDto<TPrimaryKey> input);

        Task<PagedResultOutput<TEntityDto>> GetAll(TGetAllInput input);

        Task<TEntityDto> Create(TCreateInput input);

        Task<TEntityDto> Update(TUpdateInput input);

        Task Delete(TDeleteInput input);
    }
}
