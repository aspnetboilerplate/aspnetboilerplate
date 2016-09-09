using System.Linq;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;

namespace Abp.Application.Services
{
    public abstract class CrudAppService<TEntity, TEntityDto>
        : CrudAppService<TEntity, TEntityDto, int>
        where TEntity : class, IEntity<int>
        where TEntityDto : IEntityDto<int>
    {
        protected CrudAppService(IRepository<TEntity, int> repository)
            : base(repository)
        {

        }
    }

    public abstract class CrudAppService<TEntity, TEntityDto, TPrimaryKey>
        : CrudAppService<TEntity, TEntityDto, TPrimaryKey, PagedAndSortedResultRequestInput>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        protected CrudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {

        }
    }

    public abstract class CrudAppService<TEntity, TEntityDto, TPrimaryKey, TSelectRequestInput>
        : CrudAppService<TEntity, TEntityDto, TPrimaryKey, TSelectRequestInput, TEntityDto, TEntityDto>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        protected CrudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {

        }
    }

    public abstract class CrudAppService<TEntity, TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput>
        : CrudAppService<TEntity, TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TCreateInput>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TCreateInput : IEntityDto<TPrimaryKey>
    {
        protected CrudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {

        }
    }

    public abstract class CrudAppService<TEntity, TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TUpdateInput>
        : CrudAppService<TEntity, TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TUpdateInput, EntityDto<TPrimaryKey>>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        protected CrudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {

        }
    }

    public abstract class CrudAppService<TEntity, TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TUpdateInput, TDeleteInput>
       : CrudAppServiceBase<TEntity, TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TUpdateInput>, ICrudAppService<TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TUpdateInput, TDeleteInput>
       where TEntity : class, IEntity<TPrimaryKey>
       where TEntityDto : IEntityDto<TPrimaryKey>
       where TUpdateInput : IEntityDto<TPrimaryKey>
       where TDeleteInput : IEntityDto<TPrimaryKey>
    {
        protected CrudAppService(IRepository<TEntity, TPrimaryKey> repository) 
            : base(repository)
        {

        }

        public virtual TEntityDto Get(IdInput<TPrimaryKey> input)
        {
            var entity = GetEntityById(input.Id);
            return MapToEntityDto(entity);
        }

        public virtual PagedResultOutput<TEntityDto> GetAll(TSelectRequestInput input)
        {
            var query = CreateFilteredQuery(input);

            var totalCount = query.Count();

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = query.ToList();

            return new PagedResultOutput<TEntityDto>(
                totalCount,
                entities.Select(MapToEntityDto).ToList()
            );
        }

        public virtual TEntityDto Create(TCreateInput input)
        {
            var entity = MapToEntity(input);

            Repository.Insert(entity);
            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(entity);
        }

        public virtual TEntityDto Update(TUpdateInput input)
        {
            var entity = GetEntityById(input.Id);

            MapToEntity(input, entity);
            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(entity);
        }

        public virtual void Delete(TDeleteInput input)
        {
            Repository.Delete(input.Id);
        }

        protected virtual TEntity GetEntityById(TPrimaryKey id)
        {
            return Repository.Get(id);
        }
    }
}
