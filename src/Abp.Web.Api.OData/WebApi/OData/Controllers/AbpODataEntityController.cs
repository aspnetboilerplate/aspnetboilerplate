using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;

namespace Abp.WebApi.OData.Controllers
{
    public abstract class AbpODataEntityController<TEntity> : AbpODataEntityController<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected AbpODataEntityController(IRepository<TEntity> repository)
            : base(repository)
        {

        }
    }

    public abstract class AbpODataEntityController<TEntity, TPrimaryKey> : AbpODataController
        where TPrimaryKey : IEquatable<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected IRepository<TEntity, TPrimaryKey> Repository { get; private set; }
        
        protected AbpODataEntityController(IRepository<TEntity, TPrimaryKey> repository)
        {
            Repository = repository;
        }

        [EnableQuery]
        public virtual IQueryable<TEntity> Get()
        {
            return Repository.GetAll();
        }

        [EnableQuery]
        public virtual SingleResult<TEntity> Get([FromODataUri] TPrimaryKey key)
        {
            var entity = Repository.GetAll().Where(e => e.Id.Equals(key));

            return SingleResult.Create(entity);
        }

        public virtual async Task<IHttpActionResult> Post(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdEntity = await Repository.InsertAsync(entity);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            
            return Created(createdEntity);
        }

        public virtual async Task<IHttpActionResult> Patch([FromODataUri] TPrimaryKey key, Delta<TEntity> entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var dbLookup = await Repository.GetAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            
            entity.Patch(dbLookup);

            return Updated(entity);
        }

        public virtual async Task<IHttpActionResult> Put([FromODataUri] TPrimaryKey key, TEntity update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!key.Equals(update.Id))
            {
                return BadRequest();
            }
            
            var updated = await Repository.UpdateAsync(update);

            return Updated(updated);
        }

        public virtual async Task<IHttpActionResult> Delete([FromODataUri] TPrimaryKey key)
        {
            var product = await Repository.GetAsync(key);
            if (product == null)
            {
                return NotFound();
            }
            
            await Repository.DeleteAsync(key);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
