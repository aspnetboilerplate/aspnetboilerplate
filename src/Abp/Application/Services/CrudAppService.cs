using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;

namespace Abp.Application.Services
{
    public abstract class CrudAppService<TEntity, TEntityDto>
        : CrudAppService<TEntity, TEntityDto, int>
        where TEntity : class, IEntity<int>
        where TEntityDto : EntityRequestInput<int>
    {
        protected CrudAppService(IRepository<TEntity, int> repository)
            : base(repository)
        {

        }
    }

    public abstract class CrudAppService<TEntity, TEntityDto, TPrimaryKey>
        : CrudAppService<IRepository<TEntity, TPrimaryKey>, TEntity, TEntityDto, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : EntityRequestInput<TPrimaryKey>
    {
        protected CrudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {

        }
    }

    public abstract class CrudAppService<TRepository, TEntity, TEntityDto, TPrimaryKey>
        : CrudAppService<TRepository, TEntity, TEntityDto, TPrimaryKey, PagedResultRequest>
        where TRepository : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : EntityRequestInput<TPrimaryKey>
    {
        protected CrudAppService(TRepository repository)
            : base(repository)
        {

        }
    }

    public abstract class CrudAppService<TRepository, TEntity, TEntityDto, TPrimaryKey, TSelectRequestInput>
        : CrudAppService<TRepository, TEntity, TEntityDto, TPrimaryKey, TSelectRequestInput, TEntityDto, TEntityDto>
        where TSelectRequestInput : IPagedResultRequest
        where TRepository : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : EntityRequestInput<TPrimaryKey>
    {
        protected CrudAppService(TRepository repository)
            : base(repository)
        {

        }
    }

    public abstract class CrudAppService<TRepository, TEntity, TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TUpdateInput>
       : ApplicationService, ICrudAppService<TEntityDto, TPrimaryKey, TSelectRequestInput, TCreateInput, TUpdateInput>
       where TSelectRequestInput : IPagedResultRequest
       where TRepository : IRepository<TEntity, TPrimaryKey>
       where TEntity : class, IEntity<TPrimaryKey>
       where TUpdateInput : EntityRequestInput<TPrimaryKey>
    {
        protected readonly TRepository Repository;

        protected CrudAppService(TRepository repository)
        {
            Repository = repository;
        }

        public virtual TEntityDto Get(IdInput<TPrimaryKey> input)
        {
            return ObjectMapper.Map<TEntityDto>(Repository.Get(input.Id));
        }

        public virtual PagedResultOutput<TEntityDto> GetAll(TSelectRequestInput input)
        {
            var query = CreateQueryable(input);

            var totalCount = query.Count();
            var items = query.OrderByDescending(e => e.Id).PageBy(input).ToList();

            return new PagedResultOutput<TEntityDto>(
                totalCount,
                ObjectMapper.Map<List<TEntityDto>>(items)
            );
        }

        public virtual TPrimaryKey Create(TCreateInput input)
        {
            return Repository.InsertAndGetId(ObjectMapper.Map<TEntity>(input));
        }

        public virtual void Update(TUpdateInput input)
        {
            var entity = Repository.Get(input.Id);
            ObjectMapper.Map(input, entity);
        }

        public virtual void Delete(IdInput<TPrimaryKey> input)
        {
            Repository.Delete(input.Id);
        }

        protected virtual IQueryable<TEntity> CreateQueryable(TSelectRequestInput input)
        {
            return Repository.GetAll();
        }
    }
}
