using System;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This attribute is used to indicate that declaring method is transactional (atomic) and should be considered as a unit of work.
    /// A method that has this attribute is intercepted, a transaction starts before call the method.
    /// At the end of method call, transaction is commited and all changes applied to the database if there is no exception,
    /// othervise it's rolled back.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)] //TODO: Also Apply to Classes in the future.
    public class UnitOfWorkAttribute : Attribute
    {

    }
}