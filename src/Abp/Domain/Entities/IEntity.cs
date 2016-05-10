using System;

namespace Abp.Domain.Entities
{
    /// <summary>
    ///     A shortcut of <see cref="IEntity{TPrimaryKey}" /> for most used primary key type (<see cref="Guid" />).
    /// </summary>
    public interface IEntity : IEntity<Guid>
    {
    }
}