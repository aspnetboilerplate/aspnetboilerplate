using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Abp.Application.Services
{
    public abstract class AsyncCrudAppService<TEntity, TEntityDto>
        : AsyncCrudAppService<TEntity, TEntityDto, int>
        where TEntity : class, IEntity<int>
        where TEntityDto : IEntityDto<int>
    {
        protected AsyncCrudAppService(IRepository<TEntity, int> repository)
            : base(repository)
        {

        }
    }

    public abstract class AsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey>
        : AsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey, TEntityDto, PagedAndSortedResultRequestDto>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        protected AsyncCrudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {

        }
    }

    public abstract class AsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey, TEntityItemDto, TGetAllInput>
        : AsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey, TEntityItemDto, TGetAllInput, TEntityDto, TEntityDto>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TEntityItemDto : IEntityDto<TPrimaryKey>
    {
        protected AsyncCrudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {

        }
    }

    public abstract class AsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey, TEntityItemDto, TGetAllInput, TCreateInput>
        : AsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey, TEntityItemDto, TGetAllInput, TCreateInput, TCreateInput>
        where TGetAllInput : IPagedAndSortedResultRequest
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TEntityItemDto : IEntityDto<TPrimaryKey>
       where TCreateInput : IEntityDto<TPrimaryKey>
    {
        protected AsyncCrudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {

        }
    }

    public abstract class AsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey, TEntityItemDto, TGetAllInput, TCreateInput, TUpdateInput>
       : CrudAppServiceBase<TEntity, TEntityDto, TPrimaryKey, TEntityItemDto, TGetAllInput, TCreateInput, TUpdateInput>,
        IAsyncCrudAppService<TEntityDto, TPrimaryKey, TEntityItemDto, TGetAllInput, TCreateInput, TUpdateInput>
           where TEntity : class, IEntity<TPrimaryKey>
           where TEntityDto : IEntityDto<TPrimaryKey>
           where TEntityItemDto : IEntityDto<TPrimaryKey>
           where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        protected AsyncCrudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public virtual Task<TEntityDto> GetAsync(TPrimaryKey id)
        {
            CheckGetPermission();
            var query = CreateFilteredByIdQuery(id);
            return AsyncQueryableExecuter.FirstOrDefaultAsync(query);
        }

        protected virtual IQueryable<TEntityDto> CreateFilteredByIdQuery(TPrimaryKey id)
        {
            return Repository.GetAll().Where(e => e.Id.Equals(id)).ProjectTo<TEntityDto>(ConfigurationProvider);
        }

        public virtual async Task<PagedResultDto<TEntityItemDto>> GetAllAsync(TGetAllInput input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncQueryableExecuter.CountAsync(query);
            if (totalCount == 0)
            {
                return new PagedResultDto<TEntityItemDto>(0, new List<TEntityItemDto>());
            }

            //query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var dtoItems = await AsyncQueryableExecuter.ToListAsync(query);

            return new PagedResultDto<TEntityItemDto>(
                totalCount,
                dtoItems
            );
        }

        public virtual async Task<TEntityDto> CreateAsync(TCreateInput input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);

            await ProcessBeforeCreateAsync(entity, input);

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            await ProcessAfterCreateAsync(entity, input);

            return MapToEntityDto(entity);
        }

        protected virtual Task ProcessBeforeCreateAsync(TEntity entity, TCreateInput input)
        {
            return Task.FromResult(true);
        }

        protected virtual Task ProcessAfterCreateAsync(TEntity entity, TCreateInput input)
        {
            return Task.FromResult(true);
        }

        public virtual async Task<TEntityDto> UpdateAsync(TUpdateInput input)
        {
            CheckUpdatePermission();

            var entity = await GetEntityByIdAsync(input.Id);

            MapToEntity(input, entity);

            await ProcessBeforeUpdateAsync(entity, input);

            await Repository.UpdateAsync(entity);

            await CurrentUnitOfWork.SaveChangesAsync();

            await ProcessAfterUpdateAsync(entity, input);

            return MapToEntityDto(entity);
        }
        protected virtual Task ProcessBeforeUpdateAsync(TEntity entity, TUpdateInput input)
        {
            return Task.FromResult(true);
        }

        protected virtual Task ProcessAfterUpdateAsync(TEntity entity, TUpdateInput input)
        {
            return Task.FromResult(true);
        }

        public virtual Task DeleteAsync(TPrimaryKey id)
        {
            CheckDeletePermission();

            return Repository.DeleteAsync(id);
        }

        protected virtual Task<TEntity> GetEntityByIdAsync(TPrimaryKey id)
        {
            return Repository.GetAsync(id);
        }
    }
}
