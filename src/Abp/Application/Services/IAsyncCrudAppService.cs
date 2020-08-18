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
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, PagedAndSortedResultRequestDto>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {

    }

    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, in TGetAllInput>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TEntityDto, TGetAllInput, TEntityDto, TEntityDto>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {

    }

    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, TEntityItemDto, in TGetAllInput, in TCreateInput>
        : IAsyncCrudAppService<TEntityDto, TPrimaryKey, TEntityItemDto, TGetAllInput, TCreateInput, TCreateInput>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TEntityItemDto : IEntityDto<TPrimaryKey>
        where TCreateInput : IEntityDto<TPrimaryKey>
    {

    }

    public interface IAsyncCrudAppService<TEntityDto, TPrimaryKey, TEntityItemDto, in TGetAllInput, in TCreateInput, in TUpdateInput>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TEntityItemDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        Task<TEntityDto> GetAsync(TPrimaryKey input);

        Task<PagedResultDto<TEntityItemDto>> GetAllAsync(TGetAllInput input);

        Task<TEntityDto> CreateAsync(TCreateInput input);

        Task<TEntityDto> UpdateAsync(TUpdateInput input);

        Task DeleteAsync(TPrimaryKey input);
    }
}
