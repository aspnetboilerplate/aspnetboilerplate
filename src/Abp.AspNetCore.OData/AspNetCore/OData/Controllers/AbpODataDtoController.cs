using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace Abp.AspNetCore.OData.Controllers;

public abstract class AbpODataDtoController<TEntity, TOutputDto, TInputDto> : AbpODataDtoController<TEntity, TOutputDto, TInputDto, int>
    where TEntity : class, IEntity<int>
    where TInputDto : class
    where TOutputDto : class, IEntityDto<int>
{
    protected AbpODataDtoController(IRepository<TEntity> repository, IObjectMapper objectMapper)
        : base(repository, objectMapper)
    {
    }
}

public abstract class AbpODataDtoController<TEntity, TOutputDto, TInputDto, TPrimaryKey> : AbpODataController
    where TPrimaryKey : IEquatable<TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
    where TInputDto : class
    where TOutputDto : class, IEntityDto<TPrimaryKey>
{
    protected IRepository<TEntity, TPrimaryKey> Repository { get; private set; }

    protected IObjectMapper ObjectMapper { get; private set; }

    protected AbpODataDtoController(
        IRepository<TEntity, TPrimaryKey> repository,
        IObjectMapper objectMapper)
    {
        Repository = repository;
        ObjectMapper = objectMapper;
    }

    protected virtual string GetPermissionName { get; set; }

    protected virtual string GetAllPermissionName { get; set; }

    protected virtual string CreatePermissionName { get; set; }

    protected virtual string UpdatePermissionName { get; set; }

    protected virtual string DeletePermissionName { get; set; }

    [EnableQuery]
    public virtual IQueryable<TOutputDto> Get()
    {
        CheckGetAllPermission();

        return ObjectMapper.ProjectTo<TOutputDto>(Repository.GetAll());
    }

    [EnableQuery]
    public virtual SingleResult<TOutputDto> Get([FromODataUri] TPrimaryKey key)
    {
        CheckGetPermission();

        var entity = Repository.GetAll().Where(e => e.Id.Equals(key));

        return SingleResult.Create(ObjectMapper.ProjectTo<TOutputDto>(entity));
    }

    public virtual async Task<IActionResult> Post([FromBody] TInputDto entity)
    {
        CheckCreatePermission();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdEntity = await Repository.InsertAsync(ObjectMapper.Map<TEntity>(entity));
        await UnitOfWorkManager.Current.SaveChangesAsync();

        return Created(createdEntity);
    }

    public virtual async Task<IActionResult> Patch([FromODataUri] TPrimaryKey key, [FromBody] TInputDto inputDto)
    {
        CheckUpdatePermission();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = await Repository.GetAsync(key);
        if (entity == null)
        {
            return NotFound();
        }

        ObjectMapper.Map(inputDto, entity);
        await Repository.UpdateAsync(entity);

        return Updated(entity);
    }

    public virtual async Task<IActionResult> Put([FromODataUri] TPrimaryKey key, [FromBody] TEntity inputDto)
    {
        CheckUpdatePermission();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!key.Equals(inputDto.Id))
        {
            return BadRequest();
        }

        var newEntity = ObjectMapper.Map<TEntity>(inputDto);
        var updatedEntity = await Repository.UpdateAsync(newEntity);

        return Updated(ObjectMapper.Map<TOutputDto>(updatedEntity));
    }

    public virtual async Task<IActionResult> Delete([FromODataUri] TPrimaryKey key)
    {
        CheckDeletePermission();

        var product = await Repository.GetAsync(key);
        if (product == null)
        {
            return NotFound();
        }

        await Repository.DeleteAsync(key);

        return StatusCode((int)HttpStatusCode.NoContent);
    }

    protected virtual void CheckPermission(string permissionName)
    {
        if (!string.IsNullOrEmpty(permissionName))
        {
            PermissionChecker.Authorize(permissionName);
        }
    }

    protected virtual void CheckGetPermission()
    {
        CheckPermission(GetPermissionName);
    }

    protected virtual void CheckGetAllPermission()
    {
        CheckPermission(GetAllPermissionName);
    }

    protected virtual void CheckCreatePermission()
    {
        CheckPermission(CreatePermissionName);
    }

    protected virtual void CheckUpdatePermission()
    {
        CheckPermission(UpdatePermissionName);
    }

    protected virtual void CheckDeletePermission()
    {
        CheckPermission(DeletePermissionName);
    }
}