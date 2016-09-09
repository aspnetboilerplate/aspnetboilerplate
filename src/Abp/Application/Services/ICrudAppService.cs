using Abp.Application.Services.Dto;

namespace Abp.Application.Services
{
    public interface ICrudAppService<TEntityDto>
        : ICrudAppService<TEntityDto, int>
        where TEntityDto : IEntityDto<int>
    {

    }

    public interface ICrudAppService<TEntityDto, TPrimaryKey>
        : ICrudAppService<TEntityDto, TPrimaryKey, PagedAndSortedResultRequestInput>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {

    }

    public interface ICrudAppService<TEntityDto, TPrimaryKey, in TSelectRequestInput>
        : ICrudAppService<TEntityDto, TPrimaryKey, TSelectRequestInput, TEntityDto, TEntityDto>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {

    }

    public interface ICrudAppService<TEntityDto, TPrimaryKey, in TSelectRequestInput, in TCreateInput>
        : ICrudAppService<TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TCreateInput>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TCreateInput : IEntityDto<TPrimaryKey>
    {

    }

    public interface ICrudAppService<TEntityDto, TPrimaryKey, in TSelectRequestInput, in TCreateInput, in TUpdateInput>
        : ICrudAppService<TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TUpdateInput, EntityDto<TPrimaryKey>>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {

    }

    public interface ICrudAppService<TEntityDto, TPrimaryKey, in TSelectRequestInput, in TCreateInput, in TUpdateInput, in TDeleteInput>
        : IApplicationService
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TDeleteInput : IEntityDto<TPrimaryKey>
    {
        TEntityDto Get(IEntityDto<TPrimaryKey> input);

        PagedResultOutput<TEntityDto> GetAll(TSelectRequestInput input);

        TEntityDto Create(TCreateInput input);

        TEntityDto Update(TUpdateInput input);

        void Delete(TDeleteInput input);
    }
}
