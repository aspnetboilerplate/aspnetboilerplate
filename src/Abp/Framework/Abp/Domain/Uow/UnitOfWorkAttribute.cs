using System;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This attribute is used to indicate that declaring method is transactional (atomic) and should be considered as a unit of work.
    /// A method that has this attribute is intercepted, a database connection is opened and a transaction is started before call the method.
    /// At the end of method call, transaction is commited and all changes applied to the database if there is no exception,
    /// othervise it's rolled back. 
    /// </summary>
    /// <remarks>
    /// This attribute has no effect if there is already a unit of work before calling this method, so this method uses the same transaction.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitOfWorkAttribute : Attribute
    {

    }
}