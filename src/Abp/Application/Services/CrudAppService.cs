using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq;
using Abp.Linq.Extensions;

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
        where TSelectRequestInput : IPagedAndSortedResultRequest
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        protected CrudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {

        }
    }

    public abstract class CrudAppService<TEntity, TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TUpdateInput>
       : ApplicationService, ICrudAppService<TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TUpdateInput>
       where TSelectRequestInput : IPagedAndSortedResultRequest
       where TEntity : class, IEntity<TPrimaryKey>
       where TEntityDto : IEntityDto<TPrimaryKey>
       where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        protected readonly IRepository<TEntity, TPrimaryKey> Repository;

        protected CrudAppService(IRepository<TEntity, TPrimaryKey> repository)
        {
            Repository = repository;
        }

        public virtual TEntityDto Get(IdInput<TPrimaryKey> input)
        {
            var entity = GetEntityById(input.Id);
            return ObjectMapper.Map<TEntityDto>(entity);
        }

        public virtual PagedResultOutput<TEntityDto> GetAll(TSelectRequestInput input)
        {
            var query = CreateQueryable(input);

            var totalCount = query.Count();

            if (!input.Sorting.IsNullOrWhiteSpace())
            {
                query = query.OrderBy(input.Sorting);
            }
            else
            {
                query = query.OrderByDescending(e => e.Id);
            }

            var items = query.PageBy(input).ToList();

            return new PagedResultOutput<TEntityDto>(
                totalCount,
                ObjectMapper.Map<List<TEntityDto>>(items)
            );
        }

        public virtual TPrimaryKey Create(TCreateInput input)
        {
            var entity = ObjectMapper.Map<TEntity>(input);
            return Repository.InsertAndGetId(entity);
        }

        public virtual void Update(TUpdateInput input)
        {
            var entity = GetEntityById(input.Id);
            ObjectMapper.Map(input, entity);
        }

        public virtual void Delete(IdInput<TPrimaryKey> input)
        {
            Repository.Delete(input.Id);
        }

        protected virtual TEntity GetEntityById(TPrimaryKey id)
        {
            return Repository.Get(id);
        }

        protected virtual IQueryable<TEntity> CreateQueryable(TSelectRequestInput input)
        {
            return Repository.GetAll();
        }
    }
}
